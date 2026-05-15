using System;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [XmlType(TypeName = "PartnerAccountSummary", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class PartnerAccountSummary
    {
        public string PartnerName { get; set; }

        public string AccountCode { get; set; }

        public string RelationshipTier { get; set; }

        public bool ExportEnabled { get; set; }
    }
}
