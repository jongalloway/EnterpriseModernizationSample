using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class PartnerContractRecord
    {
        [DataMember(Order = 1)]
        public string ContractNumber { get; set; }

        [DataMember(Order = 2)]
        public string PartnerCode { get; set; }

        [DataMember(Order = 3)]
        public string PartnerName { get; set; }

        [DataMember(Order = 4)]
        public string AccountCode { get; set; }

        [DataMember(Order = 5)]
        public string RelationshipTier { get; set; }

        [DataMember(Order = 6)]
        public string ContractStatus { get; set; }

        [DataMember(Order = 7)]
        public DateTime EffectiveDateUtc { get; set; }

        [DataMember(Order = 8)]
        public DateTime ExpirationDateUtc { get; set; }

        [DataMember(Order = 9)]
        public DateTime? LastRenewedDateUtc { get; set; }

        [DataMember(Order = 10)]
        public DateTime? TerminatedDateUtc { get; set; }

        [DataMember(Order = 11)]
        public string TerminationReason { get; set; }

        [DataMember(Order = 12)]
        public string ReferralChannelCode { get; set; }

        [DataMember(Order = 13)]
        public decimal CommissionRatePercent { get; set; }

        [DataMember(Order = 14)]
        public decimal MinimumMonthlyCommitment { get; set; }

        [DataMember(Order = 15)]
        public string PricingPlan { get; set; }
    }
}
