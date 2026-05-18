using System;
using Fabrikam.EnterprisePizza.Core.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IBusinessClock _businessClock;

        public OrderStatusService()
            : this(new SystemBusinessClock())
        {
        }

        public OrderStatusService(IBusinessClock businessClock)
        {
            _businessClock = businessClock ?? throw new ArgumentNullException(nameof(businessClock));
        }

        public OrderStatusChangeResult ChangeStatus(OrderProcessingResult order, OrderStatus requestedStatus, string changedBy)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var result = new OrderStatusChangeResult
            {
                PreviousStatus = order.Status,
                CurrentStatus = order.Status,
                Accepted = false
            };

            if (requestedStatus != order.Status + 1)
            {
                result.Message = string.Format("Orders can only advance to the next state. Requested {0} from {1}.", requestedStatus, order.Status);
                return result;
            }

            order.Status = requestedStatus;
            result.Accepted = true;
            result.CurrentStatus = requestedStatus;
            result.Message = string.Format("Order advanced from {0} to {1}.", result.PreviousStatus, requestedStatus);
            order.StatusHistory.Add(string.Format("{0:g} - {1} by {2}", _businessClock.GetCurrentTime(), requestedStatus, string.IsNullOrWhiteSpace(changedBy) ? "workflow" : changedBy));
            return result;
        }
    }
}
