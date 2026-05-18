using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IOrderProcessingService
    {
        OrderProcessingResult CreateOrder(OrderRequest request);

        OrderStatusChangeResult UpdateStatus(OrderProcessingResult order, OrderStatus requestedStatus, string changedBy);
    }
}
