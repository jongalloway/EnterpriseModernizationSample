using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderPricingResult
    {
        public OrderPricingResult()
        {
            AppliedDiscounts = new List<AppliedDiscount>();
        }

        [DataMember(Order = 1)]
        public decimal Subtotal { get; set; }

        [DataMember(Order = 2)]
        public decimal VolumeDiscount { get; set; }

        [DataMember(Order = 3)]
        public decimal PromoDiscount { get; set; }

        [DataMember(Order = 4)]
        public decimal TaxRate { get; set; }

        [DataMember(Order = 5)]
        public decimal TaxAmount { get; set; }

        [DataMember(Order = 6)]
        public decimal Total { get; set; }

        [DataMember(Order = 7)]
        public List<AppliedDiscount> AppliedDiscounts { get; set; }
    }
}
