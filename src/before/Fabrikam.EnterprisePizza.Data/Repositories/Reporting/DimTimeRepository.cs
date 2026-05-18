using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public class DimTimeRepository : ReportingRepositoryBase, IDimTimeRepository
    {
        public DimTimeRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public DimTimeRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
            : base(gateway, connectionCatalog)
        {
        }

        public ReportingWarehouseDimensionsDataSet GetCalendarRange(DateTime startDate, DateTime endDate)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetDimTimeRange,
                new GatewayParameter("@StartDate", startDate),
                new GatewayParameter("@EndDate", endDate));

            return ReportingWarehouseDimensionsDataSet.FromTable(ReportingWarehouseDimensionsDataSet.DimTimeTableName, GetFirstTable(source));
        }
    }
}
