using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class DispatchCoordinator
    {
        public IList<DispatchTicket> GetActiveTickets(string storeNumber)
        {
            return new List<DispatchTicket>
            {
                new DispatchTicket
                {
                    TicketId = 4105,
                    StoreNumber = storeNumber,
                    DriverCode = "DRV-17",
                    RouteZone = "Northwest Corporate Corridor"
                },
                new DispatchTicket
                {
                    TicketId = 4106,
                    StoreNumber = storeNumber,
                    DriverCode = "DRV-03",
                    RouteZone = "Mall Annex"
                }
            };
        }
    }
}
