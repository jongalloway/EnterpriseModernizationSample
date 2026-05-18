using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Models
{
    public class BatchJobDefinition
    {
        public string Name { get; set; }

        public string Schedule { get; set; }

        public bool Enabled { get; set; }
    }

    public class BatchJobExecutionResult
    {
        public string JobName { get; set; }

        public DateTime StartedUtc { get; set; }

        public DateTime CompletedUtc { get; set; }

        public bool Succeeded { get; set; }

        public int ReportsGenerated { get; set; }

        public int SqlScriptsPrepared { get; set; }

        public string SummaryMessage { get; set; }
    }

    public class NightlyBatchRunSummary
    {
        private readonly IList<BatchJobExecutionResult> jobResults = new List<BatchJobExecutionResult>();

        public DateTime StartedUtc { get; set; }

        public DateTime CompletedUtc { get; set; }

        public IList<BatchJobExecutionResult> JobResults
        {
            get { return jobResults; }
        }

        public bool HasFailures
        {
            get { return jobResults.Any(result => !result.Succeeded); }
        }
    }
}
