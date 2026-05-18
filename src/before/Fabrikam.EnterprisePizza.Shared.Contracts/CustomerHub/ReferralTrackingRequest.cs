using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class ReferralTrackingRequest
    {
        [DataMember(Order = 1)]
        public string PartnerCode { get; set; }

        [DataMember(Order = 2)]
        public string ReferredAccountCode { get; set; }

        [DataMember(Order = 3)]
        public string ReferredAccountName { get; set; }

        [DataMember(Order = 4)]
        public string ReferralChannelCode { get; set; }

        [DataMember(Order = 5)]
        public decimal OrderSubtotal { get; set; }

        [DataMember(Order = 6)]
        public DateTime FirstOrderDateUtc { get; set; }

        [DataMember(Order = 7)]
        public int LocationCount { get; set; }
    }
}
