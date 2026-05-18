using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public class FactDeliveryRepository : ReportingRepositoryBase, IFactDeliveryRepository
    {
        public FactDeliveryRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public FactDeliveryRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
            : base(gateway, connectionCatalog)
        {
        }

        public ReportingWarehouseFactsDataSet GetDailyDeliveries(string storeNumber, DateTime summaryDate)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetFactDeliveryByDate,
                new GatewayParameter("@StoreNumber", storeNumber),
                new GatewayParameter("@SummaryDate", summaryDate));

            return ReportingWarehouseFactsDataSet.FromTable(ReportingWarehouseFactsDataSet.FactDeliveryTableName, GetFirstTable(source));
        }
    }
}
