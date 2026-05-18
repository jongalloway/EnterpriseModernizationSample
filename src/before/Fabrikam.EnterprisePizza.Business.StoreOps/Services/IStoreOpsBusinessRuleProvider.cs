using System;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IStoreOpsBusinessRuleProvider
    {
        decimal GetTaxRate(string storeNumber);

        decimal GetMinimumOrder(string storeNumber);

        bool IsDeliveryZoneAllowed(string deliveryZone);

        bool IsWithinStoreHours(DateTime requestedFulfillmentTimeLocal);

        bool TryGetConfiguredPrice(string storeNumber, string menuItemCode, out decimal unitPrice);

        decimal GetVolumeDiscountRate(int totalQuantity);

        PromoDefinition GetPromo(string promoCode);
    }

    public enum PromoDiscountType
    {
        None = 0,
        Percentage = 1,
        FixedAmount = 2
    }

    public class PromoDefinition
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public PromoDiscountType DiscountType { get; set; }

        public decimal Value { get; set; }
    }
}
