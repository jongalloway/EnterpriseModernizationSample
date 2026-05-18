using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class PartnerContractRenewalRequest
    {
        [DataMember(Order = 1)]
        public string ContractNumber { get; set; }

        [DataMember(Order = 2)]
        public DateTime RenewalDateUtc { get; set; }

        [DataMember(Order = 3)]
        public int RenewalTermMonths { get; set; }
    }
}
