using System;
using System.Globalization;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Domain.Reporting;

namespace Fabrikam.EnterprisePizza.Reporting.Batch
{
    internal static class Program
    {
        private static readonly CultureInfo DisplayCulture = CultureInfo.GetCultureInfo("en-US");

        private static void Main()
        {
            var summaryDate = DateTime.UtcNow.Date;
            var reportingService = new WorkforceReportingService();
            var snapshot = reportingService.GetSnapshot("014", summaryDate);

            Console.WriteLine("Fabrikam Enterprise Pizza - Workforce Reporting Batch");
            Console.WriteLine("Store 014 summary date: {0}", summaryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            Console.WriteLine();

            WriteLaborCostSection(snapshot.LaborCost);
            WriteOvertimeSection(snapshot);
            WriteTurnoverSection(snapshot.Turnover);
            WriteStaffingSection(snapshot.Staffing);
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
