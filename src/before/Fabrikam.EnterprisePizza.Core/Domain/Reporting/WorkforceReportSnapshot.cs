using System.Collections.Generic;

namespace Fabrikam.EnterprisePizza.Core.Domain.Reporting
{
    public class WorkforceReportSnapshot
    {
        public WorkforceReportSnapshot()
        {
            OvertimeTrend = new List<OvertimeTrendPoint>();
        }

        public LaborCostSummary LaborCost { get; set; }

        public IList<OvertimeTrendPoint> OvertimeTrend { get; private set; }

        public TurnoverSummary Turnover { get; set; }

        public StaffingSummary Staffing { get; set; }
    }
}
