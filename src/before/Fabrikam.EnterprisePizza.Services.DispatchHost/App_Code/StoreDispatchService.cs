using System;
using System.Collections.Generic;
using System.ServiceModel;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Core.ExceptionHandling;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Services.DispatchHost
{
    [ServiceContract]
    public interface IStoreDispatchService
    {
        [OperationContract]
        IList<DispatchTicket> GetDispatchBoard(string storeNumber);

        [OperationContract]
        DispatchBoardSnapshot GetDispatchBoardSnapshot(StoreDispatchRequest request);
    }

    public class StoreDispatchService : IStoreDispatchService
    {
        private readonly IDispatchCoordinator _coordinator;

        public StoreDispatchService()
            : this(LegacyServiceLocator.Resolve<IDispatchCoordinator>())
        {
        }

        public StoreDispatchService(IDispatchCoordinator coordinator)
        {
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
        }

        public IList<DispatchTicket> GetDispatchBoard(string storeNumber)
        {
            try
            {
                return _coordinator.GetActiveTickets(storeNumber);
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        public DispatchBoardSnapshot GetDispatchBoardSnapshot(StoreDispatchRequest request)
        {
            try
            {
                var storeNumber = request == null || string.IsNullOrWhiteSpace(request.StoreNumber)
                    ? "014"
                    : request.StoreNumber;
                var tickets = _coordinator.GetActiveTickets(storeNumber);

                return new DispatchBoardSnapshot
                {
                    StoreNumber = storeNumber,
                    GeneratedAtUtc = DateTime.UtcNow,
                    SourceSystem = "StoreOps.DispatchBoard",
                    Tickets = new List<DispatchTicket>(tickets)
                };
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        private static Exception HandleServiceBoundaryException(Exception exception)
        {
            Exception exceptionToThrow;
            if (ExceptionPolicy.HandleException(exception, "ServiceBoundaryPolicy", out exceptionToThrow) && exceptionToThrow != null)
            {
                return exceptionToThrow;
            }

            return exception;
        }
    }
}
