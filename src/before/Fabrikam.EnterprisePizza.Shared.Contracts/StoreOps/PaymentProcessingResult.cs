using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class PaymentProcessingResult
    {
        [DataMember(Order = 1)]
        public bool Approved { get; set; }

        [DataMember(Order = 2)]
        public string ApprovalCode { get; set; }

        [DataMember(Order = 3)]
        public string StatusMessage { get; set; }

        [DataMember(Order = 4)]
        public decimal AmountAuthorized { get; set; }

        [DataMember(Order = 5)]
        public DateTime ProcessedAtLocal { get; set; }

        [DataMember(Order = 6)]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
