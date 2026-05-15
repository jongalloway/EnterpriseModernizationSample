using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Data.Repositories.StoreOps
{
    public interface IDispatchTicketRepository
    {
        IList<DispatchTicket> GetActiveTickets(string storeNumber);
    }
}
