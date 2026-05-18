using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public class DimPartnerRepository : ReportingRepositoryBase, IDimPartnerRepository
    {
        public DimPartnerRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public DimPartnerRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
            : base(gateway, connectionCatalog)
        {
        }

        public ReportingWarehouseDimensionsDataSet GetPartners(string partnerTier, bool includeInactive)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetDimPartnerCatalog,
                new GatewayParameter("@PartnerTier", partnerTier),
                new GatewayParameter("@IncludeInactive", includeInactive));

            return ReportingWarehouseDimensionsDataSet.FromTable(ReportingWarehouseDimensionsDataSet.DimPartnerTableName, GetFirstTable(source));
        }
    }
}
