using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class RoutePlan
    {
        public RoutePlan()
        {
            Stops = new List<RouteStop>();
        }

        [DataMember(Order = 1)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 2)]
        public string RouteZone { get; set; }

        [DataMember(Order = 3)]
        public string ZoneBoundary { get; set; }

        [DataMember(Order = 4)]
        public string DispatchWave { get; set; }

        [DataMember(Order = 5)]
        public int DriverCount { get; set; }

        [DataMember(Order = 6)]
        public decimal TotalDistanceMiles { get; set; }

        [DataMember(Order = 7)]
        public int EstimatedTravelMinutes { get; set; }

        [DataMember(Order = 8)]
        public DateTime ReadyWindowStartLocal { get; set; }

        [DataMember(Order = 9)]
        public DateTime ReadyWindowEndLocal { get; set; }

        [DataMember(Order = 10)]
        public string PlannerNote { get; set; }

        [DataMember(Order = 11)]
        public List<RouteStop> Stops { get; set; }
    }
}
