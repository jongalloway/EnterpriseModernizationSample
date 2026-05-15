using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Data.Repositories.StoreOps;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class DispatchCoordinator : IDispatchCoordinator
    {
        private readonly IDispatchTicketRepository _dispatchTicketRepository;

        public DispatchCoordinator()
            : this(new DispatchTicketRepository())
        {
        }

        public DispatchCoordinator(IDispatchTicketRepository dispatchTicketRepository)
        {
            _dispatchTicketRepository = dispatchTicketRepository ?? throw new ArgumentNullException(nameof(dispatchTicketRepository));
        }

        public IList<DispatchTicket> GetActiveTickets(string storeNumber)
        {
            return _dispatchTicketRepository.GetActiveTickets(storeNumber);
        }
    }
}
