using System;
using System.Data;
using System.Linq;
using System.Threading;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;
using Fabrikam.EnterprisePizza.Core.ExceptionHandling;
using Fabrikam.EnterprisePizza.Reporting.Batch.Execution;
using Fabrikam.EnterprisePizza.Reporting.Batch.Logging;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Jobs
{
    public abstract class LegacyBatchJobBase : ILegacyBatchJob
    {
        private readonly LegacyBatchLogger logger;

        protected LegacyBatchJobBase(LegacyBatchLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public abstract string JobName { get; }

        protected LegacyBatchLogger Logger
        {
            get { return logger; }
        }

        public BatchJobExecutionResult Execute(BatchJobDefinition definition, DateTime processDate)
        {
            return Execute(definition, new NightlyBatchContext(processDate)).Result;
        }

        public NightlyBatchStage Execute(BatchJobDefinition definition, NightlyBatchContext context)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var maxAttempts = Math.Max(1, definition.MaxRetryCount + 1);
            var startedUtc = DateTime.UtcNow;
            Exception lastError = null;

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    logger.Info(
                        "Starting {0} from {1} to {2} on {3} (attempt {4}/{5}).",
                        definition.Name,
                        FormatConnection(definition.SourceConnectionName),
                        FormatConnection(definition.TargetConnectionName),
                        definition.Schedule,
                        attempt,
                        maxAttempts);

                    var workingSet = ExecuteWorkingSet(definition, context) ?? new DataSet(definition.Name + "WorkingSet");
                    var extractedRows = CountRows(workingSet);
                    var result = new BatchJobExecutionResult
                    {
                        JobName = definition.Name,
                        AttemptCount = attempt,
                        RowsExtracted = extractedRows,
                        RowsLoaded = GetRowsLoaded(workingSet),
                        StartedUtc = startedUtc,
                        CompletedUtc = DateTime.UtcNow,
                        Succeeded = true,
                        SummaryMessage = BuildSummaryMessage(definition, workingSet)
                    };

                    logger.Info("Completed {0}: {1}", definition.Name, result.SummaryMessage);
                    return new NightlyBatchStage
                    {
                        WorkingSet = workingSet,
                        Result = result
                    };
                }
                catch (Exception ex)
                {
                    lastError = HandleException(ex, definition.Name);
                    logger.Error("{0} failed on attempt {1}: {2}", definition.Name, attempt, lastError.Message);
                    if (attempt < maxAttempts && definition.RetryIntervalSeconds > 0)
                    {
                        logger.Warn("Retry policy sleeping {0} seconds before replaying {1}.", definition.RetryIntervalSeconds, definition.Name);
                        Thread.Sleep(Math.Min(definition.RetryIntervalSeconds * 100, 1000));
                    }
                }
            }

            return new NightlyBatchStage
            {
                WorkingSet = new DataSet(definition.Name + "FailedWorkingSet"),
                Result = new BatchJobExecutionResult
                {
                    JobName = definition.Name,
                    AttemptCount = maxAttempts,
                    RowsExtracted = 0,
                    RowsLoaded = 0,
                    StartedUtc = startedUtc,
                    CompletedUtc = DateTime.UtcNow,
                    Succeeded = false,
                    SummaryMessage = lastError == null ? "Batch job failed without an error message." : lastError.Message
                }
            };
        }

        protected virtual int GetRowsLoaded(DataSet workingSet)
        {
            return CountRows(workingSet);
        }

        protected virtual string BuildSummaryMessage(BatchJobDefinition definition, DataSet workingSet)
        {
            return CountRows(workingSet) + " rows staged for " + FormatConnection(definition.TargetConnectionName) + ".";
        }

        protected int CountRows(DataSet workingSet)
        {
            if (workingSet == null || workingSet.Tables.Count == 0)
            {
                return 0;
            }

            return workingSet.Tables.Cast<DataTable>().Sum(table => table.Rows.Count);
        }

        protected abstract DataSet ExecuteWorkingSet(BatchJobDefinition definition, NightlyBatchContext context);

        private static string FormatConnection(string connectionName)
        {
            return string.IsNullOrWhiteSpace(connectionName) ? "(none)" : connectionName;
        }

        private static Exception HandleException(Exception exception, string jobName)
        {
            Exception exceptionToThrow;
            var boundaryException = new InvalidOperationException("Nightly batch job failed: " + jobName + ".", exception);
            if (ExceptionPolicy.HandleException(boundaryException, "BatchProcessingPolicy", out exceptionToThrow) && exceptionToThrow != null)
            {
                return exceptionToThrow;
            }

            return boundaryException;
        }
    }
}
