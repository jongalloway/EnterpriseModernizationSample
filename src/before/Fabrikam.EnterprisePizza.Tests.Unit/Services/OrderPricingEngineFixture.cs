using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class OrderPricingEngineFixture
    {
        [Test]
        public void Calculate_applies_franchise_pricing_volume_discount_and_promo()
        {
            var engine = new OrderPricingEngine(new StoreOpsBusinessRuleProvider());

            var pricing = engine.Calculate(new OrderRequest
            {
                StoreNumber = "014",
                CustomerName = "Campus Catering",
                ServiceMode = "Carryout",
                PromoCode = "TUESDAY10",
                LineItems =
                {
                    new OrderLineItem { MenuItemCode = "PIZZA-LG", Quantity = 3 }
                }
            });

            Assert.That(pricing.Subtotal, Is.EqualTo(43.47m));
            Assert.That(pricing.VolumeDiscount, Is.EqualTo(2.17m));
            Assert.That(pricing.PromoDiscount, Is.EqualTo(4.13m));
            Assert.That(pricing.TaxAmount, Is.EqualTo(3.07m));
            Assert.That(pricing.Total, Is.EqualTo(40.24m));
            Assert.That(pricing.AppliedDiscounts, Has.Count.EqualTo(2));
        }
    }
}
