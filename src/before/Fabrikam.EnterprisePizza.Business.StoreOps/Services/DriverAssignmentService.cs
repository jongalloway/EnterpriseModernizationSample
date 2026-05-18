using System;
using System.Collections.Generic;
using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Configuration;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class DriverAssignmentService : StoreOpsServiceBase, IDriverAssignmentService
    {
        private readonly DispatchBusinessRules _rules;

        public DriverAssignmentService()
            : this(new DispatchBusinessRules())
        {
        }

        public DriverAssignmentService(DispatchBusinessRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public IList<DriverAvailability> GetAvailableDrivers(string storeNumber, IList<DispatchTicket> tickets)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                var activeTicketGroups = (tickets ?? new List<DispatchTicket>())
                    .Where(CountsAgainstLoad)
                    .GroupBy(ticket => ticket.DriverCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.OrdinalIgnoreCase);

                return _rules.GetDriverRoster(normalizedStoreNumber)
                    .Select(driver => CreateAvailability(normalizedStoreNumber, driver, activeTicketGroups))
                    .OrderByDescending(driver => driver.IsAvailable)
                    .ThenBy(driver => driver.CurrentActiveDeliveries)
                    .ThenBy(driver => driver.NextAvailableAtLocal)
                    .ThenBy(driver => driver.DriverCode)
                    .ToList();
            }, "DriverAssignmentService.GetAvailableDrivers");
        }

        public IList<DriverAssignmentRecommendation> RecommendAssignments(string storeNumber, IList<DispatchTicket> tickets, IList<DriverAvailability> drivers)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                var projectedDrivers = (drivers ?? new List<DriverAvailability>())
                    .Select(CloneDriver)
                    .ToList();
                var recommendations = new List<DriverAssignmentRecommendation>();
                var orderedTickets = (tickets ?? new List<DispatchTicket>())
                    .Where(ticket => ticket != null)
                    .OrderByDescending(ticket => ticket.PriorityScore)
                    .ThenBy(ticket => ticket.PromiseTimeLocal == default(DateTime) ? DateTime.Today.AddHours(17).AddMinutes(30) : ticket.PromiseTimeLocal)
                    .ThenBy(ticket => ticket.TicketId)
                    .ToList();

                foreach (var ticket in orderedTickets)
                {
                    var preferredDriver = FindExistingDriver(projectedDrivers, ticket.DriverCode);
                    var selectedDriver = preferredDriver ?? ChooseDriver(ticket, projectedDrivers);
                    if (selectedDriver == null)
                    {
                        recommendations.Add(new DriverAssignmentRecommendation
                        {
                            TicketId = ticket.TicketId,
                            StoreNumber = normalizedStoreNumber,
                            DriverCode = string.Empty,
                            RouteZone = ticket.RouteZone,
                            ZoneMatch = "Manual Review",
                            DeliveryCountAfterAssignment = 0,
                            EstimatedDepartureLocal = ticket.ReadyAtLocal,
                            EstimatedArrivalLocal = ticket.PromiseTimeLocal,
                            WorkloadScore = decimal.MaxValue,
                            RecommendationNote = "No available driver meets the zone and workload rules."
                        });
                        continue;
                    }

                    selectedDriver.CurrentActiveDeliveries += 1;
                    selectedDriver.AvailableCapacity = Math.Max(0, selectedDriver.MaxDeliveries - selectedDriver.CurrentActiveDeliveries);
                    selectedDriver.IsAvailable = selectedDriver.AvailableCapacity > 0;
                    selectedDriver.DispatchWave = BuildWave(selectedDriver.CurrentActiveDeliveries);
                    selectedDriver.NextAvailableAtLocal = (ticket.ReadyAtLocal == default(DateTime)
                        ? DateTime.Today.AddHours(17).AddMinutes(15)
                        : ticket.ReadyAtLocal).AddMinutes(Math.Max(12, ticket.EstimatedTravelMinutes));

                    recommendations.Add(new DriverAssignmentRecommendation
                    {
                        TicketId = ticket.TicketId,
                        StoreNumber = normalizedStoreNumber,
                        DriverCode = selectedDriver.DriverCode,
                        RouteZone = ticket.RouteZone,
                        ZoneMatch = DetermineZoneMatch(ticket.RouteZone, selectedDriver.HomeZone),
                        DeliveryCountAfterAssignment = selectedDriver.CurrentActiveDeliveries,
                        EstimatedDepartureLocal = ticket.ReadyAtLocal,
                        EstimatedArrivalLocal = (ticket.ReadyAtLocal == default(DateTime) ? DateTime.Today.AddHours(17).AddMinutes(20) : ticket.ReadyAtLocal)
                            .AddMinutes(Math.Max(12, ticket.EstimatedTravelMinutes)),
                        WorkloadScore = selectedDriver.CurrentActiveDeliveries + (selectedDriver.HomeZone.Equals(ticket.RouteZone, StringComparison.OrdinalIgnoreCase) ? 0m : 0.5m),
                        RecommendationNote = preferredDriver != null
                            ? "Keep the current staged driver to avoid rework on the board."
                            : "Best zone fit with the lightest projected workload."
                    });
                }

                return recommendations;
            }, "DriverAssignmentService.RecommendAssignments");
        }

        private DriverAvailability CreateAvailability(string storeNumber, DriverRosterEntry driver, IDictionary<string, List<DispatchTicket>> activeTicketGroups)
        {
            List<DispatchTicket> assignedTickets;
            activeTicketGroups.TryGetValue(driver.DriverCode, out assignedTickets);
            assignedTickets = assignedTickets ?? new List<DispatchTicket>();

            var currentDeliveries = assignedTickets.Count;
            var nextAvailable = assignedTickets.Count == 0
                ? driver.LastCompletedAtLocal
                : assignedTickets.Max(ticket => ticket.PromiseTimeLocal == default(DateTime)
                    ? driver.LastCompletedAtLocal.AddMinutes(20)
                    : ticket.PromiseTimeLocal);

            return new DriverAvailability
            {
                StoreNumber = storeNumber,
                DriverCode = driver.DriverCode,
                HomeZone = driver.HomeZone,
                BoundaryCode = driver.BoundaryCode,
                CurrentActiveDeliveries = currentDeliveries,
                MaxDeliveries = _rules.MaxDeliveriesPerDriver,
                AvailableCapacity = Math.Max(0, _rules.MaxDeliveriesPerDriver - currentDeliveries),
                IsAvailable = currentDeliveries < _rules.MaxDeliveriesPerDriver,
                NextAvailableAtLocal = nextAvailable,
                DispatchWave = BuildWave(currentDeliveries)
            };
        }

        private static DriverAvailability CloneDriver(DriverAvailability driver)
        {
            return new DriverAvailability
            {
                StoreNumber = driver.StoreNumber,
                DriverCode = driver.DriverCode,
                HomeZone = driver.HomeZone,
                BoundaryCode = driver.BoundaryCode,
                CurrentActiveDeliveries = driver.CurrentActiveDeliveries,
                MaxDeliveries = driver.MaxDeliveries,
                AvailableCapacity = driver.AvailableCapacity,
                IsAvailable = driver.IsAvailable,
                NextAvailableAtLocal = driver.NextAvailableAtLocal,
                DispatchWave = driver.DispatchWave
            };
        }

        private DriverAvailability ChooseDriver(DispatchTicket ticket, IList<DriverAvailability> drivers)
        {
            return drivers
                .Where(driver => driver.IsAvailable)
                .OrderBy(driver => DetermineZoneMatch(ticket.RouteZone, driver.HomeZone) == "Primary Zone" ? 0 : 1)
                .ThenBy(driver => driver.CurrentActiveDeliveries)
                .ThenBy(driver => driver.NextAvailableAtLocal)
                .ThenBy(driver => driver.DriverCode)
                .FirstOrDefault();
        }

        private static DriverAvailability FindExistingDriver(IList<DriverAvailability> drivers, string driverCode)
        {
            if (string.IsNullOrWhiteSpace(driverCode))
            {
                return null;
            }

            return drivers.FirstOrDefault(driver => string.Equals(driver.DriverCode, driverCode, StringComparison.OrdinalIgnoreCase));
        }

        private static bool CountsAgainstLoad(DispatchTicket ticket)
        {
            return ticket != null
                && ticket.Status != DeliveryStatus.Delivered
                && ticket.Status != DeliveryStatus.Closed
                && ticket.Status != DeliveryStatus.DeliveryException;
        }

        private static string DetermineZoneMatch(string routeZone, string homeZone)
        {
            return string.Equals(routeZone, homeZone, StringComparison.OrdinalIgnoreCase)
                ? "Primary Zone"
                : "Cross-Zone";
        }

        private static string NormalizeStoreNumber(string storeNumber)
        {
            return string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
        }

        private static string BuildWave(int currentDeliveries)
        {
            return "Wave " + Math.Min(currentDeliveries + 1, 4);
        }
    }
}
