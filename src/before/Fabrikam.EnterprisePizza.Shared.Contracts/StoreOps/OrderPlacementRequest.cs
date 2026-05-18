using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderPlacementRequest
    {
        [DataMember]
        public string StoreNumber { get; set; }

        [DataMember]
        public string CustomerName { get; set; }

        [DataMember]
        public string Channel { get; set; }

        [DataMember]
        public string ServiceMode { get; set; }

        [DataMember]
        public decimal TicketTotal { get; set; }

        [DataMember]
        public bool IsCorporateAccount { get; set; }

        [DataMember]
        public decimal DeliveryMileage { get; set; }

        [DataMember]
        public DateTime NeededByUtc { get; set; }

        [DataMember]
        public string DeliveryAddress { get; set; }

        [DataMember]
        public string SpecialInstructions { get; set; }
    }
}
