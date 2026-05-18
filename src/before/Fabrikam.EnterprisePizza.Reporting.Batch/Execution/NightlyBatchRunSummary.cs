using System;
using System.Collections.Generic;
using System.Linq;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Execution
{
    public class NightlyBatchRunSummary
    {
        private readonly IList<BatchJobExecutionResult> jobResults;

        public NightlyBatchRunSummary()
        {
            jobResults = new List<BatchJobExecutionResult>();
        }

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
