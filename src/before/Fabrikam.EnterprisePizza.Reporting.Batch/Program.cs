using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Domain.Reporting;
using Fabrikam.EnterprisePizza.Reporting.Batch.Analytics;
using Fabrikam.EnterprisePizza.Reporting.Batch.Configuration;
using Fabrikam.EnterprisePizza.Reporting.Batch.Logging;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;
using Fabrikam.EnterprisePizza.Reporting.Batch.Services;
using Fabrikam.EnterprisePizza.Reporting.Batch.Configuration;
using Fabrikam.EnterprisePizza.Reporting.Batch.Execution;
using Fabrikam.EnterprisePizza.Reporting.Batch.Logging;

namespace Fabrikam.EnterprisePizza.Reporting.Batch
{
    internal static class Program
    {
        private static readonly CultureInfo DisplayCulture = CultureInfo.GetCultureInfo("en-US");

        private static int Main(string[] args)
        {
            if (args != null && args.Any(argument => string.Equals(argument, "/workforce-report", StringComparison.OrdinalIgnoreCase)))
            {
                RunWorkforceSnapshotReport();
                return 0;
            }

            if (args != null && args.Any(argument => string.Equals(argument, "/partner-profitability", StringComparison.OrdinalIgnoreCase)))
            {
                RunPartnerProfitabilityReport();
                return 0;
            }

            var scheduler = new BatchScheduler(new BatchSettingsProvider(), new LegacyBatchLogger());
            var summary = scheduler.RunNightlyWindow(DateTime.UtcNow.Date);
            WriteNightlySummary(summary);
            return summary.HasFailures ? 1 : 0;
        }

        private static void WriteNightlySummary(NightlyBatchRunSummary summary)
        {
            Console.WriteLine("Fabrikam Enterprise Pizza - Nightly Reporting Batch");
            Console.WriteLine("Fabrikam Enterprise Pizza - Nightly ETL and Integration Batch");
            Console.WriteLine("Window started : {0}", summary.StartedUtc.ToString("u", CultureInfo.InvariantCulture));
            Console.WriteLine("Window finished: {0}", summary.CompletedUtc.ToString("u", CultureInfo.InvariantCulture));
            Console.WriteLine();

            foreach (var result in summary.JobResults)
            {
                Console.WriteLine(
                    "{0} - {1} (reports: {2}, sql stubs: {3})",
                    result.JobName,
                    result.Succeeded ? "Succeeded" : "Failed",
                    result.ReportsGenerated,
                    result.SqlScriptsPrepared);
                    "{0} - {1} (attempts: {2}, extracted: {3}, loaded: {4})",
                    result.JobName,
                    result.Succeeded ? "Succeeded" : "Failed",
                    result.AttemptCount,
                    result.RowsExtracted,
                    result.RowsLoaded);
                Console.WriteLine("  {0}", result.SummaryMessage);
            }
        }

