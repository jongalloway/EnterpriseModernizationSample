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
    public class DriverAssignmentServiceFixture
    {
        [Test]
        public void RecommendAssignments_prefers_primary_zone_driver_when_capacity_exists()
        {
            var service = new DriverAssignmentService(new DispatchBusinessRules(new NameValueCollection()));
            var ticket = CreateTicket(601, string.Empty, "Northwest Corporate Corridor", 82);
            var drivers = service.GetAvailableDrivers("014", new List<DispatchTicket>());

            var recommendations = service.RecommendAssignments("014", new List<DispatchTicket> { ticket }, drivers);

            Assert.That(recommendations.Single().DriverCode, Is.EqualTo("DRV-17"));
            Assert.That(recommendations.Single().ZoneMatch, Is.EqualTo("Primary Zone"));
        }

        [Test]
        public void GetAvailableDrivers_marks_driver_unavailable_when_maximum_delivery_count_is_reached()
        {
            var rules = new DispatchBusinessRules(new NameValueCollection
            {
                ["storeOps:dispatch:maxDeliveriesPerDriver"] = "1"
            });
            var service = new DriverAssignmentService(rules);
            var tickets = new List<DispatchTicket>
            {
                CreateTicket(602, "DRV-17", "Northwest Corporate Corridor", 75)
            };

            var drivers = service.GetAvailableDrivers("014", tickets);
            var driver = drivers.Single(item => item.DriverCode == "DRV-17");

            Assert.That(driver.IsAvailable, Is.False);
            Assert.That(driver.AvailableCapacity, Is.EqualTo(0));
        }

        private static DispatchTicket CreateTicket(int ticketId, string driverCode, string routeZone, int priority)
        {
            var readyAt = DateTime.Today.AddHours(17).AddMinutes(10);
            return new DispatchTicket
            {
                TicketId = ticketId,
                StoreNumber = "014",
                DriverCode = driverCode,
                RouteZone = routeZone,
                CustomerName = "Customer " + ticketId,
                ReadyAtLocal = readyAt,
                PromiseTimeLocal = readyAt.AddMinutes(20),
                RouteDistanceMiles = 5.0m,
                EstimatedTravelMinutes = 16,
                Status = DeliveryStatus.Assigned,
                PriorityScore = priority
            };
        }
    }
}
