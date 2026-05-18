using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public class FactPartnerRevenueRepository : ReportingRepositoryBase, IFactPartnerRevenueRepository
    {
        public FactPartnerRevenueRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public FactPartnerRevenueRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
            : base(gateway, connectionCatalog)
        {
        }

        public ReportingWarehouseFactsDataSet GetMonthlyPartnerRevenue(string partnerCode, DateTime summaryMonth)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetFactPartnerRevenueByMonth,
                new GatewayParameter("@PartnerCode", partnerCode),
                new GatewayParameter("@SummaryMonth", summaryMonth));

            return ReportingWarehouseFactsDataSet.FromTable(ReportingWarehouseFactsDataSet.FactPartnerRevenueTableName, GetFirstTable(source));
        }
    }
}
