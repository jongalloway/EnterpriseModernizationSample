using System.Collections.Generic;
using System.ServiceModel;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Services.DispatchHost
{
    [ServiceContract]
    public interface IStoreDispatchService
    {
        [OperationContract]
        IList<DispatchTicket> GetDispatchBoard(string storeNumber);
    }

    public class StoreDispatchService : IStoreDispatchService
    {
        private readonly DispatchCoordinator _coordinator = new DispatchCoordinator();

        public IList<DispatchTicket> GetDispatchBoard(string storeNumber)
        {
            return _coordinator.GetActiveTickets(storeNumber);
        }
    }
}
