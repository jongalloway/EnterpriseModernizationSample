using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Analytics
{
    public class CommissionCalculator
    {
        public void Apply(IList<PartnerFinancialReport> reports, DateTime summaryDate)
        {
            if (reports == null)
            {
                throw new ArgumentNullException(nameof(reports));
            }

            foreach (var report in reports)
            {
                var baseRate = GetBaseRate(report.Partner.RelationshipTier);
                var acceleratorRate = report.Revenue.GrossRevenue >= GetAcceleratorThreshold(report.Partner.RelationshipTier) ? 0.0075m : 0m;
                var effectiveRate = baseRate + acceleratorRate;
                var grossCommission = Math.Round(report.Revenue.GrossRevenue * effectiveRate, 2, MidpointRounding.AwayFromZero);
                var reserveWithheld = Math.Round(grossCommission * GetReserveHoldback(report.Partner.RelationshipTier), 2, MidpointRounding.AwayFromZero);
                var netPayout = grossCommission - reserveWithheld;

                report.Commission = new CommissionPayoutSummary
                {
                    TierName = report.Partner.RelationshipTier,
                    BaseRatePercentage = baseRate * 100m,
                    EffectiveRatePercentage = effectiveRate * 100m,
                    GrossCommission = grossCommission,
                    ReserveWithheld = reserveWithheld,
                    NetCommissionPayout = netPayout
                };
                report.Settlement = new SettlementSummary
                {
                    SettlementReference = string.Format("SET-{0:yyyyMMdd}-{1}", summaryDate, report.Partner.PartnerCode),
                    SettlementWindow = "Nightly AP export",
                    EstimatedSettlementDate = summaryDate.AddDays(2),
                    EstimatedSettlementAmount = netPayout,
                    ReadyForSettlement = true
                };
            }
        }

        private static decimal GetBaseRate(string relationshipTier)
        {
            switch (relationshipTier)
            {
                case "Gold":
                    return 0.085m;
                case "Community":
                    return 0.050m;
                default:
                    return 0.110m;
            }
        }

        private static decimal GetReserveHoldback(string relationshipTier)
        {
            switch (relationshipTier)
            {
                case "Gold":
                    return 0.08m;
                case "Community":
                    return 0.05m;
                default:
                    return 0.12m;
            }
        }

        private static decimal GetAcceleratorThreshold(string relationshipTier)
        {
            switch (relationshipTier)
            {
                case "Gold":
                    return 5000m;
                case "Community":
                    return 3000m;
                default:
                    return 2500m;
            }
        }
    }
}
