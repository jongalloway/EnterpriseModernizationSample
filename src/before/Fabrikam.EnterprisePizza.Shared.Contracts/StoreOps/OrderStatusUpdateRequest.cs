using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderStatusUpdateRequest
    {
        [DataMember]
        public int OrderNumber { get; set; }

        [DataMember]
        public string StoreNumber { get; set; }

        [DataMember]
        public string StatusCode { get; set; }

        [DataMember]
        public string UpdatedBy { get; set; }

        [DataMember]
        public string StatusNote { get; set; }
    }
}
