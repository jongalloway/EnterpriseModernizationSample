using System.Data;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;
using Fabrikam.EnterprisePizza.Integrations.PosSync.Importers;
using Fabrikam.EnterprisePizza.Reporting.Batch.Execution;
using Fabrikam.EnterprisePizza.Reporting.Batch.Logging;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Jobs
{
    public class PayrollFeedImportJob : LegacyBatchJobBase
    {
        private readonly PayrollFeedImporter payrollFeedImporter;

        public PayrollFeedImportJob(LegacyBatchLogger logger)
            : this(logger, new PayrollFeedImporter())
        {
        }

        public PayrollFeedImportJob(LegacyBatchLogger logger, PayrollFeedImporter payrollFeedImporter)
            : base(logger)
        {
            this.payrollFeedImporter = payrollFeedImporter ?? throw new System.ArgumentNullException(nameof(payrollFeedImporter));
        }

        public override string JobName
        {
            get { return "PayrollFeedImport"; }
        }

        protected override DataSet ExecuteWorkingSet(BatchJobDefinition definition, NightlyBatchContext context)
        {
            return payrollFeedImporter.Import(definition, context.ProcessDate.Date);
        }

        protected override string BuildSummaryMessage(BatchJobDefinition definition, DataSet workingSet)
        {
            return CountRows(workingSet) + " payroll records staged from " + definition.FeedPath + ".";
        }
    }
}
