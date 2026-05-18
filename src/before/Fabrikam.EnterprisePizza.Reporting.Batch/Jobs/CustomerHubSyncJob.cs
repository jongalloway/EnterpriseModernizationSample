using System;
using System.Data;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;
using Fabrikam.EnterprisePizza.Reporting.Batch.Execution;
using Fabrikam.EnterprisePizza.Reporting.Batch.Logging;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Jobs
{
    public class CustomerHubSyncJob : LegacyBatchJobBase
    {
        public CustomerHubSyncJob(LegacyBatchLogger logger)
            : base(logger)
        {
        }

        public override string JobName
        {
            get { return "CustomerHubSync"; }
        }

        protected override DataSet ExecuteWorkingSet(BatchJobDefinition definition, NightlyBatchContext context)
        {
            var workingSet = new DataSet("CustomerHubNightlySync");
            var table = workingSet.Tables.Add("CustomerHubPartnerSnapshot");
            table.Columns.Add("PartnerId", typeof(string));
            table.Columns.Add("AccountCode", typeof(string));
            table.Columns.Add("FranchiseLocationCount", typeof(int));
            table.Columns.Add("ActiveContractCount", typeof(int));
            table.Columns.Add("SnapshotUtc", typeof(DateTime));
            table.Columns.Add("SyncStatus", typeof(string));

            var snapshotUtc = context.ProcessDate.Date.AddHours(1).AddMinutes(25);
            table.Rows.Add("PARTNER-1002", "CORP-1002", 4, 2, snapshotUtc, "ReadyForRollup");
            table.Rows.Add("PARTNER-8821", "COMM-8821", 1, 1, snapshotUtc, "ReadyForRollup");
            return workingSet;
        }

        protected override string BuildSummaryMessage(BatchJobDefinition definition, DataSet workingSet)
        {
            return CountRows(workingSet) + " CustomerHub partner rows synchronized toward " + definition.TargetConnectionName + ".";
        }
    }
}
