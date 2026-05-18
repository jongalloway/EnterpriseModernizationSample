using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class DriverAvailability
    {
        [DataMember(Order = 1)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 2)]
        public string DriverCode { get; set; }

        [DataMember(Order = 3)]
        public string HomeZone { get; set; }

        [DataMember(Order = 4)]
        public string BoundaryCode { get; set; }

        [DataMember(Order = 5)]
        public int CurrentActiveDeliveries { get; set; }

        [DataMember(Order = 6)]
        public int MaxDeliveries { get; set; }

        [DataMember(Order = 7)]
        public int AvailableCapacity { get; set; }

        [DataMember(Order = 8)]
        public bool IsAvailable { get; set; }

        [DataMember(Order = 9)]
        public DateTime NextAvailableAtLocal { get; set; }

        [DataMember(Order = 10)]
        public string DispatchWave { get; set; }
    }
}
