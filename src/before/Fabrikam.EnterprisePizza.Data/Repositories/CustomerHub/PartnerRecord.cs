using System;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class PartnerRecord
    {
        public int PartnerAccountId { get; set; }

        public string PartnerCode { get; set; }

        public string PartnerName { get; set; }

        public string RelationshipTier { get; set; }

        public string PreferredStoreNumber { get; set; }

        public string StatusCode { get; set; }

        public DateTime? LastContractRenewalDate { get; set; }

        public bool CreditHold { get; set; }
    }
}
