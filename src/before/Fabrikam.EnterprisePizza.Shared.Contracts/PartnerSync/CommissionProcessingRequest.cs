using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync
{
    [Serializable]
    [DataContract(Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    [XmlType(TypeName = "CommissionProcessingRequest", Namespace = "http://fabrikam.com/pizza/partners/2006/09")]
    public class CommissionProcessingRequest
    {
        [DataMember(Order = 1)]
        public string PartnerId { get; set; }

        [DataMember(Order = 2)]
        public string SettlementBatchId { get; set; }

        [DataMember(Order = 3)]
        public decimal GrossSalesAmount { get; set; }

        [DataMember(Order = 4)]
        public decimal CommissionRatePercent { get; set; }

        [DataMember(Order = 5)]
        public string RequestedBy { get; set; }
    }
}
