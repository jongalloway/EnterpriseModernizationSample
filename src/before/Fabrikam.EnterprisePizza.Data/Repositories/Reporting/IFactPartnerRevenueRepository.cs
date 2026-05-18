using System;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public interface IFactPartnerRevenueRepository
    {
        ReportingWarehouseFactsDataSet GetMonthlyPartnerRevenue(string partnerCode, DateTime summaryMonth);
    }
}
