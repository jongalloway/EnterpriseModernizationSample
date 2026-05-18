using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderLineItem
    {
        [DataMember(Order = 1)]
        public string MenuItemCode { get; set; }

        [DataMember(Order = 2)]
        public string Description { get; set; }

        [DataMember(Order = 3)]
        public int Quantity { get; set; }

        [DataMember(Order = 4)]
        public decimal UnitPrice { get; set; }
    }
}
