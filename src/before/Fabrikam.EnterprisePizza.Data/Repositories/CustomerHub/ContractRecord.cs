using System;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class ContractRecord
    {
        public int PartnerContractId { get; set; }

        public string ContractCode { get; set; }

        public int PartnerAccountId { get; set; }

        public int? CorporateAccountId { get; set; }

        public int? FranchiseLocationId { get; set; }

        public string ContractType { get; set; }

        public string PricingScheduleName { get; set; }

        public string ReferralChannel { get; set; }

        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public decimal MinimumOrderAmount { get; set; }

        public decimal DiscountPercentage { get; set; }

        public short CateringLeadHours { get; set; }

        public string StatusCode { get; set; }

        public DateTime LastReviewedUtc { get; set; }
    }
}
