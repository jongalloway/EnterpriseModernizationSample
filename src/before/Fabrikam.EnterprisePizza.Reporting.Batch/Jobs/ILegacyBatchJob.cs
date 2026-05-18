using Fabrikam.EnterprisePizza.Core.Domain.Batch;
using Fabrikam.EnterprisePizza.Reporting.Batch.Execution;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Jobs
{
    public interface ILegacyBatchJob
    {
        string JobName { get; }

        NightlyBatchStage Execute(BatchJobDefinition definition, NightlyBatchContext context);
    }
}
