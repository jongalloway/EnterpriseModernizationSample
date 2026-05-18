using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderSearchRequest
    {
        [DataMember]
        public string StoreNumber { get; set; }

        [DataMember]
        public string SearchText { get; set; }

        [DataMember]
        public string StatusCode { get; set; }

        [DataMember]
        public DateTime FromSubmittedUtc { get; set; }

        [DataMember]
        public DateTime ToSubmittedUtc { get; set; }

        [DataMember]
        public bool IncludeClosed { get; set; }
    }
}
