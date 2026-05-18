using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Domain;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Data.Repositories.StoreOps
{
    public interface IOrderRepository
    {
        OrderRecord GetOrder(int orderNumber);

        IList<OrderLookupRecord> SearchOrders(OrderSearchRequest request);

        OrderRecord PlaceOrder(OrderPlacementRequest request, DeliveryPromise promise, DateTime submittedAtUtc);

        OrderRecord UpdateOrderStatus(OrderStatusUpdateRequest request, DateTime updatedAtUtc);

        IList<OrderHistoryRecord> GetOrderHistory(int orderNumber);
    }
}
