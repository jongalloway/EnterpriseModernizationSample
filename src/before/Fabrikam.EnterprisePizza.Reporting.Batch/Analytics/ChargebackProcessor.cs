using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Analytics
{
    public class ChargebackProcessor
    {
        public void Apply(IList<PartnerFinancialReport> reports)
        {
            if (reports == null)
            {
                throw new ArgumentNullException(nameof(reports));
            }

            foreach (var report in reports)
            {
                var chargebackCount = GetChargebackCount(report);
                var disputedCount = Math.Max(0, chargebackCount - 1);
                var grossChargebackAmount = Math.Round(report.Revenue.GrossRevenue * GetChargebackRatio(report.Partner.RelationshipTier), 2, MidpointRounding.AwayFromZero);
                var recoveredAmount = Math.Round(grossChargebackAmount * GetRecoveryRatio(report.Partner.RelationshipTier), 2, MidpointRounding.AwayFromZero);
                var outstandingExposure = grossChargebackAmount - recoveredAmount;

                report.Chargebacks = new ChargebackSummary
                {
                    ChargebackCount = chargebackCount,
                    DisputedCount = disputedCount,
                    ChargebackRatePercentage = report.Volume.DeliveredOrders == 0
                        ? 0m
                        : Math.Round((chargebackCount * 100m) / report.Volume.DeliveredOrders, 2, MidpointRounding.AwayFromZero),
                    DisputeRatePercentage = chargebackCount == 0
                        ? 0m
                        : Math.Round((disputedCount * 100m) / chargebackCount, 2, MidpointRounding.AwayFromZero),
                    GrossChargebackAmount = grossChargebackAmount,
                    RecoveredAmount = recoveredAmount,
                    OutstandingExposure = outstandingExposure
                };
                report.NetRevenueImpact = new NetRevenueImpactSummary
                {
                    GrossRevenue = report.Revenue.GrossRevenue,
                    CommissionExpense = report.Commission == null ? 0m : report.Commission.GrossCommission,
                    ChargebackExpense = grossChargebackAmount,
                    RecoveryAmount = recoveredAmount,
                    NetRevenueAfterAdjustments = report.Revenue.GrossRevenue
                        - (report.Commission == null ? 0m : report.Commission.GrossCommission)
                        - outstandingExposure
                };
            }
        }

        private static int GetChargebackCount(PartnerFinancialReport report)
        {
            switch (report.Partner.RelationshipTier)
            {
                case "Gold":
                    return 2 + (report.Volume.DeliveredOrders / 60);
                case "Community":
                    return 1 + (report.Volume.DeliveredOrders / 70);
                default:
                    return 2 + (report.Volume.DeliveredOrders / 35);
            }
        }

        private static decimal GetChargebackRatio(string relationshipTier)
        {
            switch (relationshipTier)
            {
                case "Gold":
                    return 0.018m;
                case "Community":
                    return 0.014m;
                default:
                    return 0.029m;
            }
        }

        private static decimal GetRecoveryRatio(string relationshipTier)
        {
            switch (relationshipTier)
            {
                case "Gold":
                    return 0.46m;
                case "Community":
                    return 0.58m;
                default:
                    return 0.33m;
            }
        }
    }
}
