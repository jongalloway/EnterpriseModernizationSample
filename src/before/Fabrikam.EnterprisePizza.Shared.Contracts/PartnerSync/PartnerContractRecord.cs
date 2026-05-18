using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [DataContract(Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    [XmlType(TypeName = "PartnerContractRecord", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class PartnerContractRecord
    {
        [DataMember(Order = 1)]
        public string PartnerId { get; set; }

        [DataMember(Order = 2)]
        public string ContractCode { get; set; }

        [DataMember(Order = 3)]
        public string PricingPlanCode { get; set; }

        [DataMember(Order = 4)]
        public DateTime EffectiveDateUtc { get; set; }

        [DataMember(Order = 5)]
        public DateTime ExpirationDateUtc { get; set; }

        [DataMember(Order = 6)]
        public bool IsAutoRenew { get; set; }
    }
}
