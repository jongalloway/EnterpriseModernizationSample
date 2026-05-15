using System;
using Fabrikam.EnterprisePizza.Core.Domain.Reporting;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.Repositories.Reporting;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class WorkforceReportingService : IWorkforceReportingService
    {
        private readonly IWorkforceReportRepository workforceReportRepository;

        public WorkforceReportingService()
            : this(new WorkforceReportRepository(new LegacyDbGateway(new LegacyConnectionCatalog())))
        {
        }

        public WorkforceReportingService(IWorkforceReportRepository workforceReportRepository)
        {
            this.workforceReportRepository = workforceReportRepository ?? throw new ArgumentNullException(nameof(workforceReportRepository));
        }

        public WorkforceReportSnapshot GetSnapshot(string storeNumber, DateTime summaryDate)
        {
            var snapshot = new WorkforceReportSnapshot
            {
                LaborCost = workforceReportRepository.GetLaborCostSummary(storeNumber, summaryDate),
                Turnover = workforceReportRepository.GetTurnoverSummary(storeNumber, new DateTime(summaryDate.Year, summaryDate.Month, 1)),
                Staffing = workforceReportRepository.GetStaffingSummary(storeNumber, summaryDate)
            };

            foreach (var point in workforceReportRepository.GetOvertimeTrend(storeNumber, 4))
            {
                snapshot.OvertimeTrend.Add(point);
            }

            return snapshot;
        }
    }
}
