using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub
{
    [DataContract]
    public class CorporateLocationRecord
    {
        [DataMember(Order = 1)]
        public string LocationCode { get; set; }

        [DataMember(Order = 2)]
        public string LocationName { get; set; }

        [DataMember(Order = 3)]
        public string City { get; set; }

        [DataMember(Order = 4)]
        public string ServiceWindow { get; set; }

        [DataMember(Order = 5)]
        public bool ParticipatesInVolumePricing { get; set; }
    }
}
