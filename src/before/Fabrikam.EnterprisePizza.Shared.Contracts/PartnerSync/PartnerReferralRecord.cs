using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [DataContract(Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    [XmlType(TypeName = "PartnerReferralRecord", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class PartnerReferralRecord
    {
        [DataMember(Order = 1)]
        public string ReferralId { get; set; }

        [DataMember(Order = 2)]
        public string PartnerId { get; set; }

        [DataMember(Order = 3)]
        public string ReferredAccountName { get; set; }

        [DataMember(Order = 4)]
        public string ReferralChannel { get; set; }

        [DataMember(Order = 5)]
        public string ReferralStatus { get; set; }

        [DataMember(Order = 6)]
        public string SubmittedBy { get; set; }

        [DataMember(Order = 7)]
        public DateTime SubmittedAtUtc { get; set; }
    }
}
