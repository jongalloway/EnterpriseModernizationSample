using System;
using System.Linq;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class OrderValidationService : IOrderValidationService
    {
        private readonly IStoreOpsBusinessRuleProvider _businessRules;

        public OrderValidationService()
            : this(new StoreOpsBusinessRuleProvider())
        {
        }

        public OrderValidationService(IStoreOpsBusinessRuleProvider businessRules)
        {
            _businessRules = businessRules ?? throw new ArgumentNullException(nameof(businessRules));
        }

        public OrderValidationResult Validate(OrderRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OrderValidationResult();
            var requestedTime = request.RequestedFulfillmentTimeLocal == default(DateTime)
                ? DateTime.Now
                : request.RequestedFulfillmentTimeLocal;

            if (string.IsNullOrWhiteSpace(request.StoreNumber))
            {
                result.Issues.Add(new OrderValidationIssue { Code = "store-number", Message = "Store number is required before the order can enter StoreOps." });
            }

            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                result.Issues.Add(new OrderValidationIssue { Code = "customer-name", Message = "Customer name is required for kitchen and dispatch visibility." });
            }

            if (request.LineItems == null || request.LineItems.Count == 0)
            {
                result.Issues.Add(new OrderValidationIssue { Code = "line-items", Message = "At least one menu item is required to build an order." });
            }
            else
            {
                foreach (var item in request.LineItems.Where(item => item != null))
                {
                    if (string.IsNullOrWhiteSpace(item.MenuItemCode))
                    {
                        result.Issues.Add(new OrderValidationIssue { Code = "item-code", Message = "Every order line needs a menu item code." });
                        continue;
                    }

                    if (item.Quantity <= 0)
                    {
                        result.Issues.Add(new OrderValidationIssue { Code = "quantity", Message = "Line item quantities must be greater than zero." });
                    }

                    decimal configuredPrice;
                    if (!_businessRules.TryGetConfiguredPrice(request.StoreNumber, item.MenuItemCode, out configuredPrice) && item.UnitPrice <= 0m)
                    {
                        result.Issues.Add(new OrderValidationIssue
                        {
                            Code = "menu-price",
                            Message = string.Format("No configured price was found for menu item {0}.", item.MenuItemCode)
                        });
                    }
                }
            }

            if (IsDelivery(request) && !_businessRules.IsDeliveryZoneAllowed(request.DeliveryZone))
            {
                result.Issues.Add(new OrderValidationIssue { Code = "delivery-zone", Message = "Delivery address falls outside the configured service zones for this store." });
            }

            if (!_businessRules.IsWithinStoreHours(requestedTime))
            {
                result.Issues.Add(new OrderValidationIssue { Code = "store-hours", Message = "Requested promise time falls outside configured store hours." });
            }

            var promo = _businessRules.GetPromo(request.PromoCode);
            if (!string.IsNullOrWhiteSpace(request.PromoCode) && promo == null)
            {
                result.Issues.Add(new OrderValidationIssue { Code = "promo-code", Message = "Promo code is not active in the configuration catalog." });
            }

            var estimatedSubtotal = EstimateSubtotal(request);
            if (IsDelivery(request) && estimatedSubtotal < _businessRules.GetMinimumOrder(request.StoreNumber))
            {
                result.Issues.Add(new OrderValidationIssue
                {
                    Code = "minimum-order",
                    Message = string.Format("Delivery orders for store {0} must meet the configured minimum of {1:C}.", request.StoreNumber, _businessRules.GetMinimumOrder(request.StoreNumber))
                });
            }

            result.IsValid = result.Issues.Count == 0;
            return result;
        }

        private decimal EstimateSubtotal(OrderRequest request)
        {
            if (request.LineItems == null)
            {
                return 0m;
            }

            return request.LineItems.Where(item => item != null)
                .Sum(item => ResolveUnitPrice(request.StoreNumber, item) * item.Quantity);
        }

        private decimal ResolveUnitPrice(string storeNumber, OrderLineItem item)
        {
            decimal configuredPrice;
            if (_businessRules.TryGetConfiguredPrice(storeNumber, item.MenuItemCode, out configuredPrice))
            {
                return configuredPrice;
            }

            return item.UnitPrice < 0m ? 0m : item.UnitPrice;
        }

        private static bool IsDelivery(OrderRequest request)
        {
            return string.Equals(request.ServiceMode, "Delivery", StringComparison.OrdinalIgnoreCase);
        }
    }
}
