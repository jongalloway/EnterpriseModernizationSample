using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class DeliveryTrackingRecord
    {
        public DeliveryTrackingRecord()
        {
            History = new List<DeliveryStatusEvent>();
        }

        [DataMember(Order = 1)]
        public int TicketId { get; set; }

        [DataMember(Order = 2)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 3)]
        public string DriverCode { get; set; }

        [DataMember(Order = 4)]
        public DeliveryStatus CurrentStatus { get; set; }

        [DataMember(Order = 5)]
        public DateTime EstimatedArrivalLocal { get; set; }

        [DataMember(Order = 6)]
        public DateTime? DispatchedAtLocal { get; set; }

        [DataMember(Order = 7)]
        public DateTime? DeliveredAtLocal { get; set; }

        [DataMember(Order = 8)]
        public DateTime LastUpdatedAtLocal { get; set; }

        [DataMember(Order = 9)]
        public string CompletionNote { get; set; }

        [DataMember(Order = 10)]
        public List<DeliveryStatusEvent> History { get; set; }
    }
}
