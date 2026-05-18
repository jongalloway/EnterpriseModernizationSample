using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class ReferralAttributionRecord
    {
        [DataMember(Order = 1)]
        public string ReferralId { get; set; }

        [DataMember(Order = 2)]
        public string PartnerCode { get; set; }

        [DataMember(Order = 3)]
        public string ReferredAccountCode { get; set; }

        [DataMember(Order = 4)]
        public string ReferredAccountName { get; set; }

        [DataMember(Order = 5)]
        public string ReferralChannelCode { get; set; }

        [DataMember(Order = 6)]
        public string AttributionStatus { get; set; }

        [DataMember(Order = 7)]
        public decimal CommissionRatePercent { get; set; }

        [DataMember(Order = 8)]
        public decimal CommissionAmount { get; set; }

        [DataMember(Order = 9)]
        public DateTime AttributedOnUtc { get; set; }

        [DataMember(Order = 10)]
        public string Notes { get; set; }
    }
}
