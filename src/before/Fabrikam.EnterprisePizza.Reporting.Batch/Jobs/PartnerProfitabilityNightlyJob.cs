using System;
using System.Globalization;
using System.Linq;
using Fabrikam.EnterprisePizza.Reporting.Batch.Analytics;
using Fabrikam.EnterprisePizza.Reporting.Batch.Logging;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Jobs
{
    public class PartnerProfitabilityNightlyJob : ILegacyBatchJob
    {
        private static readonly CultureInfo CurrencyCulture = CultureInfo.GetCultureInfo("en-US");
        private readonly ChargebackProcessor chargebackProcessor;
        private readonly CommissionCalculator commissionCalculator;
        private readonly LegacyBatchLogger logger;
        private readonly PartnerRevenueAggregator revenueAggregator;

        public PartnerProfitabilityNightlyJob(LegacyBatchLogger logger, PartnerRevenueAggregator revenueAggregator, CommissionCalculator commissionCalculator, ChargebackProcessor chargebackProcessor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.revenueAggregator = revenueAggregator ?? throw new ArgumentNullException(nameof(revenueAggregator));
            this.commissionCalculator = commissionCalculator ?? throw new ArgumentNullException(nameof(commissionCalculator));
            this.chargebackProcessor = chargebackProcessor ?? throw new ArgumentNullException(nameof(chargebackProcessor));
        }

        public string JobName
        {
            get { return "PartnerProfitabilityReports"; }
        }

        public BatchJobExecutionResult Execute(BatchJobDefinition definition, DateTime processDate)
        {
            var startedUtc = DateTime.UtcNow;

            try
            {
                var reports = revenueAggregator.BuildReports(processDate);
                commissionCalculator.Apply(reports, processDate);
                chargebackProcessor.Apply(reports);

                var totalRevenue = reports.Sum(report => report.Revenue.GrossRevenue);
                var totalCommission = reports.Sum(report => report.Commission.GrossCommission);
                var totalChargebackExposure = reports.Sum(report => report.Chargebacks.OutstandingExposure);
                var result = new BatchJobExecutionResult
                {
                    JobName = definition.Name,
                    StartedUtc = startedUtc,
                    CompletedUtc = DateTime.UtcNow,
                    Succeeded = true,
                    ReportsGenerated = reports.Count,
                    SqlScriptsPrepared = 3,
                    SummaryMessage = string.Format(
                        CultureInfo.InvariantCulture,
                        "Prepared {0} partner profitability scorecards covering {1}, {2} in commissions, and {3} in net chargeback exposure.",
                        reports.Count,
                        totalRevenue.ToString("C", CurrencyCulture),
                        totalCommission.ToString("C", CurrencyCulture),
                        totalChargebackExposure.ToString("C", CurrencyCulture))
                };

                logger.Info("Partner profitability job produced {0} scorecards for {1:yyyy-MM-dd}.", reports.Count, processDate);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error("Partner profitability job failed: {0}", ex.Message);
                return new BatchJobExecutionResult
                {
                    JobName = definition.Name,
                    StartedUtc = startedUtc,
                    CompletedUtc = DateTime.UtcNow,
                    Succeeded = false,
                    SummaryMessage = ex.Message
                };
            }
        }
    }
}
