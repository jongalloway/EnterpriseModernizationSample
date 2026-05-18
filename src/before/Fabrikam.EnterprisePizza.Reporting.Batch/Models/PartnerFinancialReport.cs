using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Domain.CustomerHub;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Models
{
    public class PartnerFinancialReport
    {
        private readonly IList<PartnerRevenueTrendPoint> revenueTrend = new List<PartnerRevenueTrendPoint>();

        public PartnerAccountSnapshot Partner { get; set; }

        public DateTime SummaryDate { get; set; }

        public PartnerRevenueSummary Revenue { get; set; }

        public IList<PartnerRevenueTrendPoint> RevenueTrend
        {
            get { return revenueTrend; }
        }

        public PartnerVolumeMetrics Volume { get; set; }

        public CommissionPayoutSummary Commission { get; set; }

        public SettlementSummary Settlement { get; set; }

        public ChargebackSummary Chargebacks { get; set; }

        public NetRevenueImpactSummary NetRevenueImpact { get; set; }
    }

    public class PartnerRevenueSummary
    {
        public decimal GrossRevenue { get; set; }

        public decimal PreviousPeriodRevenue { get; set; }

        public decimal GrowthPercentage { get; set; }
    }

    public class PartnerRevenueTrendPoint
    {
        public DateTime WeekEndingDate { get; set; }

        public decimal RevenueAmount { get; set; }

        public int DeliveredOrders { get; set; }
    }

    public class PartnerVolumeMetrics
    {
        public int DeliveredOrders { get; set; }

        public int CateringOrders { get; set; }

        public decimal AverageTicket { get; set; }

        public decimal AverageWeeklyOrders { get; set; }

        public decimal LargestSingleOrderValue { get; set; }

        public decimal RepeatOrderRate { get; set; }
    }

    public class CommissionPayoutSummary
    {
        public string TierName { get; set; }

        public decimal BaseRatePercentage { get; set; }

        public decimal EffectiveRatePercentage { get; set; }

        public decimal GrossCommission { get; set; }

        public decimal ReserveWithheld { get; set; }

        public decimal NetCommissionPayout { get; set; }
    }

    public class SettlementSummary
    {
        public string SettlementReference { get; set; }

        public string SettlementWindow { get; set; }

        public DateTime EstimatedSettlementDate { get; set; }

        public decimal EstimatedSettlementAmount { get; set; }

        public bool ReadyForSettlement { get; set; }
    }

    public class ChargebackSummary
    {
        public int ChargebackCount { get; set; }

        public int DisputedCount { get; set; }

        public decimal ChargebackRatePercentage { get; set; }

        public decimal DisputeRatePercentage { get; set; }

        public decimal GrossChargebackAmount { get; set; }

        public decimal RecoveredAmount { get; set; }

        public decimal OutstandingExposure { get; set; }
    }

    public class NetRevenueImpactSummary
    {
        public decimal GrossRevenue { get; set; }

        public decimal CommissionExpense { get; set; }

        public decimal ChargebackExpense { get; set; }

        public decimal RecoveryAmount { get; set; }

        public decimal NetRevenueAfterAdjustments { get; set; }
    }
}