        private static void RunPartnerProfitabilityReport()
        {
            var summaryDate = DateTime.UtcNow.Date;
            var reports = BuildPartnerProfitabilityReports(summaryDate);

            Console.WriteLine("Fabrikam Enterprise Pizza - Partner Profitability Report");
            Console.WriteLine("Summary date: {0}", summaryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            Console.WriteLine();

            foreach (var report in reports)
            {
                Console.WriteLine("{0} ({1}) - {2}", report.Partner.PartnerName, report.Partner.PartnerCode, report.Partner.RelationshipTier);
                Console.WriteLine(
                    "  Revenue      : {0} across {1} orders (growth {2:N1}%)",
                    report.Revenue.GrossRevenue.ToString("C", DisplayCulture),
                    report.Volume.DeliveredOrders,
                    report.Revenue.GrowthPercentage);
                Console.WriteLine(
                    "  Volume       : {0} catering orders, avg ticket {1}, repeat rate {2:N1}%",
                    report.Volume.CateringOrders,
                    report.Volume.AverageTicket.ToString("C", DisplayCulture),
                    report.Volume.RepeatOrderRate);
                Console.WriteLine(
                    "  Commission   : {0} at {1:N2}% effective rate",
                    report.Commission.GrossCommission.ToString("C", DisplayCulture),
                    report.Commission.EffectiveRatePercentage);
                Console.WriteLine(
                    "  Chargebacks  : {0} cases, dispute rate {1:N1}%, net exposure {2}",
                    report.Chargebacks.ChargebackCount,
                    report.Chargebacks.DisputeRatePercentage,
                    report.Chargebacks.OutstandingExposure.ToString("C", DisplayCulture));
                Console.WriteLine(
                    "  Net revenue  : {0}",
                    report.NetRevenueImpact.NetRevenueAfterAdjustments.ToString("C", DisplayCulture));
                Console.WriteLine();
            }
        }

        private static IList<PartnerFinancialReport> BuildPartnerProfitabilityReports(DateTime summaryDate)
        {
            var revenueAggregator = new PartnerRevenueAggregator();
            var reports = revenueAggregator.BuildReports(summaryDate);
            var commissionCalculator = new CommissionCalculator();
            commissionCalculator.Apply(reports, summaryDate);
            var chargebackProcessor = new ChargebackProcessor();
            chargebackProcessor.Apply(reports);
            return reports;
        }

        private static void RunWorkforceSnapshotReport()
        {
            var summaryDate = DateTime.UtcNow.Date;
            var workforceReportingService = new WorkforceReportingService();
            var snapshot = workforceReportingService.GetSnapshot("014", summaryDate);
            var ssrsReportingService = new ReportingService();

            Console.WriteLine("Fabrikam Enterprise Pizza - Workforce Reporting Batch");
            Console.WriteLine("Store 014 summary date: {0}", summaryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            Console.WriteLine();

            WriteLaborCostSection(snapshot.LaborCost);
            WriteOvertimeSection(snapshot);
            WriteTurnoverSection(snapshot.Turnover);
            WriteStaffingSection(snapshot.Staffing);
            WriteSsrsFrameworkSection(ssrsReportingService, summaryDate);
        }

        private static void WriteSsrsFrameworkSection(ReportingService ssrsReportingService, DateTime summaryDate)
        {
            Console.WriteLine();
            Console.WriteLine("SSRS reporting framework");
            Console.WriteLine("  Execution endpoint : {0}", ssrsReportingService.Configuration.ExecutionEndpoint);
            Console.WriteLine("  Default format     : {0}", ssrsReportingService.Configuration.DefaultRenderFormat);

            foreach (var dataSource in ssrsReportingService.DataSources)
            {
                Console.WriteLine("  Shared data source : {0} ({1})", dataSource.SharedDataSourcePath, dataSource.Provider);
            }

            Console.WriteLine();
            WriteExecutionPreview(ssrsReportingService, ReportCatalog.DeliveryPerformance, new DeliveryPerformanceReportParameters
            {
                StoreNumber = "014",
                SummaryDate = summaryDate,
                MinimumCompletedRuns = 1
            });
            WriteExecutionPreview(ssrsReportingService, ReportCatalog.StoreOperationsSummary, new StoreOperationsSummaryReportParameters
            {
                StoreNumber = "014",
                StartDate = summaryDate.AddDays(-6),
                EndDate = summaryDate,
                RollupMode = "Weekly"
            });
            WriteExecutionPreview(ssrsReportingService, ReportCatalog.PartnerProfitability, new PartnerProfitabilityReportParameters
            {
                SummaryDate = summaryDate,
                PartnerCode = "ALL",
                MinimumGrossSales = 0m
            });
        }

        private static void WriteExecutionPreview(ReportingService ssrsReportingService, ReportDefinition reportDefinition, IReportParameterModel parameterModel)
        {
            var request = ssrsReportingService.CreateExecutionRequest(reportDefinition, parameterModel);
            Console.WriteLine("  {0}", reportDefinition.DisplayName);
            Console.WriteLine("    Report path : {0}", request.ReportPath);
            Console.WriteLine("    Definition  : {0}", reportDefinition.LocalDefinitionPath);
            foreach (var parameter in request.Parameters)
            {
                Console.WriteLine("    {0} = {1}", parameter.Name, parameter.Value);
            }
        }

        private static void WriteLaborCostSection(LaborCostSummary laborCost)
        {
            Console.WriteLine("Labor cost summary");
            Console.WriteLine("  Scheduled hours : {0:N1}", laborCost.ScheduledHours);
            Console.WriteLine("  Worked hours    : {0:N1}", laborCost.WorkedHours);
            Console.WriteLine("  Overtime hours  : {0:N1}", laborCost.OvertimeHours);
            Console.WriteLine("  Regular labor   : {0}", laborCost.RegularLaborCost.ToString("C", DisplayCulture));
            Console.WriteLine("  Overtime labor  : {0}", laborCost.OvertimeLaborCost.ToString("C", DisplayCulture));
            Console.WriteLine("  Agency labor    : {0}", laborCost.AgencyLaborCost.ToString("C", DisplayCulture));
            Console.WriteLine("  Net sales       : {0}", laborCost.NetSales.ToString("C", DisplayCulture));
            Console.WriteLine("  Labor percent   : {0:N1}%", laborCost.LaborCostPercentageOfSales);
            Console.WriteLine();
        }

        private static void WriteOvertimeSection(WorkforceReportSnapshot snapshot)
        {
            Console.WriteLine("Overtime trend (last 4 weeks)");
            foreach (var point in snapshot.OvertimeTrend)
            {
                Console.WriteLine(
                    "  Week ending {0}: drivers {1:N1}h, kitchen {2:N1}h, shift leads {3:N1}h, total {4:N1}h",
                    point.WeekEndingDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    point.DriverOvertimeHours,
                    point.KitchenOvertimeHours,
                    point.ShiftLeadOvertimeHours,
                    point.TotalOvertimeHours);
            }

            Console.WriteLine();
        }

        private static void WriteTurnoverSection(TurnoverSummary turnover)
        {
            Console.WriteLine("Turnover summary");
            Console.WriteLine("  Beginning headcount : {0}", turnover.BeginningHeadcount);
            Console.WriteLine("  Hires               : {0}", turnover.HireCount);
            Console.WriteLine("  Separations         : {0}", turnover.SeparationCount);
            Console.WriteLine("  Ending headcount    : {0}", turnover.EndingHeadcount);
            Console.WriteLine("  Turnover rate       : {0:N1}%", turnover.TurnoverRate);
            Console.WriteLine();
        }

        private static void WriteStaffingSection(StaffingSummary staffing)
        {
            Console.WriteLine("Staffing coverage");
            Console.WriteLine("  Driver slots scheduled : {0}", staffing.ScheduledDriverSlots);
            Console.WriteLine("  Driver slots filled    : {0}", staffing.FilledDriverSlots);
            Console.WriteLine("  Driver slots open      : {0}", staffing.OpenDriverSlots);
            Console.WriteLine("  Cross-trained backup   : {0}", staffing.CrossTrainedTeamMembers);
            Console.WriteLine("  Callouts               : {0}", staffing.CalloutCount);
            Console.WriteLine("  Coverage               : {0:N1}%", staffing.StaffingCoverageRate);
        }
    }
}
