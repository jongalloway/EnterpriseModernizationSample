using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class AppliedDiscount
    {
        [DataMember(Order = 1)]
        public string Code { get; set; }

        [DataMember(Order = 2)]
        public string Description { get; set; }

        [DataMember(Order = 3)]
        public decimal Amount { get; set; }
    }
}
