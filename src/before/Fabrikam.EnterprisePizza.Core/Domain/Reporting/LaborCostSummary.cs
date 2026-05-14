using System;

namespace Fabrikam.EnterprisePizza.Core.Domain.Reporting
{
    public class LaborCostSummary
    {
        public string StoreNumber { get; set; }

        public DateTime SummaryDate { get; set; }

        public decimal ScheduledHours { get; set; }

        public decimal WorkedHours { get; set; }

        public decimal OvertimeHours { get; set; }

        public decimal RegularLaborCost { get; set; }

        public decimal OvertimeLaborCost { get; set; }

        public decimal AgencyLaborCost { get; set; }

        public decimal NetSales { get; set; }

        public decimal LaborCostPercentageOfSales { get; set; }
    }
}
