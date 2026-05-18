using System;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class ReferralRecord
    {
        public int PartnerReferralId { get; set; }

        public string ReferralCode { get; set; }

        public int PartnerAccountId { get; set; }

        public int? CorporateAccountId { get; set; }

        public string ReferralChannel { get; set; }

        public string ReferrerName { get; set; }

        public string AttributionCode { get; set; }

        public DateTime ReferredOn { get; set; }

        public string StatusCode { get; set; }

        public string Notes { get; set; }
    }
}
