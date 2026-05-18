using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [DataContract(Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    [XmlType(TypeName = "PartnerRegistrationRequest", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class PartnerRegistrationRequest
    {
        [DataMember(Order = 1)]
        public string PartnerName { get; set; }

        [DataMember(Order = 2)]
        public string AccountCode { get; set; }

        [DataMember(Order = 3)]
        public string RelationshipTier { get; set; }

        [DataMember(Order = 4)]
        public string PrimaryContact { get; set; }

        [DataMember(Order = 5)]
        public string RequestedBy { get; set; }
    }
}
