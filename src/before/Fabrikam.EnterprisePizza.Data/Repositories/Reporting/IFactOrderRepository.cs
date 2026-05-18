using System;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public interface IFactOrderRepository
    {
        ReportingWarehouseFactsDataSet GetDailyOrders(string storeNumber, DateTime summaryDate);
    }
}
