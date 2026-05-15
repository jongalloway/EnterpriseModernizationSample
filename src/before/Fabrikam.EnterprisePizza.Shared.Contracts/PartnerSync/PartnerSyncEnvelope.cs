using System;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [XmlRoot("PartnerSyncEnvelope", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class PartnerSyncEnvelope
    {
        public PartnerSyncEnvelope()
        {
            Partners = Array.Empty<PartnerAccountSummary>();
        }

        public string SourceSystem { get; set; }

        public DateTime GeneratedAtUtc { get; set; }

        [XmlArray("PreferredPartners")]
        [XmlArrayItem("Partner")]
        public PartnerAccountSummary[] Partners { get; set; }
    }
}
