using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class DispatchQueueEntry
    {
        [DataMember(Order = 1)]
        public int TicketId { get; set; }

        [DataMember(Order = 2)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 3)]
        public string RouteZone { get; set; }

        [DataMember(Order = 4)]
        public string DispatchWave { get; set; }

        [DataMember(Order = 5)]
        public int PriorityRank { get; set; }

        [DataMember(Order = 6)]
        public string PriorityLabel { get; set; }

        [DataMember(Order = 7)]
        public string SuggestedDriverCode { get; set; }

        [DataMember(Order = 8)]
        public string QueueStatus { get; set; }

        [DataMember(Order = 9)]
        public string HoldReason { get; set; }

        [DataMember(Order = 10)]
        public int EstimatedEtaMinutes { get; set; }

        [DataMember(Order = 11)]
        public string BatchKey { get; set; }
    }
}
