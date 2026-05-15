using System;
using Fabrikam.EnterprisePizza.Core.Domain.Reporting;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IWorkforceReportingService
    {
        WorkforceReportSnapshot GetSnapshot(string storeNumber, DateTime summaryDate);
    }
}
