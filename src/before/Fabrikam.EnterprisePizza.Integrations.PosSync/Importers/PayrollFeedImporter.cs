using System;
using System.Data;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;

namespace Fabrikam.EnterprisePizza.Integrations.PosSync.Importers
{
    public class PayrollFeedImporter
    {
        public DataSet Import(BatchJobDefinition definition, DateTime processDate)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            var workingSet = new DataSet("PayrollFeedImport");
            var table = workingSet.Tables.Add("PayrollShiftImport");
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("EmployeeNumber", typeof(string));
            table.Columns.Add("PayPeriodEndDate", typeof(DateTime));
            table.Columns.Add("RegularHours", typeof(decimal));
            table.Columns.Add("OvertimeHours", typeof(decimal));
            table.Columns.Add("GrossLaborCost", typeof(decimal));
            table.Columns.Add("BenefitCode", typeof(string));
            table.Columns.Add("SourceFeedPath", typeof(string));
            table.Columns.Add("ImportedUtc", typeof(DateTime));

            var payPeriodEndDate = processDate.Date.AddDays(-1);
            var importedUtc = processDate.Date.AddHours(2).AddMinutes(35);
            var sourceFeedPath = string.IsNullOrWhiteSpace(definition.FeedPath)
                ? @"\\payroll-gw01\exports\benefits\PayrollNightly.xml"
                : definition.FeedPath;

            table.Rows.Add("014", "E01417", payPeriodEndDate, 38.50m, 2.50m, 712.44m, "MEDICAL-A", sourceFeedPath, importedUtc);
            table.Rows.Add("014", "E01442", payPeriodEndDate, 24.00m, 0.00m, 318.12m, "DENTAL-B", sourceFeedPath, importedUtc);
            table.Rows.Add("022", "E02203", payPeriodEndDate, 36.00m, 1.75m, 569.38m, "MEDICAL-A", sourceFeedPath, importedUtc);

            return workingSet;
        }
    }
}
