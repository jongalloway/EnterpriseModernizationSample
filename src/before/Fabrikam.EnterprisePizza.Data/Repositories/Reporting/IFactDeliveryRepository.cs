using System;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public interface IFactDeliveryRepository
    {
        ReportingWarehouseFactsDataSet GetDailyDeliveries(string storeNumber, DateTime summaryDate);
    }
}
