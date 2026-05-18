using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IDispatchQueueService
    {
        IList<DispatchQueueEntry> BuildDispatchQueue(string storeNumber, IList<DispatchTicket> tickets, IList<RoutePlan> routePlans, IList<DriverAssignmentRecommendation> assignments);

        IList<DispatchBatch> BuildDispatchBatches(string storeNumber, IList<DispatchQueueEntry> queueEntries);
    }
}
