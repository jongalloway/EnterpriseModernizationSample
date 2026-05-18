using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [DataContract(Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    [XmlType(TypeName = "PartnerReferralSubmission", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class PartnerReferralSubmission
    {
        [DataMember(Order = 1)]
        public string PartnerId { get; set; }

        [DataMember(Order = 2)]
        public string ReferredAccountName { get; set; }

        [DataMember(Order = 3)]
        public string ChannelCode { get; set; }

        [DataMember(Order = 4)]
        public string SubmittedBy { get; set; }
    }
}
