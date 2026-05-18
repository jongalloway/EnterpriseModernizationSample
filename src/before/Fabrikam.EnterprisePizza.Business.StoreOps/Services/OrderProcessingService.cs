using System;
using System.Threading;
using Fabrikam.EnterprisePizza.Core.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class OrderProcessingService : IOrderProcessingService
    {
        private static int _lastOrderNumber = 72000;

        private readonly IOrderValidationService _validationService;
        private readonly IOrderPricingEngine _pricingEngine;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IPosWorkflowService _posWorkflowService;
        private readonly IBusinessClock _businessClock;
        private readonly StoreOpsExceptionShield _exceptionShield;

        public OrderProcessingService()
            : this(CreateValidationService(), CreatePricingEngine(), CreateStatusService(), CreatePosWorkflowService(), new SystemBusinessClock(), new StoreOpsExceptionShield())
        {
        }

        public OrderProcessingService(
            IOrderValidationService validationService,
            IOrderPricingEngine pricingEngine,
            IOrderStatusService orderStatusService,
            IPosWorkflowService posWorkflowService,
            IBusinessClock businessClock,
            StoreOpsExceptionShield exceptionShield)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _pricingEngine = pricingEngine ?? throw new ArgumentNullException(nameof(pricingEngine));
            _orderStatusService = orderStatusService ?? throw new ArgumentNullException(nameof(orderStatusService));
            _posWorkflowService = posWorkflowService ?? throw new ArgumentNullException(nameof(posWorkflowService));
            _businessClock = businessClock ?? throw new ArgumentNullException(nameof(businessClock));
            _exceptionShield = exceptionShield ?? throw new ArgumentNullException(nameof(exceptionShield));
        }

        public OrderProcessingResult CreateOrder(OrderRequest request)
        {
            return _exceptionShield.Execute(() =>
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var submittedAt = request.RequestedFulfillmentTimeLocal == default(DateTime)
                    ? _businessClock.GetCurrentTime()
                    : request.RequestedFulfillmentTimeLocal;
                var validation = _validationService.Validate(request);
                var canCalculatePricing = CanCalculatePricing(validation);
                var pricing = canCalculatePricing ? _pricingEngine.Calculate(request) : new OrderPricingResult();
                var order = new OrderProcessingResult
                {
                    Accepted = validation.IsValid,
                    OrderNumber = Interlocked.Increment(ref _lastOrderNumber),
                    StoreNumber = request.StoreNumber,
                    CustomerName = request.CustomerName,
                    ServiceMode = request.ServiceMode,
                    PromoCode = request.PromoCode,
                    Status = OrderStatus.Pending,
                    SubmittedAtLocal = submittedAt,
                    Validation = validation,
                    Pricing = pricing,
                    Cart = canCalculatePricing ? _posWorkflowService.OpenCart(request) : null
                };

                order.StatusHistory.Add(string.Format("{0:g} - Pending by intake", submittedAt));
                order.StatusHistory.Add(validation.IsValid
                    ? "Order cleared validation, pricing, and cart assembly."
                    : "Order created in pending review because validation reported issues.");

                return order;
            }, StoreOpsExceptionShield.OrderProcessingPolicy);
        }

        public OrderStatusChangeResult UpdateStatus(OrderProcessingResult order, OrderStatus requestedStatus, string changedBy)
        {
            return _exceptionShield.Execute(() => _orderStatusService.ChangeStatus(order, requestedStatus, changedBy), StoreOpsExceptionShield.OrderStatusPolicy);
        }

        private static bool CanCalculatePricing(OrderValidationResult validation)
        {
            if (validation == null || validation.Issues == null)
            {
                return true;
            }

            foreach (var issue in validation.Issues)
            {
                if (issue == null)
                {
                    continue;
                }

                switch (issue.Code)
                {
                    case "item-code":
                    case "line-items":
                    case "menu-price":
                    case "quantity":
                        return false;
                }
            }

            return true;
        }

        private static IOrderValidationService CreateValidationService()
        {
            var rules = new StoreOpsBusinessRuleProvider();
            return new OrderValidationService(rules);
        }

        private static IOrderPricingEngine CreatePricingEngine()
        {
            var rules = new StoreOpsBusinessRuleProvider();
            return new OrderPricingEngine(rules);
        }

        private static IOrderStatusService CreateStatusService()
        {
            return new OrderStatusService(new SystemBusinessClock());
        }

        private static IPosWorkflowService CreatePosWorkflowService()
        {
            var pricing = CreatePricingEngine();
            var clock = new SystemBusinessClock();
            return new PosWorkflowService(pricing, new StubPaymentGateway(clock), clock, new StoreOpsExceptionShield());
        }
    }
}
