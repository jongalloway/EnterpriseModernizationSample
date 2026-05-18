using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class PartnerContractRequest
    {
        [DataMember(Order = 1)]
        public string PartnerCode { get; set; }

        [DataMember(Order = 2)]
        public string PartnerName { get; set; }

        [DataMember(Order = 3)]
        public string AccountCode { get; set; }

        [DataMember(Order = 4)]
        public DateTime EffectiveDateUtc { get; set; }

        [DataMember(Order = 5)]
        public int TermMonths { get; set; }

        [DataMember(Order = 6)]
        public string ReferralChannelCode { get; set; }

        [DataMember(Order = 7)]
        public decimal MinimumMonthlyCommitment { get; set; }

        [DataMember(Order = 8)]
        public string PricingPlan { get; set; }
    }
}
