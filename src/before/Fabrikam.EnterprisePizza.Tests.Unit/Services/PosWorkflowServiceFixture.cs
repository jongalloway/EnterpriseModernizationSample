using System;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class PosWorkflowServiceFixture
    {
        [Test]
        public void ProcessPayment_and_generate_receipt_return_legacy_pos_outputs()
        {
            var clock = new FixedBusinessClock();
            var pricing = new OrderPricingEngine(new StoreOpsBusinessRuleProvider());
            var service = new PosWorkflowService(pricing, new StubPaymentGateway(clock), clock, new StoreOpsExceptionShield());

            var cart = service.OpenCart(new OrderRequest
            {
                StoreNumber = "014",
                CustomerName = "Walk-Up Guest",
                ServiceMode = "Carryout",
                RequestedFulfillmentTimeLocal = new DateTime(2026, 5, 18, 18, 20, 0),
                PaymentMethod = PaymentMethod.Cash,
                CashierId = "LANE-05",
                LineItems =
                {
                    new OrderLineItem { MenuItemCode = "PASTA-BAKE", Quantity = 1 },
                    new OrderLineItem { MenuItemCode = "BREADSTICKS", Quantity = 1 }
                }
            });
            var payment = service.ProcessPayment(cart, new PaymentRequest
            {
                Method = PaymentMethod.Cash,
                AmountTendered = cart.EstimatedTotal,
                CashierId = "LANE-05"
            });
            var receipt = service.GenerateReceipt(new OrderProcessingResult
            {
                OrderNumber = 72111,
                StoreNumber = cart.StoreNumber,
                CustomerName = cart.CustomerName,
                Status = OrderStatus.Confirmed,
                Pricing = pricing.Calculate(new OrderRequest
                {
                    StoreNumber = cart.StoreNumber,
                    CustomerName = cart.CustomerName,
                    ServiceMode = cart.ServiceMode,
                    RequestedFulfillmentTimeLocal = cart.OpenedAtLocal,
                    LineItems = cart.Items
                }),
                Cart = cart
            }, payment);

            Assert.That(payment.Approved, Is.True);
            Assert.That(payment.ApprovalCode, Does.StartWith("AUTH-"));
            Assert.That(receipt.ReceiptNumber, Is.EqualTo("014-72111"));
            Assert.That(receipt.Lines, Has.Some.Contains("Payment: Cash"));
        }

        private sealed class FixedBusinessClock : IBusinessClock
        {
            public DateTime GetCurrentTime()
            {
                return new DateTime(2026, 5, 18, 18, 25, 0);
            }
        }
    }
}
