using System;
using System.Collections.Specialized;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class OrderValidationServiceFixture
    {
        [Test]
        public void Validate_flags_orders_that_violate_minimum_delivery_rules()
        {
            var settings = new NameValueCollection
            {
                ["StoreOps.BusinessRules.MinimumOrder.Default"] = "20.00",
                ["StoreOps.BusinessRules.DeliveryZones"] = "NORTH|SOUTH",
                ["StoreOps.BusinessRules.StoreHours.Open"] = "11",
                ["StoreOps.BusinessRules.StoreHours.Close"] = "22"
            };
            var service = new OrderValidationService(new StoreOpsBusinessRuleProvider(settings));

            var result = service.Validate(new OrderRequest
            {
                StoreNumber = "014",
                CustomerName = "Jon's Office",
                ServiceMode = "Delivery",
                DeliveryZone = "REMOTE",
                RequestedFulfillmentTimeLocal = new DateTime(2026, 5, 18, 23, 15, 0),
                LineItems = { new OrderLineItem { MenuItemCode = "SODA-2L", Quantity = 1 } }
            });

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Issues, Has.Some.Matches<OrderValidationIssue>(issue => issue.Code == "delivery-zone"));
            Assert.That(result.Issues, Has.Some.Matches<OrderValidationIssue>(issue => issue.Code == "minimum-order"));
            Assert.That(result.Issues, Has.Some.Matches<OrderValidationIssue>(issue => issue.Code == "store-hours"));
        }
    }
}
