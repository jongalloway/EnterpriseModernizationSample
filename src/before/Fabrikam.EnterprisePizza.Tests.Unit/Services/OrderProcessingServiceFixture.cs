using System;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class OrderProcessingServiceFixture
    {
        [Test]
        public void CreateOrder_returns_pending_order_with_pricing_and_cart()
        {
            var clock = new FixedBusinessClock();
            var rules = new StoreOpsBusinessRuleProvider();
            var pricing = new OrderPricingEngine(rules);
            var service = new OrderProcessingService(
                new OrderValidationService(rules),
                pricing,
                new OrderStatusService(clock),
                new PosWorkflowService(pricing, new StubPaymentGateway(clock), clock, new StoreOpsExceptionShield()),
                clock,
                new StoreOpsExceptionShield());

            var order = service.CreateOrder(new OrderRequest
            {
                StoreNumber = "031",
                CustomerName = "Downtown Co-Op",
                ServiceMode = "Delivery",
                Channel = "POS",
                DeliveryZone = "NORTH",
                RequestedFulfillmentTimeLocal = new DateTime(2026, 5, 18, 18, 30, 0),
                PromoCode = "FAMILY5",
                CashierId = "LANE-02",
                PaymentMethod = PaymentMethod.Card,
                LineItems =
                {
                    new OrderLineItem { MenuItemCode = "WINGS-12", Quantity = 2 },
                    new OrderLineItem { MenuItemCode = "SODA-2L", Quantity = 1 }
                }
            });

            Assert.That(order.Accepted, Is.True);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Pending));
            Assert.That(order.Pricing.Total, Is.GreaterThan(0m));
            Assert.That(order.Cart, Is.Not.Null);
            Assert.That(order.Cart.CashierId, Is.EqualTo("LANE-02"));
            Assert.That(order.StatusHistory, Has.Some.Contains("Pending"));
        }

        private sealed class FixedBusinessClock : IBusinessClock
        {
            public DateTime GetCurrentTime()
            {
                return new DateTime(2026, 5, 18, 17, 45, 0);
            }
        }
    }
}
