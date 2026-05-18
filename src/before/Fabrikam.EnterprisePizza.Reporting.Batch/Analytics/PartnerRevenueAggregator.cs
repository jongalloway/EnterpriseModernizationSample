using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;
using Fabrikam.EnterprisePizza.Core.Domain.CustomerHub;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Analytics
{
    public class PartnerRevenueAggregator
    {
        private readonly IPartnerAccountService partnerAccountService;

        public PartnerRevenueAggregator()
            : this(new PartnerAccountService())
        {
        }

        public PartnerRevenueAggregator(IPartnerAccountService partnerAccountService)
        {
            this.partnerAccountService = partnerAccountService ?? throw new ArgumentNullException(nameof(partnerAccountService));
        }

        public IList<PartnerFinancialReport> BuildReports(DateTime summaryDate)
        {
            var reports = new List<PartnerFinancialReport>();
            var partners = partnerAccountService.GetPreferredPartnerSnapshots();
            var offset = 0;

            foreach (var partner in partners)
            {
                reports.Add(BuildReport(partner, summaryDate, offset));
                offset++;
            }

            return reports;
        }

        private static PartnerFinancialReport BuildReport(PartnerAccountSnapshot partner, DateTime summaryDate, int seedOffset)
        {
            var profile = GetProfile(partner.RelationshipTier, seedOffset);
            var grossRevenue = Math.Round(profile.DeliveredOrders * profile.AverageTicket, 2, MidpointRounding.AwayFromZero);
            var previousPeriodRevenue = Math.Round(grossRevenue * profile.PreviousPeriodFactor, 2, MidpointRounding.AwayFromZero);
            var growthPercentage = previousPeriodRevenue == 0m
                ? 0m
                : Math.Round(((grossRevenue - previousPeriodRevenue) / previousPeriodRevenue) * 100m, 2, MidpointRounding.AwayFromZero);
            var report = new PartnerFinancialReport
            {
                Partner = partner,
                SummaryDate = summaryDate,
                Revenue = new PartnerRevenueSummary
                {
                    GrossRevenue = grossRevenue,
                    PreviousPeriodRevenue = previousPeriodRevenue,
                    GrowthPercentage = growthPercentage
                },
                Volume = new PartnerVolumeMetrics
                {
                    DeliveredOrders = profile.DeliveredOrders,
                    CateringOrders = profile.CateringOrders,
                    AverageTicket = profile.AverageTicket,
                    AverageWeeklyOrders = Math.Round(profile.DeliveredOrders / 4m, 1, MidpointRounding.AwayFromZero),
                    LargestSingleOrderValue = profile.LargestSingleOrderValue,
                    RepeatOrderRate = profile.RepeatOrderRate
                }
            };

            for (var trendIndex = 0; trendIndex < profile.TrendMultipliers.Length; trendIndex++)
            {
                var multiplier = profile.TrendMultipliers[trendIndex];
                report.RevenueTrend.Add(new PartnerRevenueTrendPoint
                {
                    WeekEndingDate = summaryDate.AddDays((trendIndex - (profile.TrendMultipliers.Length - 1)) * 7),
                    RevenueAmount = Math.Round((grossRevenue / profile.TrendMultipliers.Length) * multiplier, 2, MidpointRounding.AwayFromZero),
                    DeliveredOrders = Math.Max(1, (int)Math.Round((profile.DeliveredOrders / (decimal)profile.TrendMultipliers.Length) * multiplier, MidpointRounding.AwayFromZero))
                });
            }

            return report;
        }

        private static RevenueProfile GetProfile(string relationshipTier, int seedOffset)
        {
            switch (relationshipTier)
            {
                case "Gold":
                    return new RevenueProfile(132 + seedOffset, 48.25m, 0.92m, new[] { 0.91m, 0.95m, 1.03m, 1.11m }, 18, 384.50m, 72.0m);
                case "Community":
                    return new RevenueProfile(84 + seedOffset, 37.40m, 0.89m, new[] { 0.87m, 0.94m, 1.01m, 1.08m }, 9, 226.00m, 64.5m);
                default:
                    return new RevenueProfile(51 + seedOffset, 56.75m, 0.85m, new[] { 0.76m, 0.88m, 0.98m, 1.18m }, 6, 472.25m, 41.0m);
            }
        }

        private sealed class RevenueProfile
        {
            public RevenueProfile(int deliveredOrders, decimal averageTicket, decimal previousPeriodFactor, decimal[] trendMultipliers, int cateringOrders, decimal largestSingleOrderValue, decimal repeatOrderRate)
            {
                DeliveredOrders = deliveredOrders;
                AverageTicket = averageTicket;
                PreviousPeriodFactor = previousPeriodFactor;
                TrendMultipliers = trendMultipliers;
                CateringOrders = cateringOrders;
                LargestSingleOrderValue = largestSingleOrderValue;
                RepeatOrderRate = repeatOrderRate;
            }

            public int DeliveredOrders { get; private set; }

            public decimal AverageTicket { get; private set; }

            public decimal PreviousPeriodFactor { get; private set; }

            public decimal[] TrendMultipliers { get; private set; }

            public int CateringOrders { get; private set; }

            public decimal LargestSingleOrderValue { get; private set; }

            public decimal RepeatOrderRate { get; private set; }
        }
    }
}
