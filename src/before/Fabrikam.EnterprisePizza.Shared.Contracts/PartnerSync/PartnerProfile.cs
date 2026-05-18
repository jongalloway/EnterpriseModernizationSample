using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [DataContract(Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    [XmlType(TypeName = "PartnerProfile", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class PartnerProfile
    {
        [DataMember(Order = 1)]
        public string PartnerId { get; set; }

        [DataMember(Order = 2)]
        public string PartnerName { get; set; }

        [DataMember(Order = 3)]
        public string AccountCode { get; set; }

        [DataMember(Order = 4)]
        public string Status { get; set; }

        [DataMember(Order = 5)]
        public string RelationshipTier { get; set; }

        [DataMember(Order = 6)]
        public string ContractCode { get; set; }

        [DataMember(Order = 7)]
        public string PrimaryContact { get; set; }

        [DataMember(Order = 8)]
        public DateTime ActiveSinceUtc { get; set; }

        [DataMember(Order = 9)]
        public bool IsPreferred { get; set; }

        [DataMember(Order = 10)]
        public DateTime StatusUpdatedAtUtc { get; set; }
    }
}
