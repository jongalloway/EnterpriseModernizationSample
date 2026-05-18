using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderHistoryRecord
    {
        [DataMember]
        public DateTime LoggedAtUtc { get; set; }

        [DataMember]
        public string StatusCode { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public string UpdatedBy { get; set; }

        [DataMember]
        public string SourceSystem { get; set; }
    }
}
