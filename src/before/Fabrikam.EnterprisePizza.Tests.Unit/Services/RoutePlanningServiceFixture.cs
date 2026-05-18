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
    public class RoutePlanningServiceFixture
    {
        [Test]
        public void BuildRoutePlans_groups_tickets_by_zone_and_calculates_totals()
        {
            var rules = new DispatchBusinessRules(new NameValueCollection
            {
                ["storeOps:dispatch:maxBatchSize"] = "2"
            });
            var service = new RoutePlanningService(rules);
            var tickets = new List<DispatchTicket>
            {
                CreateTicket(501, "Northwest Corporate Corridor", 6.0m, 18, 90, false),
                CreateTicket(502, "Northwest Corporate Corridor", 3.6m, 12, 70, true),
                CreateTicket(503, "Mall Annex", 4.2m, 14, 80, false)
            };

            var routePlans = service.BuildRoutePlans("014", tickets);

            Assert.That(routePlans, Has.Count.EqualTo(2));
            var northwestPlan = routePlans.Single(plan => plan.RouteZone == "Northwest Corporate Corridor");
            Assert.That(northwestPlan.Stops, Has.Count.EqualTo(2));
            Assert.That(northwestPlan.TotalDistanceMiles, Is.EqualTo(9.6m));
            Assert.That(northwestPlan.DriverCount, Is.EqualTo(1));
            Assert.That(northwestPlan.PlannerNote, Does.Contain("counter-hold"));
        }

        private static DispatchTicket CreateTicket(int ticketId, string routeZone, decimal miles, int travelMinutes, int priority, bool requiresPairing)
        {
            var readyAt = DateTime.Today.AddHours(17).AddMinutes(ticketId % 10);
            return new DispatchTicket
            {
                TicketId = ticketId,
                StoreNumber = "014",
                DriverCode = string.Empty,
                RouteZone = routeZone,
                CustomerName = "Customer " + ticketId,
                ReadyAtLocal = readyAt,
                PromiseTimeLocal = readyAt.AddMinutes(20),
                RouteDistanceMiles = miles,
                EstimatedTravelMinutes = travelMinutes,
                RequiresPairing = requiresPairing,
                Status = DeliveryStatus.ReadyForDispatch,
                PriorityScore = priority
            };
        }
    }
}
