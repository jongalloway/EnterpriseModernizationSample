using System;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Repositories.Reporting;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Fabrikam.EnterprisePizza.Tests.Unit.Testing;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Repositories.Reporting
{
    [TestFixture]
    public class WorkforceReportRepositoryInteractionFixture
    {
        private RecordingLegacyDbGateway gateway;
        private WorkforceReportRepository repository;

        [SetUp]
        public void SetUp()
        {
            gateway = new RecordingLegacyDbGateway();
            repository = new WorkforceReportRepository(gateway, new LegacyConnectionCatalog());
        }

        [TearDown]
        public void TearDown()
        {
            gateway = null;
            repository = null;
        }

        [Test]
        public void GetLaborCostSummary_builds_reporting_call_with_summary_date_parameter()
        {
            var summaryDate = new DateTime(2026, 5, 18);
            gateway.DataSetToReturn = CreateLaborCostSummaryDataSet(summaryDate);

            var summary = repository.GetLaborCostSummary("014", summaryDate);

            Assert.That(gateway.LastExecutedCall.ConnectionName, Is.EqualTo("FabrikamPizza_Reporting"));
            Assert.That(gateway.LastExecutedCall.StoredProcedureName, Is.EqualTo(LegacyStoredProcedures.Reporting.GetLaborCostSummary));
            Assert.That(gateway.LastExecutedCall.Parameters, Has.Count.EqualTo(2));
            Assert.That(gateway.LastExecutedCall.Parameters[0].Value, Is.EqualTo("014"));
            Assert.That(gateway.LastExecutedCall.Parameters[1].Value, Is.EqualTo(summaryDate));
            Assert.That(summary.OvertimeLaborCost, Is.EqualTo(91.5m));
        }

        [Test]
        public void GetOvertimeTrend_passes_requested_weeks_back_and_maps_multiple_rows()
        {
            gateway.DataSetToReturn = CreateOvertimeTrendDataSet();

            var points = repository.GetOvertimeTrend("014", 3);

            Assert.That(gateway.LastExecutedCall.StoredProcedureName, Is.EqualTo(LegacyStoredProcedures.Reporting.GetOvertimeTrend));
            Assert.That(gateway.LastExecutedCall.Parameters[1].Value, Is.EqualTo(3));
            Assert.That(points, Has.Count.EqualTo(2));
            Assert.That(points[1].TotalOvertimeHours, Is.EqualTo(4.75m));
        }

        [Test]
        public void GetStaffingSummary_defaults_missing_numeric_columns_to_zero()
        {
            gateway.DataSetToReturn = CreateSparseStaffingSummaryDataSet();

            var summary = repository.GetStaffingSummary("014", new DateTime(2026, 5, 18));

            Assert.That(summary, Is.Not.Null);
            Assert.That(summary.ScheduledDriverSlots, Is.EqualTo(0));
            Assert.That(summary.StaffingCoverageRate, Is.EqualTo(0m));
        }

        private static DataSet CreateLaborCostSummaryDataSet(DateTime summaryDate)
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add("LaborCostSummary");
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("SummaryDate", typeof(DateTime));
            table.Columns.Add("OvertimeLaborCost", typeof(decimal));
            table.Rows.Add("014", summaryDate, 91.5m);
            return dataSet;
        }

        private static DataSet CreateOvertimeTrendDataSet()
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add("OvertimeTrend");
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("WeekEndingDate", typeof(DateTime));
            table.Columns.Add("DriverOvertimeHours", typeof(decimal));
            table.Columns.Add("KitchenOvertimeHours", typeof(decimal));
            table.Columns.Add("ShiftLeadOvertimeHours", typeof(decimal));
            table.Columns.Add("TotalOvertimeHours", typeof(decimal));
            table.Rows.Add("014", new DateTime(2026, 5, 18), 2.25m, 1.0m, 0.5m, 3.75m);
            table.Rows.Add("014", new DateTime(2026, 5, 11), 3.0m, 1.25m, 0.5m, 4.75m);
            return dataSet;
        }

        private static DataSet CreateSparseStaffingSummaryDataSet()
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add("StaffingSummary");
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("SummaryDate", typeof(DateTime));
            table.Rows.Add("014", new DateTime(2026, 5, 18));
            return dataSet;
        }
    }
}
