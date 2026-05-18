using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [DataContract(Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    [XmlType(TypeName = "PartnerStatusRecord", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class PartnerStatusRecord
    {
        [DataMember(Order = 1)]
        public string PartnerId { get; set; }

        [DataMember(Order = 2)]
        public string AccountCode { get; set; }

        [DataMember(Order = 3)]
        public string Status { get; set; }

        [DataMember(Order = 4)]
        public string ContractCode { get; set; }

        [DataMember(Order = 5)]
        public DateTime StatusUpdatedAtUtc { get; set; }
    }
}
