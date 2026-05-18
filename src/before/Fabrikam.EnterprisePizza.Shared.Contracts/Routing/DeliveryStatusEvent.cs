using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class DeliveryStatusEvent
    {
        [DataMember(Order = 1)]
        public DeliveryStatus Status { get; set; }

        [DataMember(Order = 2)]
        public DateTime ChangedAtLocal { get; set; }

        [DataMember(Order = 3)]
        public string ChangedBy { get; set; }

        [DataMember(Order = 4)]
        public string Note { get; set; }
    }
}
