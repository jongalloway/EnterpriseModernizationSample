using System;

namespace Fabrikam.EnterprisePizza.Core.Domain.Reporting
{
    public class StaffingSummary
    {
        public string StoreNumber { get; set; }

        public DateTime SummaryDate { get; set; }

        public int ScheduledDriverSlots { get; set; }

        public int FilledDriverSlots { get; set; }

        public int OpenDriverSlots { get; set; }

        public int CrossTrainedTeamMembers { get; set; }

        public int CalloutCount { get; set; }

        public decimal StaffingCoverageRate { get; set; }
    }
}
