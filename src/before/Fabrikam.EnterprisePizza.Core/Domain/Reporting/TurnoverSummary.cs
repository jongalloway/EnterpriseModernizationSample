using System;

namespace Fabrikam.EnterprisePizza.Core.Domain.Reporting
{
    public class TurnoverSummary
    {
        public string StoreNumber { get; set; }

        public DateTime SummaryMonth { get; set; }

        public int BeginningHeadcount { get; set; }

        public int HireCount { get; set; }

        public int SeparationCount { get; set; }

        public int EndingHeadcount { get; set; }

        public decimal TurnoverRate { get; set; }
    }
}
