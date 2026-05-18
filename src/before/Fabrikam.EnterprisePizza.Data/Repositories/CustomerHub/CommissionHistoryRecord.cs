using System;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class CommissionHistoryRecord
    {
        public int ReferralCommissionHistoryId { get; set; }

        public int PartnerReferralId { get; set; }

        public DateTime CommissionPeriodStart { get; set; }

        public DateTime CommissionPeriodEnd { get; set; }

        public decimal CommissionAmount { get; set; }

        public string CommissionStatus { get; set; }

        public DateTime? PaidUtc { get; set; }
    }
}
