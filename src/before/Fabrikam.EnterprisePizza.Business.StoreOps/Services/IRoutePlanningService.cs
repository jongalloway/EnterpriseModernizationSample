using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IRoutePlanningService
    {
        IList<RoutePlan> BuildRoutePlans(string storeNumber, IList<DispatchTicket> tickets);
    }
}
