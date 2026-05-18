using System;
using System.Linq;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class OrderPricingEngine : IOrderPricingEngine
    {
        private readonly IStoreOpsBusinessRuleProvider _businessRules;

        public OrderPricingEngine()
            : this(new StoreOpsBusinessRuleProvider())
        {
        }

        public OrderPricingEngine(IStoreOpsBusinessRuleProvider businessRules)
        {
            _businessRules = businessRules ?? throw new ArgumentNullException(nameof(businessRules));
        }

        public OrderPricingResult Calculate(OrderRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var pricing = new OrderPricingResult();
            var lineItems = request.LineItems ?? Enumerable.Empty<OrderLineItem>();
            pricing.Subtotal = RoundCurrency(lineItems.Where(item => item != null)
                .Sum(item => ResolveUnitPrice(request.StoreNumber, item) * item.Quantity));

            var totalQuantity = lineItems.Where(item => item != null).Sum(item => item.Quantity);
            var volumeDiscountRate = _businessRules.GetVolumeDiscountRate(totalQuantity);
            pricing.VolumeDiscount = RoundCurrency(pricing.Subtotal * volumeDiscountRate);
            if (pricing.VolumeDiscount > 0m)
            {
                pricing.AppliedDiscounts.Add(new AppliedDiscount
                {
                    Code = "VOLUME",
                    Description = string.Format("Volume discount for {0} items", totalQuantity),
                    Amount = pricing.VolumeDiscount
                });
            }

            var promo = _businessRules.GetPromo(request.PromoCode);
            var discountedSubtotal = pricing.Subtotal - pricing.VolumeDiscount;
            if (promo != null)
            {
                pricing.PromoDiscount = promo.DiscountType == PromoDiscountType.FixedAmount
                    ? RoundCurrency(Math.Min(discountedSubtotal, promo.Value))
                    : RoundCurrency(discountedSubtotal * promo.Value);

                pricing.AppliedDiscounts.Add(new AppliedDiscount
                {
                    Code = promo.Code,
                    Description = promo.Description,
                    Amount = pricing.PromoDiscount
                });
            }

            var taxableSubtotal = Math.Max(0m, discountedSubtotal - pricing.PromoDiscount);
            pricing.TaxRate = _businessRules.GetTaxRate(request.StoreNumber);
            pricing.TaxAmount = RoundCurrency(taxableSubtotal * pricing.TaxRate);
            pricing.Total = RoundCurrency(taxableSubtotal + pricing.TaxAmount);
            return pricing;
        }

        private decimal ResolveUnitPrice(string storeNumber, OrderLineItem item)
        {
            decimal configuredPrice;
            if (_businessRules.TryGetConfiguredPrice(storeNumber, item.MenuItemCode, out configuredPrice))
            {
                return configuredPrice;
            }

            if (item.UnitPrice <= 0m)
            {
                throw new InvalidOperationException(string.Format("Menu item {0} is missing a configured price.", item.MenuItemCode));
            }

            return item.UnitPrice;
        }

        private static decimal RoundCurrency(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
