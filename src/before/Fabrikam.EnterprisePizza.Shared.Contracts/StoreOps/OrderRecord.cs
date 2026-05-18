using System;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderRecord
    {
        [DataMember]
        public int OrderNumber { get; set; }

        [DataMember]
        public string StoreNumber { get; set; }

        [DataMember]
        public string CustomerName { get; set; }

        [DataMember]
        public string Channel { get; set; }

        [DataMember]
        public string ServiceMode { get; set; }

        [DataMember]
        public string OrderStatus { get; set; }

        [DataMember]
        public string KitchenStatus { get; set; }

        [DataMember]
        public string DispatchStatus { get; set; }

        [DataMember]
        public string PaymentStatus { get; set; }

        [DataMember]
        public decimal TicketTotal { get; set; }

        [DataMember]
        public DateTime SubmittedAtUtc { get; set; }

        [DataMember]
        public DateTime NeededByUtc { get; set; }

        [DataMember]
        public DateTime QuotedReadyTimeUtc { get; set; }

        [DataMember]
        public string FulfillmentLane { get; set; }

        [DataMember]
        public string DeliveryAddress { get; set; }

        [DataMember]
        public string SpecialInstructions { get; set; }

        [DataMember]
        public bool IsCorporateAccount { get; set; }

        [DataMember]
        public decimal DeliveryMileage { get; set; }

        [DataMember]
        public string CurrentDriverCode { get; set; }
    }
}
