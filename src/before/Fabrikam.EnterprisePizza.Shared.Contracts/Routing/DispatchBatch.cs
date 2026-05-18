using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class DispatchBatch
    {
        public DispatchBatch()
        {
            TicketIds = new List<int>();
        }

        [DataMember(Order = 1)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 2)]
        public int BatchNumber { get; set; }

        [DataMember(Order = 3)]
        public string DriverCode { get; set; }

        [DataMember(Order = 4)]
        public string RouteZone { get; set; }

        [DataMember(Order = 5)]
        public string DispatchWave { get; set; }

        [DataMember(Order = 6)]
        public int TotalTickets { get; set; }

        [DataMember(Order = 7)]
        public DateTime EstimatedDepartureLocal { get; set; }

        [DataMember(Order = 8)]
        public DateTime EstimatedCompletionLocal { get; set; }

        [DataMember(Order = 9)]
        public List<int> TicketIds { get; set; }

        [DataMember(Order = 10)]
        public string DispatcherNote { get; set; }
    }
}
