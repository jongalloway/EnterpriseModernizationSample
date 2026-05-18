using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class PosCart
    {
        public PosCart()
        {
            Items = new List<OrderLineItem>();
        }

        [DataMember(Order = 1)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 2)]
        public string CashierId { get; set; }

        [DataMember(Order = 3)]
        public string CustomerName { get; set; }

        [DataMember(Order = 4)]
        public string ServiceMode { get; set; }

        [DataMember(Order = 5)]
        public string PromoCode { get; set; }

        [DataMember(Order = 6)]
        public string DeliveryZone { get; set; }

        [DataMember(Order = 7)]
        public DateTime OpenedAtLocal { get; set; }

        [DataMember(Order = 8)]
        public PaymentMethod PaymentMethod { get; set; }

        [DataMember(Order = 9)]
        public List<OrderLineItem> Items { get; set; }

        [DataMember(Order = 10)]
        public decimal EstimatedSubtotal { get; set; }

        [DataMember(Order = 11)]
        public decimal EstimatedTotal { get; set; }
    }
}
