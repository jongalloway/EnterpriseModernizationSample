using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderRequest
    {
        public OrderRequest()
        {
            LineItems = new List<OrderLineItem>();
        }

        [DataMember(Order = 1)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 2)]
        public string CustomerName { get; set; }

        [DataMember(Order = 3)]
        public string ServiceMode { get; set; }

        [DataMember(Order = 4)]
        public string Channel { get; set; }

        [DataMember(Order = 5)]
        public string DeliveryZone { get; set; }

        [DataMember(Order = 6)]
        public string PromoCode { get; set; }

        [DataMember(Order = 7)]
        public DateTime RequestedFulfillmentTimeLocal { get; set; }

        [DataMember(Order = 8)]
        public PaymentMethod PaymentMethod { get; set; }

        [DataMember(Order = 9)]
        public string CashierId { get; set; }

        [DataMember(Order = 10)]
        public List<OrderLineItem> LineItems { get; set; }
    }
}
