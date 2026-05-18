using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class CreditTermsDecision
    {
        [DataMember(Order = 1)]
        public string AccountCode { get; set; }

        [DataMember(Order = 2)]
        public string RelationshipTier { get; set; }

        [DataMember(Order = 3)]
        public int PaymentTermsDays { get; set; }

        [DataMember(Order = 4)]
        public decimal CreditLimit { get; set; }

        [DataMember(Order = 5)]
        public bool RequiresManualReview { get; set; }

        [DataMember(Order = 6)]
        public string ReviewReason { get; set; }
    }
}
