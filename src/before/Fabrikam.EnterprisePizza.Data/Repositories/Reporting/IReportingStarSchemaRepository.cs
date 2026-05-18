using System;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public interface IReportingStarSchemaRepository
    {
        ReportingWarehouseStarSchemaDataSet GetDeliveryStoreDriverSnapshot(string storeNumber, DateTime summaryDate);

        ReportingWarehouseStarSchemaDataSet GetOrderChannelMixSnapshot(string storeNumber, DateTime summaryDate);

        ReportingWarehouseStarSchemaDataSet GetPartnerRevenueSettlementSnapshot(string partnerCode, DateTime summaryMonth);
    }
}
