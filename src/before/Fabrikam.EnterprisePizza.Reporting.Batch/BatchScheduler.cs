using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;
using Fabrikam.EnterprisePizza.Reporting.Batch.Configuration;
using Fabrikam.EnterprisePizza.Reporting.Batch.Execution;
using Fabrikam.EnterprisePizza.Reporting.Batch.Jobs;
using Fabrikam.EnterprisePizza.Reporting.Batch.Logging;

namespace Fabrikam.EnterprisePizza.Reporting.Batch
{
    public class BatchScheduler
    {
        private readonly IDictionary<string, ILegacyBatchJob> jobCatalog;
        private readonly LegacyBatchLogger logger;
        private readonly BatchSettingsProvider settingsProvider;

        public BatchScheduler(BatchSettingsProvider settingsProvider, LegacyBatchLogger logger)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            jobCatalog = new Dictionary<string, ILegacyBatchJob>(StringComparer.OrdinalIgnoreCase)
            {
                { "StoreOpsRollup", new StoreOpsRollupJob(logger) },
                { "CustomerHubSync", new CustomerHubSyncJob(logger) },
                { "PayrollFeedImport", new PayrollFeedImportJob(logger) },
                { "ReportingAggregation", new ReportingAggregationJob(logger) }
            };
        }

        public NightlyBatchRunSummary RunNightlyWindow(DateTime processDate)
        {
            var summary = new NightlyBatchRunSummary
            {
                StartedUtc = DateTime.UtcNow
            };
            var context = new NightlyBatchContext(processDate);

            foreach (var definition in settingsProvider.LoadNightlyJobs())
            {
                if (!definition.Enabled)
                {
                    logger.Warn("Skipping disabled nightly job {0}.", definition.Name);
                    continue;
                }

                ILegacyBatchJob job;
                if (!jobCatalog.TryGetValue(definition.Name, out job))
                {
                    summary.JobResults.Add(new BatchJobExecutionResult
                    {
                        JobName = definition.Name,
                        AttemptCount = 0,
                        RowsExtracted = 0,
                        RowsLoaded = 0,
                        StartedUtc = DateTime.UtcNow,
                        CompletedUtc = DateTime.UtcNow,
                        Succeeded = false,
                        SummaryMessage = "No batch implementation is registered for this configuration entry."
                    });
                    logger.Error("No nightly batch implementation is registered for {0}.", definition.Name);
                    continue;
                }

                var stage = job.Execute(definition, context);
                summary.JobResults.Add(stage.Result);
                context.RecordStage(definition.Name, stage.WorkingSet);
            }

            summary.CompletedUtc = DateTime.UtcNow;
            return summary;
        }
    }
}
