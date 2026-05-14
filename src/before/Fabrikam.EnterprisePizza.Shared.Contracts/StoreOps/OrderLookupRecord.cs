using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderLookupRecord
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
        public string PromiseWindow { get; set; }

        [DataMember]
        public decimal TicketTotal { get; set; }

        [DataMember]
        public string KitchenStatus { get; set; }

        [DataMember]
        public string DispatchStatus { get; set; }

        [DataMember]
        public string PaymentStatus { get; set; }

        public string TicketTotalDisplay
        {
            get { return TicketTotal.ToString("C"); }
        }
    }
}
