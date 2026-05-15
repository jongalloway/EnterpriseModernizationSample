using System;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [XmlType(TypeName = "PartnerSyncRequest", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class PartnerSyncRequest
    {
        public string RequestedBy { get; set; }

        public string ChannelCode { get; set; }

        public bool IncludeCommunityPartners { get; set; }
    }
}
