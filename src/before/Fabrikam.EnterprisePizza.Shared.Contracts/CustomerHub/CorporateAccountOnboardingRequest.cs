using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class CorporateAccountOnboardingRequest
    {
        public CorporateAccountOnboardingRequest()
        {
            LocationNames = new List<string>();
        }

        [DataMember(Order = 1)]
        public string AccountCode { get; set; }

        [DataMember(Order = 2)]
        public string AccountName { get; set; }

        [DataMember(Order = 3)]
        public decimal AverageMonthlyVolume { get; set; }

        [DataMember(Order = 4)]
        public bool RequiresPurchaseOrder { get; set; }

        [DataMember(Order = 5)]
        public string PrimaryBillingContact { get; set; }

        [DataMember(Order = 6)]
        public List<string> LocationNames { get; set; }

        [DataMember(Order = 7)]
        public string LeadReferralChannelCode { get; set; }
    }
}
