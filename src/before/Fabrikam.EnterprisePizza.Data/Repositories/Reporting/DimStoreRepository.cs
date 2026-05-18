using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public class DimStoreRepository : ReportingRepositoryBase, IDimStoreRepository
    {
        public DimStoreRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public DimStoreRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
            : base(gateway, connectionCatalog)
        {
        }

        public ReportingWarehouseDimensionsDataSet GetStores(string regionName, bool includeInactive)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetDimStoreCatalog,
                new GatewayParameter("@RegionName", regionName),
                new GatewayParameter("@IncludeInactive", includeInactive));

            return ReportingWarehouseDimensionsDataSet.FromTable(ReportingWarehouseDimensionsDataSet.DimStoreTableName, GetFirstTable(source));
        }
    }
}
