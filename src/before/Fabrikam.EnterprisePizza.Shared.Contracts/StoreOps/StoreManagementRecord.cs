using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class StoreManagementRecord
    {
        [DataMember]
        public string StoreNumber { get; set; }

        [DataMember]
        public string StoreName { get; set; }

        [DataMember]
        public string District { get; set; }

        [DataMember]
        public string DispatchTerminalId { get; set; }

        [DataMember]
        public string ManagerOnDuty { get; set; }

        [DataMember]
        public string BoardMode { get; set; }

        [DataMember]
        public int OpenOrderCount { get; set; }

        [DataMember]
        public int DriverCount { get; set; }

        [DataMember]
        public string LastSyncTime { get; set; }

        [DataMember]
        public string StoreStatus { get; set; }

        [DataMember]
        public string EscalationNote { get; set; }
    }
}
