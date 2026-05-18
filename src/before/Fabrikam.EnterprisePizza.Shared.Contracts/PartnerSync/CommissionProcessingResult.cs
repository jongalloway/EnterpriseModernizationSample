using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [DataContract(Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    [XmlType(TypeName = "CommissionProcessingResult", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class CommissionProcessingResult
    {
        [DataMember(Order = 1)]
        public string PartnerId { get; set; }

        [DataMember(Order = 2)]
        public string SettlementBatchId { get; set; }

        [DataMember(Order = 3)]
        public decimal CommissionAmount { get; set; }

        [DataMember(Order = 4)]
        public string Status { get; set; }

        [DataMember(Order = 5)]
        public DateTime ProcessedAtUtc { get; set; }

        [DataMember(Order = 6)]
        public string ProcessedBy { get; set; }
    }
}
