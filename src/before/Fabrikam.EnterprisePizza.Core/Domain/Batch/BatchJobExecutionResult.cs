using System;

namespace Fabrikam.EnterprisePizza.Core.Domain.Batch
{
    public class BatchJobExecutionResult
    {
        public string JobName { get; set; }

        public bool Succeeded { get; set; }

        public int AttemptCount { get; set; }

        public int RowsExtracted { get; set; }

        public int RowsLoaded { get; set; }

        public DateTime StartedUtc { get; set; }

        public DateTime CompletedUtc { get; set; }

        public string SummaryMessage { get; set; }
    }
}
