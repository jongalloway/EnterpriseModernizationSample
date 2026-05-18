using System;
using System.Collections.Generic;
using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Configuration;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class RoutePlanningService : StoreOpsServiceBase, IRoutePlanningService
    {
        private readonly DispatchBusinessRules _rules;

        public RoutePlanningService()
            : this(new DispatchBusinessRules())
        {
        }

        public RoutePlanningService(DispatchBusinessRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public IList<RoutePlan> BuildRoutePlans(string storeNumber, IList<DispatchTicket> tickets)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                var normalizedTickets = NormalizeTickets(normalizedStoreNumber, tickets);
                return normalizedTickets
                    .GroupBy(ticket => ticket.RouteZone)
                    .Select(group => CreateRoutePlan(normalizedStoreNumber, group.Key, group.ToList()))
                    .OrderBy(plan => plan.ReadyWindowStartLocal)
                    .ThenBy(plan => plan.RouteZone)
                    .ToList();
            }, "RoutePlanningService.BuildRoutePlans");
        }

        private RoutePlan CreateRoutePlan(string storeNumber, string routeZone, IList<DispatchTicket> tickets)
        {
            var zoneBoundary = _rules.GetZoneBoundary(routeZone);
            var orderedTickets = tickets
                .OrderByDescending(ticket => ticket.PriorityScore)
                .ThenBy(ticket => ticket.PromiseTimeLocal)
                .ThenBy(ticket => ticket.TicketId)
                .ToList();

            var stops = new List<RouteStop>();
            var totalDistance = 0m;
            var totalTravelMinutes = 0;
            for (var index = 0; index < orderedTickets.Count; index++)
            {
                var ticket = orderedTickets[index];
                var distance = ticket.RouteDistanceMiles > 0m
                    ? ticket.RouteDistanceMiles
                    : zoneBoundary.DefaultDistanceMiles + (index * 0.6m);
                var travelMinutes = ticket.EstimatedTravelMinutes > 0
                    ? ticket.EstimatedTravelMinutes
                    : EstimateTravelMinutes(distance, zoneBoundary.TravelFactor);
                var arrivalTime = ticket.ReadyAtLocal.AddMinutes(travelMinutes + (index * _rules.StopServiceMinutes));

                totalDistance += distance;
                totalTravelMinutes += travelMinutes + _rules.StopServiceMinutes;
                stops.Add(new RouteStop
                {
                    TicketId = ticket.TicketId,
                    CustomerName = ticket.CustomerName,
                    RouteZone = routeZone,
                    SequenceNumber = index + 1,
                    DistanceMiles = decimal.Round(distance, 1),
                    EstimatedTravelMinutes = travelMinutes,
                    ReadyAtLocal = ticket.ReadyAtLocal,
                    PromiseTimeLocal = ticket.PromiseTimeLocal,
                    EstimatedArrivalLocal = arrivalTime,
                    PriorityLabel = GetPriorityLabel(ticket),
                    DriverCode = ticket.DriverCode
                });
            }

            var readyStart = orderedTickets.Min(ticket => ticket.ReadyAtLocal);
            var readyEnd = orderedTickets.Max(ticket => ticket.PromiseTimeLocal);
            var driverCount = Math.Max(1, (int)Math.Ceiling((double)orderedTickets.Count / _rules.MaxBatchSize));

            return new RoutePlan
            {
                StoreNumber = storeNumber,
                RouteZone = routeZone,
                ZoneBoundary = zoneBoundary.BoundaryCode,
                DispatchWave = BuildDispatchWave(readyStart, zoneBoundary.PriorityRank),
                DriverCount = driverCount,
                TotalDistanceMiles = decimal.Round(totalDistance, 1),
                EstimatedTravelMinutes = totalTravelMinutes,
                ReadyWindowStartLocal = readyStart,
                ReadyWindowEndLocal = readyEnd,
                PlannerNote = orderedTickets.Any(ticket => ticket.RequiresPairing)
                    ? "Keep counter-hold tickets in the same send window for this zone."
                    : "Stage the route as one balanced send.",
                Stops = stops
            };
        }

        private IList<DispatchTicket> NormalizeTickets(string storeNumber, IList<DispatchTicket> tickets)
        {
            var normalized = new List<DispatchTicket>();
            foreach (var ticket in tickets ?? new List<DispatchTicket>())
            {
                if (ticket == null)
                {
                    continue;
                }

                var readyAtLocal = ticket.ReadyAtLocal == default(DateTime)
                    ? DateTime.Today.AddHours(17).AddMinutes(10 + (ticket.TicketId % 5) * 4)
                    : ticket.ReadyAtLocal;
                var promiseTimeLocal = ticket.PromiseTimeLocal == default(DateTime)
                    ? readyAtLocal.AddMinutes(18)
                    : ticket.PromiseTimeLocal;

                normalized.Add(new DispatchTicket
                {
                    TicketId = ticket.TicketId,
                    StoreNumber = string.IsNullOrWhiteSpace(ticket.StoreNumber) ? storeNumber : ticket.StoreNumber,
                    DriverCode = ticket.DriverCode,
                    RouteZone = string.IsNullOrWhiteSpace(ticket.RouteZone) ? "General Delivery" : ticket.RouteZone,
                    CustomerName = string.IsNullOrWhiteSpace(ticket.CustomerName) ? "Delivery Customer" : ticket.CustomerName,
                    DeliveryAddress = ticket.DeliveryAddress,
                    ReadyAtLocal = readyAtLocal,
                    PromiseTimeLocal = promiseTimeLocal,
                    RouteDistanceMiles = ticket.RouteDistanceMiles,
                    EstimatedTravelMinutes = ticket.EstimatedTravelMinutes,
                    RequiresPairing = ticket.RequiresPairing,
                    Status = ticket.Status,
                    PriorityScore = ticket.PriorityScore <= 0 ? 50 : ticket.PriorityScore
                });
            }

            return normalized;
        }

        private int EstimateTravelMinutes(decimal distanceMiles, decimal travelFactor)
        {
            var adjustedMiles = distanceMiles * (travelFactor <= 0m ? 1.0m : travelFactor);
            var minutes = adjustedMiles / (_rules.AverageSpeedMph <= 0m ? 24.0m : _rules.AverageSpeedMph) * 60m;
            return (int)Math.Ceiling(minutes) + _rules.StopServiceMinutes;
        }

        private static string NormalizeStoreNumber(string storeNumber)
        {
            return string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
        }

        private string BuildDispatchWave(DateTime readyAtLocal, int priorityRank)
        {
            var windowStart = DateTime.Today.AddHours(17);
            var windows = (int)Math.Floor((readyAtLocal - windowStart).TotalMinutes / Math.Max(1, _rules.BatchWindowMinutes));
            var waveNumber = Math.Max(1, windows + priorityRank);
            return "Wave " + Math.Min(waveNumber, 4);
        }

        private string GetPriorityLabel(DispatchTicket ticket)
        {
            if (ticket.RequiresPairing)
            {
                return "Counter Hold";
            }

            return ticket.PriorityScore >= 85 ? "Rush" : "Standard";
        }
    }
}
