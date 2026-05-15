using System;
using System.Linq;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.Repositories.Reporting;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Repositories.Reporting
{
    [TestFixture]
    public class WorkforceReportRepositoryFixture
    {
        [Test]
        public void GetLaborCostSummary_returns_store_specific_metrics_and_requested_summary_date()
        {
            var summaryDate = new DateTime(2026, 5, 12);
            var repository = CreateRepository();

            var summary = repository.GetLaborCostSummary("022", summaryDate);

            Assert.That(summary, Is.Not.Null);
            Assert.That(summary.StoreNumber, Is.EqualTo("022"));
            Assert.That(summary.SummaryDate, Is.EqualTo(summaryDate));
            Assert.That(summary.ScheduledHours, Is.EqualTo(118.0m));
            Assert.That(summary.OvertimeLaborCost, Is.EqualTo(68.25m));
            Assert.That(summary.LaborCostPercentageOfSales, Is.EqualTo(29.95m));
        }

        [Test]
        public void GetLaborCostSummary_returns_null_when_reporting_stub_has_no_rows()
        {
            var repository = CreateRepository();

            var summary = repository.GetLaborCostSummary("999", new DateTime(2026, 5, 12));

            Assert.That(summary, Is.Null);
        }

        [Test]
        public void GetOvertimeTrend_returns_requested_weeks_with_totals_computed_by_stub()
        {
            var repository = CreateRepository();

            var points = repository.GetOvertimeTrend("022", 2);

            Assert.That(points, Has.Count.EqualTo(2));
            Assert.That(points.Select(point => point.StoreNumber), Is.All.EqualTo("022"));
            Assert.That(points.Select(point => point.TotalOvertimeHours), Is.EqualTo(new[] { 4.0m, 4.0m }));
            Assert.That(points[0].WeekEndingDate - points[1].WeekEndingDate, Is.EqualTo(TimeSpan.FromDays(7)));
        }

        [Test]
        public void GetTurnoverSummary_normalizes_summary_month_to_first_day_of_month()
        {
            var repository = CreateRepository();

            var summary = repository.GetTurnoverSummary("022", new DateTime(2026, 5, 20));

            Assert.That(summary, Is.Not.Null);
            Assert.That(summary.SummaryMonth, Is.EqualTo(new DateTime(2026, 5, 1)));
            Assert.That(summary.BeginningHeadcount, Is.EqualTo(19));
            Assert.That(summary.TurnoverRate, Is.EqualTo(5.26m));
        }

        [Test]
        public void GetStaffingSummary_returns_expected_driver_coverage_metrics()
        {
            var summaryDate = new DateTime(2026, 5, 12);
            var repository = CreateRepository();

            var summary = repository.GetStaffingSummary("014", summaryDate);

            Assert.That(summary, Is.Not.Null);
            Assert.That(summary.SummaryDate, Is.EqualTo(summaryDate));
            Assert.That(summary.ScheduledDriverSlots, Is.EqualTo(18));
            Assert.That(summary.OpenDriverSlots, Is.EqualTo(3));
            Assert.That(summary.StaffingCoverageRate, Is.EqualTo(83.33m));
        }

        private static WorkforceReportRepository CreateRepository()
        {
            return new WorkforceReportRepository(new LegacyDbGateway());
        }
    }
}
