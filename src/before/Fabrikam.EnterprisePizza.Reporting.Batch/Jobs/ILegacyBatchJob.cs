using System;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Jobs
{
    public interface ILegacyBatchJob
    {
        string JobName { get; }

        BatchJobExecutionResult Execute(BatchJobDefinition definition, DateTime processDate);
    }
}
