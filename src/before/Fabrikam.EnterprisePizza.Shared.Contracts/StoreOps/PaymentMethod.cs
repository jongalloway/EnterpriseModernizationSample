using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public enum PaymentMethod
    {
        [EnumMember]
        Card = 0,

        [EnumMember]
        Cash = 1,

        [EnumMember]
        HouseAccount = 2
    }
}
