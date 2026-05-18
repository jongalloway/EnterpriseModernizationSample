using System;
using System.Data;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;
using Fabrikam.EnterprisePizza.Reporting.Batch.Execution;
using Fabrikam.EnterprisePizza.Reporting.Batch.Logging;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Jobs
{
    public class StoreOpsRollupJob : LegacyBatchJobBase
    {
        public StoreOpsRollupJob(LegacyBatchLogger logger)
            : base(logger)
        {
        }

        public override string JobName
        {
            get { return "StoreOpsRollup"; }
        }

        protected override DataSet ExecuteWorkingSet(BatchJobDefinition definition, NightlyBatchContext context)
        {
            var workingSet = new DataSet("StoreOpsNightlyRollup");
            var table = workingSet.Tables.Add("StoreOpsRollup");
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("BusinessDate", typeof(DateTime));
            table.Columns.Add("TicketCount", typeof(int));
            table.Columns.Add("NetSales", typeof(decimal));
            table.Columns.Add("LaborHours", typeof(decimal));
            table.Columns.Add("LoadDisposition", typeof(string));

            var businessDate = context.ProcessDate.Date.AddDays(-1);
            table.Rows.Add("014", businessDate, 178, 8125.50m, 164.00m, "ReadyForReporting");
            table.Rows.Add("022", businessDate, 121, 5940.00m, 118.00m, "ReadyForReporting");
            return workingSet;
        }

        protected override string BuildSummaryMessage(BatchJobDefinition definition, DataSet workingSet)
        {
            return CountRows(workingSet) + " store rollup rows staged from " + definition.SourceConnectionName + ".";
        }
    }
}
