using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Configuration;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class DispatchQueueServiceFixture
    {
        [Test]
        public void BuildDispatchQueue_orders_entries_by_priority_and_hold_status()
        {
            var service = new DispatchQueueService(new DispatchBusinessRules(new NameValueCollection()));
            var queue = service.BuildDispatchQueue(
                "014",
                new List<DispatchTicket>
                {
                    CreateTicket(801, "Mall Annex", 75, false),
                    CreateTicket(802, "Northwest Corporate Corridor", 92, true)
                },
                new List<RoutePlan>
                {
                    new RoutePlan { RouteZone = "Mall Annex", DispatchWave = "Wave 2" },
                    new RoutePlan { RouteZone = "Northwest Corporate Corridor", DispatchWave = "Wave 1" }
                },
                new List<DriverAssignmentRecommendation>
                {
                    new DriverAssignmentRecommendation { TicketId = 801, DriverCode = "DRV-03" },
                    new DriverAssignmentRecommendation { TicketId = 802, DriverCode = "DRV-17" }
                });

            Assert.That(queue.Select(entry => entry.TicketId), Is.EqualTo(new[] { 802, 801 }));
            Assert.That(queue.First().QueueStatus, Is.EqualTo("Hold for Pair"));
        }

        [Test]
        public void BuildDispatchBatches_honors_configured_max_batch_size()
        {
            var service = new DispatchQueueService(new DispatchBusinessRules(new NameValueCollection
            {
                ["storeOps:dispatch:maxBatchSize"] = "1"
            }));
            var batches = service.BuildDispatchBatches(
                "014",
                new List<DispatchQueueEntry>
                {
                    new DispatchQueueEntry { TicketId = 901, SuggestedDriverCode = "DRV-17", DispatchWave = "Wave 1", RouteZone = "Northwest Corporate Corridor", QueueStatus = "Ready to Send", EstimatedEtaMinutes = 16, BatchKey = "DRV-17|Wave 1|Northwest Corporate Corridor" },
                    new DispatchQueueEntry { TicketId = 902, SuggestedDriverCode = "DRV-17", DispatchWave = "Wave 1", RouteZone = "Northwest Corporate Corridor", QueueStatus = "Ready to Send", EstimatedEtaMinutes = 18, BatchKey = "DRV-17|Wave 1|Northwest Corporate Corridor" }
                });

            Assert.That(batches, Has.Count.EqualTo(2));
            Assert.That(batches.All(batch => batch.TotalTickets == 1), Is.True);
        }

        private static DispatchTicket CreateTicket(int ticketId, string routeZone, int priority, bool requiresPairing)
        {
            return new DispatchTicket
            {
                TicketId = ticketId,
                StoreNumber = "014",
                RouteZone = routeZone,
                ReadyAtLocal = DateTime.Today.AddHours(17).AddMinutes(10),
                PromiseTimeLocal = DateTime.Today.AddHours(17).AddMinutes(30),
                EstimatedTravelMinutes = 16,
                RequiresPairing = requiresPairing,
                PriorityScore = priority,
                Status = DeliveryStatus.ReadyForDispatch
            };
        }
    }
}
