using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IDispatchCoordinator
    {
        IList<DispatchTicket> GetActiveTickets(string storeNumber);
    }
}
