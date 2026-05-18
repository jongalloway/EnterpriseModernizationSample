using System;
using System.Collections.Generic;
using System.Globalization;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Models
{
    public sealed class DeliveryPerformanceReportParameters : IReportParameterModel
    {
        public DeliveryPerformanceReportParameters()
        {
            StoreNumber = "014";
            SummaryDate = DateTime.UtcNow.Date;
            MinimumCompletedRuns = 1;
        }

        public string StoreNumber { get; set; }

        public DateTime SummaryDate { get; set; }

        public int MinimumCompletedRuns { get; set; }

        public IEnumerable<ReportParameterValue> ToReportParameters()
        {
            if (string.IsNullOrWhiteSpace(StoreNumber))
            {
                throw new InvalidOperationException("Store number is required for the delivery performance report.");
            }

            if (MinimumCompletedRuns < 0)
            {
                throw new InvalidOperationException("Minimum completed runs cannot be negative.");
            }

            return new[]
            {
                new ReportParameterValue("StoreNumber", StoreNumber.Trim()),
                new ReportParameterValue("SummaryDate", SummaryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new ReportParameterValue("MinimumCompletedRuns", MinimumCompletedRuns.ToString(CultureInfo.InvariantCulture))
            };
        }
    }

    public sealed class StoreOperationsSummaryReportParameters : IReportParameterModel
    {
        public StoreOperationsSummaryReportParameters()
        {
            var today = DateTime.UtcNow.Date;
            StoreNumber = "014";
            StartDate = today.AddDays(-6);
            EndDate = today;
            RollupMode = "Daily";
        }

        public string StoreNumber { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string RollupMode { get; set; }

        public IEnumerable<ReportParameterValue> ToReportParameters()
        {
            if (string.IsNullOrWhiteSpace(StoreNumber))
            {
                throw new InvalidOperationException("Store number is required for the store operations summary report.");
            }

            if (EndDate.Date < StartDate.Date)
            {
                throw new InvalidOperationException("End date must be on or after the start date.");
            }

            var normalizedRollupMode = string.IsNullOrWhiteSpace(RollupMode) ? string.Empty : RollupMode.Trim();
            if (!normalizedRollupMode.Equals("Daily", StringComparison.OrdinalIgnoreCase)
                && !normalizedRollupMode.Equals("Weekly", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Rollup mode must be Daily or Weekly.");
            }

            return new[]
            {
                new ReportParameterValue("StoreNumber", StoreNumber.Trim()),
                new ReportParameterValue("StartDate", StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new ReportParameterValue("EndDate", EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new ReportParameterValue("RollupMode", normalizedRollupMode)
            };
        }
    }

    public sealed class PartnerProfitabilityReportParameters : IReportParameterModel
    {
        public PartnerProfitabilityReportParameters()
        {
            SummaryDate = DateTime.UtcNow.Date;
            PartnerCode = "ALL";
            MinimumGrossSales = 0m;
        }

        public DateTime SummaryDate { get; set; }

        public string PartnerCode { get; set; }

        public decimal MinimumGrossSales { get; set; }

        public IEnumerable<ReportParameterValue> ToReportParameters()
        {
            if (MinimumGrossSales < 0m)
            {
                throw new InvalidOperationException("Minimum gross sales cannot be negative.");
            }

            var normalizedPartnerCode = string.IsNullOrWhiteSpace(PartnerCode) ? "ALL" : PartnerCode.Trim().ToUpperInvariant();
            return new[]
            {
                new ReportParameterValue("SummaryDate", SummaryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new ReportParameterValue("PartnerCode", normalizedPartnerCode),
                new ReportParameterValue("MinimumGrossSales", MinimumGrossSales.ToString("0.00", CultureInfo.InvariantCulture))
            };
        }
    }
}
