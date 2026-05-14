using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class DispatchTicket
    {
        [DataMember]
        public int TicketId { get; set; }

        [DataMember]
        public string StoreNumber { get; set; }

        [DataMember]
        public string DriverCode { get; set; }

        [DataMember]
        public string RouteZone { get; set; }
    }
}
