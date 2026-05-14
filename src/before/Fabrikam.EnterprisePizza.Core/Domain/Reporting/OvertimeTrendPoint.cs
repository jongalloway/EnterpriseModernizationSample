using System;

namespace Fabrikam.EnterprisePizza.Core.Domain.Reporting
{
    public class OvertimeTrendPoint
    {
        public string StoreNumber { get; set; }

        public DateTime WeekEndingDate { get; set; }

        public decimal DriverOvertimeHours { get; set; }

        public decimal KitchenOvertimeHours { get; set; }

        public decimal ShiftLeadOvertimeHours { get; set; }

        public decimal TotalOvertimeHours { get; set; }
    }
}
