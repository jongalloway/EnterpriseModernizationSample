using System;
using System.Collections.Generic;
using System.Data;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Execution
{
    public class NightlyBatchContext
    {
        private readonly IDictionary<string, DataSet> workingSets;

        public NightlyBatchContext(DateTime processDate)
        {
            ProcessDate = processDate;
            workingSets = new Dictionary<string, DataSet>(StringComparer.OrdinalIgnoreCase);
        }

        public DateTime ProcessDate { get; private set; }

        public void RecordStage(string jobName, DataSet workingSet)
        {
            if (string.IsNullOrWhiteSpace(jobName))
            {
                throw new ArgumentException("A batch job name is required.", nameof(jobName));
            }

            workingSets[jobName] = workingSet;
        }

        public bool TryGetStage(string jobName, out DataSet workingSet)
        {
            return workingSets.TryGetValue(jobName, out workingSet);
        }
    }
}
