using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.Routing
{
    [DataContract]
    public class DispatchBoardSnapshot
    {
        public DispatchBoardSnapshot()
        {
            Tickets = new List<DispatchTicket>();
        }

        [DataMember(Order = 1)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 2)]
        public DateTime GeneratedAtUtc { get; set; }

        [DataMember(Order = 3)]
        public string SourceSystem { get; set; }

        [DataMember(Order = 4)]
        public List<DispatchTicket> Tickets { get; set; }
    }
}
