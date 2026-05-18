using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class PartnerContractTerminationRequest
    {
        [DataMember(Order = 1)]
        public string ContractNumber { get; set; }

        [DataMember(Order = 2)]
        public DateTime TerminatedDateUtc { get; set; }

        [DataMember(Order = 3)]
        public string Reason { get; set; }
    }
}
