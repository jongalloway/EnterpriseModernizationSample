using System;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class OrderStatusServiceFixture
    {
        [Test]
        public void ChangeStatus_only_allows_single_step_progression()
        {
            var service = new OrderStatusService(new FixedBusinessClock());
            var order = new OrderProcessingResult
            {
                Status = OrderStatus.Pending,
                SubmittedAtLocal = new DateTime(2026, 5, 18, 18, 0, 0)
            };

            var rejected = service.ChangeStatus(order, OrderStatus.Preparing, "dispatch");
            var accepted = service.ChangeStatus(order, OrderStatus.Confirmed, "dispatch");

            Assert.That(rejected.Accepted, Is.False);
            Assert.That(accepted.Accepted, Is.True);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Confirmed));
            Assert.That(order.StatusHistory, Has.Some.Contains("dispatch"));
        }

        private sealed class FixedBusinessClock : IBusinessClock
        {
            public DateTime GetCurrentTime()
            {
                return new DateTime(2026, 5, 18, 18, 5, 0);
            }
        }
    }
}
