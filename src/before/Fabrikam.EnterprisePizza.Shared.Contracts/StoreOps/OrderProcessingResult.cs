using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderProcessingResult
    {
        public OrderProcessingResult()
        {
            Validation = new OrderValidationResult();
            Pricing = new OrderPricingResult();
            StatusHistory = new List<string>();
        }

        [DataMember(Order = 1)]
        public bool Accepted { get; set; }

        [DataMember(Order = 2)]
        public int OrderNumber { get; set; }

        [DataMember(Order = 3)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 4)]
        public string CustomerName { get; set; }

        [DataMember(Order = 5)]
        public string ServiceMode { get; set; }

        [DataMember(Order = 6)]
        public string PromoCode { get; set; }

        [DataMember(Order = 7)]
        public OrderStatus Status { get; set; }

        [DataMember(Order = 8)]
        public DateTime SubmittedAtLocal { get; set; }

        [DataMember(Order = 9)]
        public OrderValidationResult Validation { get; set; }

        [DataMember(Order = 10)]
        public OrderPricingResult Pricing { get; set; }

        [DataMember(Order = 11)]
        public PosCart Cart { get; set; }

        [DataMember(Order = 12)]
        public PosReceipt Receipt { get; set; }

        [DataMember(Order = 13)]
        public List<string> StatusHistory { get; set; }
    }
}
