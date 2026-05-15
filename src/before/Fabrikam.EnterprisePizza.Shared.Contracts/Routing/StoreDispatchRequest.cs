using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class StoreDispatchRequest
    {
        [DataMember(Order = 1)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 2)]
        public string DispatchTerminalId { get; set; }

        [DataMember(Order = 3)]
        public bool IncludeDriverNotes { get; set; }
    }
}
