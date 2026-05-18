using System.Data;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Execution
{
    public class NightlyBatchStage
    {
        public DataSet WorkingSet { get; set; }

        public BatchJobExecutionResult Result { get; set; }
    }
}
