using System;
using System.Collections.Generic;
using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Configuration;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class DispatchQueueService : StoreOpsServiceBase, IDispatchQueueService
    {
        private readonly DispatchBusinessRules _rules;

        public DispatchQueueService()
            : this(new DispatchBusinessRules())
        {
        }

        public DispatchQueueService(DispatchBusinessRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public IList<DispatchQueueEntry> BuildDispatchQueue(string storeNumber, IList<DispatchTicket> tickets, IList<RoutePlan> routePlans, IList<DriverAssignmentRecommendation> assignments)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                var routePlanLookup = (routePlans ?? new List<RoutePlan>())
                    .ToDictionary(plan => plan.RouteZone ?? string.Empty, StringComparer.OrdinalIgnoreCase);
                var assignmentLookup = (assignments ?? new List<DriverAssignmentRecommendation>())
                    .ToDictionary(assignment => assignment.TicketId);

                return (tickets ?? new List<DispatchTicket>())
                    .Where(ticket => ticket != null)
                    .Select(ticket => CreateQueueEntry(normalizedStoreNumber, ticket, routePlanLookup, assignmentLookup))
                    .OrderByDescending(entry => entry.PriorityRank)
                    .ThenBy(entry => entry.QueueStatus == "Manual Review" ? 1 : 0)
                    .ThenBy(entry => entry.DispatchWave)
                    .ThenBy(entry => entry.RouteZone)
                    .ThenBy(entry => entry.TicketId)
                    .ToList();
            }, "DispatchQueueService.BuildDispatchQueue");
        }

        public IList<DispatchBatch> BuildDispatchBatches(string storeNumber, IList<DispatchQueueEntry> queueEntries)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                var batches = new List<DispatchBatch>();
                var readyEntries = (queueEntries ?? new List<DispatchQueueEntry>())
                    .Where(entry => entry != null && entry.QueueStatus != "Manual Review")
                    .GroupBy(entry => entry.BatchKey ?? string.Empty)
                    .OrderBy(group => group.Key)
                    .ToList();

                var batchNumber = 1;
                foreach (var group in readyEntries)
                {
                    var groupedEntries = group.OrderByDescending(entry => entry.PriorityRank).ThenBy(entry => entry.TicketId).ToList();
                    for (var index = 0; index < groupedEntries.Count; index += _rules.MaxBatchSize)
                    {
                        var slice = groupedEntries.Skip(index).Take(_rules.MaxBatchSize).ToList();
                        var firstEntry = slice[0];
                        batches.Add(new DispatchBatch
                        {
                            StoreNumber = normalizedStoreNumber,
                            BatchNumber = batchNumber++,
                            DriverCode = firstEntry.SuggestedDriverCode,
                            RouteZone = firstEntry.RouteZone,
                            DispatchWave = firstEntry.DispatchWave,
                            TotalTickets = slice.Count,
                            EstimatedDepartureLocal = DateTime.Today.AddHours(17).AddMinutes(10 + (batchNumber * 2)),
                            EstimatedCompletionLocal = DateTime.Today.AddHours(17).AddMinutes(20 + slice.Max(entry => entry.EstimatedEtaMinutes)),
                            TicketIds = slice.Select(entry => entry.TicketId).ToList(),
                            DispatcherNote = slice.Any(entry => entry.QueueStatus == "Hold for Pair")
                                ? "Release the full pair together to protect the promised window."
                                : "Batch is balanced for the current dispatch wave."
                        });
                    }
                }

                return batches;
            }, "DispatchQueueService.BuildDispatchBatches");
        }

        private DispatchQueueEntry CreateQueueEntry(string storeNumber, DispatchTicket ticket, IDictionary<string, RoutePlan> routePlanLookup, IDictionary<int, DriverAssignmentRecommendation> assignmentLookup)
        {
            RoutePlan routePlan;
            DriverAssignmentRecommendation assignment;
            routePlanLookup.TryGetValue(ticket.RouteZone ?? string.Empty, out routePlan);
            assignmentLookup.TryGetValue(ticket.TicketId, out assignment);

            var priorityRank = ticket.PriorityScore <= 0 ? 50 : ticket.PriorityScore;
            var queueStatus = assignment == null || string.IsNullOrWhiteSpace(assignment.DriverCode)
                ? "Manual Review"
                : ticket.RequiresPairing
                    ? "Hold for Pair"
                    : "Ready to Send";

            return new DispatchQueueEntry
            {
                TicketId = ticket.TicketId,
                StoreNumber = storeNumber,
                RouteZone = ticket.RouteZone,
                DispatchWave = routePlan == null ? "Wave 1" : routePlan.DispatchWave,
                PriorityRank = priorityRank,
                PriorityLabel = ticket.RequiresPairing
                    ? "Counter Hold"
                    : priorityRank >= 85 ? "Rush" : "Standard",
                SuggestedDriverCode = assignment == null ? string.Empty : assignment.DriverCode,
                QueueStatus = queueStatus,
                HoldReason = queueStatus == "Hold for Pair"
                    ? "Ticket should leave with the rest of its paired wave."
                    : queueStatus == "Manual Review"
                        ? "Dispatch supervisor review required."
                        : string.Empty,
                EstimatedEtaMinutes = ticket.EstimatedTravelMinutes > 0 ? ticket.EstimatedTravelMinutes : 18,
                BatchKey = (assignment == null ? "UNASSIGNED" : assignment.DriverCode) + "|" + (routePlan == null ? "Wave 1" : routePlan.DispatchWave) + "|" + (ticket.RouteZone ?? string.Empty)
            };
        }

        private static string NormalizeStoreNumber(string storeNumber)
        {
            return string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
        }
    }
}
