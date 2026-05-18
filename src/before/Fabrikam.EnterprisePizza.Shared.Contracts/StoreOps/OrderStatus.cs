using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public enum OrderStatus
    {
        [EnumMember]
        Pending = 0,

        [EnumMember]
        Confirmed = 1,

        [EnumMember]
        Preparing = 2,

        [EnumMember]
        Ready = 3,

        [EnumMember]
        Dispatched = 4,

        [EnumMember]
        Delivered = 5
    }
}
