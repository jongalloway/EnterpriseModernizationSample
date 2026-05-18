using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public class ReportingStarSchemaRepository : ReportingRepositoryBase, IReportingStarSchemaRepository
    {
        public ReportingStarSchemaRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public ReportingStarSchemaRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
            : base(gateway, connectionCatalog)
        {
        }

        public ReportingWarehouseStarSchemaDataSet GetDeliveryStoreDriverSnapshot(string storeNumber, DateTime summaryDate)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetDeliveryStoreDriverSnapshot,
                new GatewayParameter("@StoreNumber", storeNumber),
                new GatewayParameter("@SummaryDate", summaryDate));

            return ReportingWarehouseStarSchemaDataSet.FromTable(ReportingWarehouseStarSchemaDataSet.DeliveryStoreDriverSnapshotTableName, GetFirstTable(source));
        }

        public ReportingWarehouseStarSchemaDataSet GetOrderChannelMixSnapshot(string storeNumber, DateTime summaryDate)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetOrderChannelMixSnapshot,
                new GatewayParameter("@StoreNumber", storeNumber),
                new GatewayParameter("@SummaryDate", summaryDate));

            return ReportingWarehouseStarSchemaDataSet.FromTable(ReportingWarehouseStarSchemaDataSet.OrderChannelMixSnapshotTableName, GetFirstTable(source));
        }

        public ReportingWarehouseStarSchemaDataSet GetPartnerRevenueSettlementSnapshot(string partnerCode, DateTime summaryMonth)
        {
            var source = ExecuteReportingDataSet(
                LegacyStoredProcedures.Reporting.GetPartnerRevenueSettlementSnapshot,
                new GatewayParameter("@PartnerCode", partnerCode),
                new GatewayParameter("@SummaryMonth", summaryMonth));

            return ReportingWarehouseStarSchemaDataSet.FromTable(ReportingWarehouseStarSchemaDataSet.PartnerRevenueSettlementSnapshotTableName, GetFirstTable(source));
        }
    }
}
