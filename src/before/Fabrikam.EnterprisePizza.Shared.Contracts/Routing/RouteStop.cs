using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class RouteStop
    {
        [DataMember(Order = 1)]
        public int TicketId { get; set; }

        [DataMember(Order = 2)]
        public string CustomerName { get; set; }

        [DataMember(Order = 3)]
        public string RouteZone { get; set; }

        [DataMember(Order = 4)]
        public int SequenceNumber { get; set; }

        [DataMember(Order = 5)]
        public decimal DistanceMiles { get; set; }

        [DataMember(Order = 6)]
        public int EstimatedTravelMinutes { get; set; }

        [DataMember(Order = 7)]
        public DateTime ReadyAtLocal { get; set; }

        [DataMember(Order = 8)]
        public DateTime PromiseTimeLocal { get; set; }

        [DataMember(Order = 9)]
        public DateTime EstimatedArrivalLocal { get; set; }

        [DataMember(Order = 10)]
        public string PriorityLabel { get; set; }

        [DataMember(Order = 11)]
        public string DriverCode { get; set; }
    }
}
