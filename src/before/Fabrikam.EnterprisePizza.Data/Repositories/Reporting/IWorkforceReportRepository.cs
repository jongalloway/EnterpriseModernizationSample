using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Domain.Reporting;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public interface IWorkforceReportRepository
    {
        LaborCostSummary GetLaborCostSummary(string storeNumber, DateTime summaryDate);

        IList<OvertimeTrendPoint> GetOvertimeTrend(string storeNumber, int weeksBack);

        TurnoverSummary GetTurnoverSummary(string storeNumber, DateTime summaryMonth);

        StaffingSummary GetStaffingSummary(string storeNumber, DateTime summaryDate);
    }
}
