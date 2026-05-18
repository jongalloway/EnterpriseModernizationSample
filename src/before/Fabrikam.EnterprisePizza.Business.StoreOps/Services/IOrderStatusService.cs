using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IOrderStatusService
    {
        OrderStatusChangeResult ChangeStatus(OrderProcessingResult order, OrderStatus requestedStatus, string changedBy);
    }
}
