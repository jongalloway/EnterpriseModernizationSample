using System;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;
using Fabrikam.EnterprisePizza.Reporting.Batch.Execution;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Jobs
{
    public interface ILegacyBatchJob
    {
        string JobName { get; }

        BatchJobExecutionResult Execute(BatchJobDefinition definition, DateTime processDate);
        NightlyBatchStage Execute(BatchJobDefinition definition, NightlyBatchContext context);
    }
}
