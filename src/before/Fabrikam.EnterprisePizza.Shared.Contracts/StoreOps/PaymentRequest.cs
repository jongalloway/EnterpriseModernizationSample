using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class PaymentRequest
    {
        [DataMember(Order = 1)]
        public PaymentMethod Method { get; set; }

        [DataMember(Order = 2)]
        public decimal AmountTendered { get; set; }

        [DataMember(Order = 3)]
        public string AuthorizationToken { get; set; }

        [DataMember(Order = 4)]
        public string Last4 { get; set; }

        [DataMember(Order = 5)]
        public string CashierId { get; set; }
    }
}
