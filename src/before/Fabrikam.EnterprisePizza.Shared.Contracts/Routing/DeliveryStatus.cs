using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public enum DeliveryStatus
    {
        [EnumMember]
        Routed = 0,

        [EnumMember]
        ReadyForDispatch = 1,

        [EnumMember]
        Assigned = 2,

        [EnumMember]
        Dispatched = 3,

        [EnumMember]
        EnRoute = 4,

        [EnumMember]
        Delivered = 5,

        [EnumMember]
        DeliveryException = 6,

        [EnumMember]
        Closed = 7
    }
}
