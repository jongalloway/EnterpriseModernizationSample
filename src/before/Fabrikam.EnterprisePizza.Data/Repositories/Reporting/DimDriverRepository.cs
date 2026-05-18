using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public class DimDriverRepository : ReportingRepositoryBase, IDimDriverRepository
    {
        public DimDriverRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public DimDriverRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
            : base(gateway, connectionCatalog)
        {
        }

        public ReportingWarehouseDimensionsDataSet GetDrivers(string storeNumber, bool includeInactive)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetDimDriverCatalog,
                new GatewayParameter("@StoreNumber", storeNumber),
                new GatewayParameter("@IncludeInactive", includeInactive));

            return ReportingWarehouseDimensionsDataSet.FromTable(ReportingWarehouseDimensionsDataSet.DimDriverTableName, GetFirstTable(source));
        }
    }
}
