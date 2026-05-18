using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Domain;
using Fabrikam.EnterprisePizza.Core.Services;
using Fabrikam.EnterprisePizza.Data.Repositories.StoreOps;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class OrderManagementService : IOrderManagementService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IOrderFulfillmentPolicy fulfillmentPolicy;
        private readonly IBusinessClock businessClock;

        public OrderManagementService()
            : this(new OrderRepository(), new LegacyOrderFulfillmentPolicy(new SystemBusinessClock()), new SystemBusinessClock())
        {
        }

        public OrderManagementService(IOrderRepository orderRepository, IOrderFulfillmentPolicy fulfillmentPolicy, IBusinessClock businessClock)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.fulfillmentPolicy = fulfillmentPolicy ?? throw new ArgumentNullException(nameof(fulfillmentPolicy));
            this.businessClock = businessClock ?? throw new ArgumentNullException(nameof(businessClock));
        }

        public OrderRecord GetOrder(int orderNumber)
        {
            if (orderNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(orderNumber), "Order numbers must be positive.");
            }

            return orderRepository.GetOrder(orderNumber);
        }

        public IList<OrderLookupRecord> SearchOrders(OrderSearchRequest request)
        {
            return orderRepository.SearchOrders(NormalizeSearchRequest(request));
        }

        public OrderRecord PlaceOrder(OrderPlacementRequest request)
        {
            var normalizedRequest = NormalizePlacementRequest(request);
            var submittedAtUtc = businessClock.GetCurrentTime().ToUniversalTime();
            var promise = fulfillmentPolicy.CreatePromise(new OrderSummary
            {
                OrderId = 0,
                StoreNumber = normalizedRequest.StoreNumber,
                CustomerDisplayName = normalizedRequest.CustomerName,
                Channel = ResolveChannel(normalizedRequest.Channel),
                OrderTotal = normalizedRequest.TicketTotal,
                IsCorporateAccount = normalizedRequest.IsCorporateAccount,
                DeliveryMileage = normalizedRequest.DeliveryMileage,
                NeededBy = normalizedRequest.NeededByUtc == DateTime.MinValue
                    ? submittedAtUtc.AddMinutes(35)
                    : normalizedRequest.NeededByUtc.ToUniversalTime()
            });

            normalizedRequest.NeededByUtc = normalizedRequest.NeededByUtc == DateTime.MinValue
                ? promise.QuotedReadyTime.ToUniversalTime()
                : normalizedRequest.NeededByUtc.ToUniversalTime();

            return orderRepository.PlaceOrder(normalizedRequest, promise, submittedAtUtc);
        }

        public OrderRecord UpdateOrderStatus(OrderStatusUpdateRequest request)
        {
            var normalizedRequest = NormalizeStatusUpdateRequest(request);
            return orderRepository.UpdateOrderStatus(normalizedRequest, businessClock.GetCurrentTime().ToUniversalTime());
        }

        public IList<OrderHistoryRecord> GetOrderHistory(int orderNumber)
        {
            if (orderNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(orderNumber), "Order numbers must be positive.");
            }

            return orderRepository.GetOrderHistory(orderNumber);
        }

        private static OrderSearchRequest NormalizeSearchRequest(OrderSearchRequest request)
        {
            return new OrderSearchRequest
            {
                StoreNumber = NormalizeStoreNumber(request == null ? null : request.StoreNumber),
                SearchText = request == null || string.IsNullOrWhiteSpace(request.SearchText) ? string.Empty : request.SearchText.Trim(),
                StatusCode = request == null || string.IsNullOrWhiteSpace(request.StatusCode) ? string.Empty : request.StatusCode.Trim(),
                FromSubmittedUtc = request == null ? DateTime.MinValue : request.FromSubmittedUtc,
                ToSubmittedUtc = request == null ? DateTime.MinValue : request.ToSubmittedUtc,
                IncludeClosed = request != null && request.IncludeClosed
            };
        }

        private static OrderPlacementRequest NormalizePlacementRequest(OrderPlacementRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.TicketTotal <= 0m)
            {
                throw new InvalidOperationException("Ticket totals must be greater than zero before the order can be placed.");
            }

            return new OrderPlacementRequest
            {
                StoreNumber = NormalizeStoreNumber(request.StoreNumber),
                CustomerName = string.IsNullOrWhiteSpace(request.CustomerName) ? "Walk-Up Guest" : request.CustomerName.Trim(),
                Channel = string.IsNullOrWhiteSpace(request.Channel) ? "Web" : request.Channel.Trim(),
                ServiceMode = string.IsNullOrWhiteSpace(request.ServiceMode) ? "Delivery" : request.ServiceMode.Trim(),
                TicketTotal = request.TicketTotal,
                IsCorporateAccount = request.IsCorporateAccount,
                DeliveryMileage = request.DeliveryMileage < 0m ? 0m : request.DeliveryMileage,
                NeededByUtc = request.NeededByUtc,
                DeliveryAddress = string.IsNullOrWhiteSpace(request.DeliveryAddress) ? "Front Desk Pickup" : request.DeliveryAddress.Trim(),
                SpecialInstructions = string.IsNullOrWhiteSpace(request.SpecialInstructions) ? string.Empty : request.SpecialInstructions.Trim()
            };
        }

        private static OrderStatusUpdateRequest NormalizeStatusUpdateRequest(OrderStatusUpdateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.OrderNumber <= 0)
            {
                throw new InvalidOperationException("A valid order number is required before the order status can be updated.");
            }

            if (string.IsNullOrWhiteSpace(request.StatusCode))
            {
                throw new InvalidOperationException("A target status code is required before the order status can be updated.");
            }

            return new OrderStatusUpdateRequest
            {
                OrderNumber = request.OrderNumber,
                StoreNumber = NormalizeStoreNumber(request.StoreNumber),
                StatusCode = request.StatusCode.Trim(),
                UpdatedBy = string.IsNullOrWhiteSpace(request.UpdatedBy) ? "Dispatch Console" : request.UpdatedBy.Trim(),
                StatusNote = string.IsNullOrWhiteSpace(request.StatusNote) ? string.Empty : request.StatusNote.Trim()
            };
        }

        private static string NormalizeStoreNumber(string storeNumber)
        {
            return string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
        }

        private static OrderChannel ResolveChannel(string channel)
        {
            var normalizedChannel = string.IsNullOrWhiteSpace(channel)
                ? string.Empty
                : channel.Trim().Replace(" ", string.Empty).ToUpperInvariant();

            switch (normalizedChannel)
            {
                case "PHONE":
                case "CALLCENTER":
                    return OrderChannel.PhoneQueue;
                case "CUSTOMERHUB":
                case "CUSTOMERHUBDESK":
                    return OrderChannel.CustomerHubDesk;
                case "FRANCHISE":
                case "FRANCHISEPORTAL":
                    return OrderChannel.FranchisePortal;
                case "CORPORATE":
                case "CORPORATECATERING":
                case "CATERING":
                    return OrderChannel.CorporateCatering;
                default:
                    return OrderChannel.Storefront;
            }
        }
    }
}
