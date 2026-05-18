using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class AccountTierClassification
    {
        [DataMember(Order = 1)]
        public string AccountCode { get; set; }

        [DataMember(Order = 2)]
        public string RelationshipTier { get; set; }

        [DataMember(Order = 3)]
        public decimal AverageMonthlyVolume { get; set; }

        [DataMember(Order = 4)]
        public int LocationCount { get; set; }

        [DataMember(Order = 5)]
        public string ReviewNote { get; set; }
    }
}
