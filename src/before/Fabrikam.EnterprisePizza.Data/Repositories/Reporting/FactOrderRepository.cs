using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public class FactOrderRepository : ReportingRepositoryBase, IFactOrderRepository
    {
        public FactOrderRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public FactOrderRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
            : base(gateway, connectionCatalog)
        {
        }

        public ReportingWarehouseFactsDataSet GetDailyOrders(string storeNumber, DateTime summaryDate)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetFactOrderByDate,
                new GatewayParameter("@StoreNumber", storeNumber),
                new GatewayParameter("@SummaryDate", summaryDate));

            return ReportingWarehouseFactsDataSet.FromTable(ReportingWarehouseFactsDataSet.FactOrderTableName, GetFirstTable(source));
        }
    }
}
