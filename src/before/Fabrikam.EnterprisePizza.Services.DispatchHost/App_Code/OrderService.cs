using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Core.ExceptionHandling;
using Fabrikam.EnterprisePizza.Core.Logging;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Services.DispatchHost
{
    [ServiceContract]
    public interface IOrderService
    {
        [OperationContract]
        OrderRecord GetOrder(int orderNumber);

        [OperationContract]
        IList<OrderLookupRecord> SearchOrders(OrderSearchRequest request);

        [OperationContract]
        OrderRecord PlaceOrder(OrderPlacementRequest request);

        [OperationContract]
        OrderRecord UpdateOrderStatus(OrderStatusUpdateRequest request);

        [OperationContract]
        IList<OrderHistoryRecord> GetOrderHistory(int orderNumber);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderManagementService orderManagementService;

        public OrderService()
            : this(LegacyServiceLocator.Resolve<IOrderManagementService>())
        {
        }

        public OrderService(IOrderManagementService orderManagementService)
        {
            this.orderManagementService = orderManagementService ?? throw new ArgumentNullException(nameof(orderManagementService));
        }

        public OrderRecord GetOrder(int orderNumber)
        {
            try
            {
                LogWriter.Write("Order lookup requested for order " + orderNumber + ".", "OrderService", TraceEventType.Information, null);
                var order = orderManagementService.GetOrder(orderNumber);
                LogWriter.Write(order == null
                    ? "Order " + orderNumber + " was not found in StoreOps."
                    : "Order " + orderNumber + " loaded for store " + order.StoreNumber + ".", "OrderService", TraceEventType.Information, null);
                return order;
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        public IList<OrderLookupRecord> SearchOrders(OrderSearchRequest request)
        {
            try
            {
                var storeNumber = request == null || string.IsNullOrWhiteSpace(request.StoreNumber) ? "014" : request.StoreNumber.Trim().ToUpperInvariant();
                LogWriter.Write("Order search requested for store " + storeNumber + ".", "OrderService", TraceEventType.Information, null);
                var results = orderManagementService.SearchOrders(request);
                LogWriter.Write("Order search returned " + results.Count + " rows for store " + storeNumber + ".", "OrderService", TraceEventType.Information, null);
                return results;
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        public OrderRecord PlaceOrder(OrderPlacementRequest request)
        {
            try
            {
                var storeNumber = request == null || string.IsNullOrWhiteSpace(request.StoreNumber) ? "014" : request.StoreNumber.Trim().ToUpperInvariant();
                LogWriter.Write("PlaceOrder requested for store " + storeNumber + ".", "OrderService", TraceEventType.Information, null);
                var order = orderManagementService.PlaceOrder(request);
                LogWriter.Write("Order " + order.OrderNumber + " placed for store " + order.StoreNumber + ".", "OrderService", TraceEventType.Information, null);
                return order;
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        public OrderRecord UpdateOrderStatus(OrderStatusUpdateRequest request)
        {
            try
            {
                var statusCode = request == null || string.IsNullOrWhiteSpace(request.StatusCode) ? "Unknown" : request.StatusCode.Trim();
                LogWriter.Write("Status update requested for order " + (request == null ? 0 : request.OrderNumber) + " to " + statusCode + ".", "OrderService", TraceEventType.Information, null);
                var order = orderManagementService.UpdateOrderStatus(request);
                LogWriter.Write("Order " + order.OrderNumber + " now reports status " + order.OrderStatus + ".", "OrderService", TraceEventType.Information, null);
                return order;
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        public IList<OrderHistoryRecord> GetOrderHistory(int orderNumber)
        {
            try
            {
                LogWriter.Write("Order history requested for order " + orderNumber + ".", "OrderService", TraceEventType.Information, null);
                var history = orderManagementService.GetOrderHistory(orderNumber);
                LogWriter.Write("Order history returned " + history.Count + " entries for order " + orderNumber + ".", "OrderService", TraceEventType.Information, null);
                return history;
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
