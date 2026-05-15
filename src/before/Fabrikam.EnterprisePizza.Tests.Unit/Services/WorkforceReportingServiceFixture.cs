using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Domain.Reporting;
using Fabrikam.EnterprisePizza.Data.Repositories.Reporting;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class WorkforceReportingServiceFixture
    {
        private static readonly DateTime SummaryDate = new DateTime(2005, 10, 14);

        [Test]
        public void GetSnapshot_assembles_all_four_components_from_repository()
        {
            var stub = new StubWorkforceReportRepository();
            var service = new WorkforceReportingService(stub);

            var snapshot = service.GetSnapshot("014", SummaryDate);

            Assert.That(snapshot.LaborCost, Is.Not.Null);
            Assert.That(snapshot.Turnover, Is.Not.Null);
            Assert.That(snapshot.Staffing, Is.Not.Null);
            Assert.That(snapshot.OvertimeTrend, Is.Not.Null);
        }

        [Test]
        public void GetSnapshot_propagates_labor_cost_from_repository()
        {
            var stub = new StubWorkforceReportRepository();
            var service = new WorkforceReportingService(stub);

            var snapshot = service.GetSnapshot("014", SummaryDate);

            Assert.That(snapshot.LaborCost.StoreNumber, Is.EqualTo("014"));
            Assert.That(snapshot.LaborCost.WorkedHours, Is.EqualTo(38.5m));
        }

        [Test]
        public void GetSnapshot_propagates_staffing_from_repository()
        {
            var stub = new StubWorkforceReportRepository();
            var service = new WorkforceReportingService(stub);

            var snapshot = service.GetSnapshot("014", SummaryDate);

            Assert.That(snapshot.Staffing.FilledDriverSlots, Is.EqualTo(3));
        }

        [Test]
        public void GetSnapshot_propagates_overtime_trend_points()
        {
            var stub = new StubWorkforceReportRepository();
            var service = new WorkforceReportingService(stub);

            var snapshot = service.GetSnapshot("014", SummaryDate);

            Assert.That(snapshot.OvertimeTrend, Has.Count.EqualTo(4));
            Assert.That(stub.LastOvertimeTrendWeeksBack, Is.EqualTo(4));
        }

        [Test]
        public void GetSnapshot_uses_first_of_month_for_turnover_query()
        {
            var stub = new StubWorkforceReportRepository();
            var service = new WorkforceReportingService(stub);

            service.GetSnapshot("014", new DateTime(2005, 10, 14));

            Assert.That(stub.LastTurnoverMonth, Is.EqualTo(new DateTime(2005, 10, 1)));
        }

        [Test]
        public void Constructor_rejects_null_repository()
        {
            Assert.Throws<ArgumentNullException>(() => new WorkforceReportingService(null));
        }

        private sealed class StubWorkforceReportRepository : IWorkforceReportRepository
        {
            public int LastOvertimeTrendWeeksBack { get; private set; }
            public DateTime LastTurnoverMonth { get; private set; }

            public LaborCostSummary GetLaborCostSummary(string storeNumber, DateTime summaryDate)
            {
                return new LaborCostSummary
                {
                    StoreNumber = storeNumber,
                    SummaryDate = summaryDate,
                    ScheduledHours = 40.0m,
                    WorkedHours = 38.5m,
                    OvertimeHours = 1.5m,
                    RegularLaborCost = 462.00m,
                    OvertimeLaborCost = 27.00m,
                    AgencyLaborCost = 0.00m,
                    NetSales = 3210.50m,
                    LaborCostPercentageOfSales = 15.2m
                };
            }

            public IList<OvertimeTrendPoint> GetOvertimeTrend(string storeNumber, int weeksBack)
            {
                LastOvertimeTrendWeeksBack = weeksBack;
                var points = new List<OvertimeTrendPoint>();
                for (int i = 0; i < weeksBack; i++)
                {
                    points.Add(new OvertimeTrendPoint
                    {
                        StoreNumber = storeNumber,
                        WeekEndingDate = SummaryDate.AddDays(-(7 * i)),
                        DriverOvertimeHours = 1.5m,
                        KitchenOvertimeHours = 0.5m,
                        ShiftLeadOvertimeHours = 0.0m,
                        TotalOvertimeHours = 2.0m
                    });
                }
                return points;
            }

            public TurnoverSummary GetTurnoverSummary(string storeNumber, DateTime summaryMonth)
            {
                LastTurnoverMonth = summaryMonth;
                return new TurnoverSummary
                {
                    StoreNumber = storeNumber,
                    SummaryMonth = summaryMonth,
                    BeginningHeadcount = 14,
                    HireCount = 1,
                    SeparationCount = 2,
                    EndingHeadcount = 13,
                    TurnoverRate = 14.3m
                };
            }

            public StaffingSummary GetStaffingSummary(string storeNumber, DateTime summaryDate)
            {
                return new StaffingSummary
                {
                    StoreNumber = storeNumber,
                    SummaryDate = summaryDate,
                    ScheduledDriverSlots = 4,
                    FilledDriverSlots = 3,
                    OpenDriverSlots = 1,
                    CrossTrainedTeamMembers = 2,
                    CalloutCount = 1,
                    StaffingCoverageRate = 75.0m
                };
            }
        }
    }
}
