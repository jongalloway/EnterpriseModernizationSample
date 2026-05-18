using System;
using System.Data;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;
using Fabrikam.EnterprisePizza.Reporting.Batch.Execution;
using Fabrikam.EnterprisePizza.Reporting.Batch.Logging;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Jobs
{
    public class ReportingAggregationJob : LegacyBatchJobBase
    {
        public ReportingAggregationJob(LegacyBatchLogger logger)
            : base(logger)
        {
        }

        public override string JobName
        {
            get { return "ReportingAggregation"; }
        }

        protected override DataSet ExecuteWorkingSet(BatchJobDefinition definition, NightlyBatchContext context)
        {
            var workingSet = new DataSet("ReportingAggregation");
            var table = workingSet.Tables.Add("NightlyReportingAggregation");
            table.Columns.Add("SourceStage", typeof(string));
            table.Columns.Add("RowCount", typeof(int));
            table.Columns.Add("AggregatedUtc", typeof(DateTime));
            table.Columns.Add("LoadTarget", typeof(string));

            var aggregatedUtc = context.ProcessDate.Date.AddHours(4).AddMinutes(10);
            AddStageRow(table, context, "StoreOpsRollup", aggregatedUtc, definition.TargetConnectionName);
            AddStageRow(table, context, "CustomerHubSync", aggregatedUtc, definition.TargetConnectionName);
            AddStageRow(table, context, "PayrollFeedImport", aggregatedUtc, definition.TargetConnectionName);
            return workingSet;
        }

        protected override string BuildSummaryMessage(BatchJobDefinition definition, DataSet workingSet)
        {
            return CountRows(workingSet) + " nightly aggregate checkpoints loaded into " + definition.TargetConnectionName + ".";
        }

        private static void AddStageRow(DataTable table, NightlyBatchContext context, string stageName, DateTime aggregatedUtc, string targetConnectionName)
        {
            DataSet stage;
            var rowCount = context.TryGetStage(stageName, out stage) ? CountStageRows(stage) : 0;
            table.Rows.Add(stageName, rowCount, aggregatedUtc, targetConnectionName);
        }

        private static int CountStageRows(DataSet workingSet)
        {
            if (workingSet == null || workingSet.Tables.Count == 0)
            {
                return 0;
            }

            var rowCount = 0;
            foreach (DataTable table in workingSet.Tables)
            {
                rowCount += table.Rows.Count;
            }

            return rowCount;
        }
    }
}
