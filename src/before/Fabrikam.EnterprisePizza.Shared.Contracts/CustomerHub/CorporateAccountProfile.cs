using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class CorporateAccountProfile
    {
        public CorporateAccountProfile()
        {
            Locations = new List<CorporateLocationRecord>();
        }

        [DataMember(Order = 1)]
        public string AccountCode { get; set; }

        [DataMember(Order = 2)]
        public string AccountName { get; set; }

        [DataMember(Order = 3)]
        public string WorkflowStatus { get; set; }

        [DataMember(Order = 4)]
        public string RelationshipTier { get; set; }

        [DataMember(Order = 5)]
        public int PaymentTermsDays { get; set; }

        [DataMember(Order = 6)]
        public decimal CreditLimit { get; set; }

        [DataMember(Order = 7)]
        public decimal VolumeDiscountPercent { get; set; }

        [DataMember(Order = 8)]
        public string PrimaryBillingContact { get; set; }

        [DataMember(Order = 9)]
        public string LeadReferralChannelCode { get; set; }

        [DataMember(Order = 10)]
        public List<CorporateLocationRecord> Locations { get; set; }

        [DataMember(Order = 11)]
        public string AccountManager { get; set; }

        [DataMember(Order = 12)]
        public DateTime NextReviewDateUtc { get; set; }
    }
}
