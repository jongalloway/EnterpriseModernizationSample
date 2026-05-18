using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class DispatchTicket
    {
        [DataMember(Order = 1)]
        public int TicketId { get; set; }

        [DataMember(Order = 2)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 3)]
        public string DriverCode { get; set; }

        [DataMember(Order = 4)]
        public string RouteZone { get; set; }

        [DataMember(Order = 5)]
        public string CustomerName { get; set; }

        [DataMember(Order = 6)]
        public string DeliveryAddress { get; set; }

        [DataMember(Order = 7)]
        public DateTime ReadyAtLocal { get; set; }

        [DataMember(Order = 8)]
        public DateTime PromiseTimeLocal { get; set; }

        [DataMember(Order = 9)]
        public decimal RouteDistanceMiles { get; set; }

        [DataMember(Order = 10)]
        public int EstimatedTravelMinutes { get; set; }

        [DataMember(Order = 11)]
        public bool RequiresPairing { get; set; }

        [DataMember(Order = 12)]
        public DeliveryStatus Status { get; set; }

        [DataMember(Order = 13)]
        public int PriorityScore { get; set; }
    }
}
