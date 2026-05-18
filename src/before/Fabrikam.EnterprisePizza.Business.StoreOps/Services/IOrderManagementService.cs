using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IOrderManagementService
    {
        OrderRecord GetOrder(int orderNumber);

        IList<OrderLookupRecord> SearchOrders(OrderSearchRequest request);

        OrderRecord PlaceOrder(OrderPlacementRequest request);

        OrderRecord UpdateOrderStatus(OrderStatusUpdateRequest request);

        IList<OrderHistoryRecord> GetOrderHistory(int orderNumber);
    }
}
