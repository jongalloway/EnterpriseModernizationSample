using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderStatusChangeResult
    {
        [DataMember(Order = 1)]
        public bool Accepted { get; set; }

        [DataMember(Order = 2)]
        public OrderStatus PreviousStatus { get; set; }

        [DataMember(Order = 3)]
        public OrderStatus CurrentStatus { get; set; }

        [DataMember(Order = 4)]
        public string Message { get; set; }
    }
}
