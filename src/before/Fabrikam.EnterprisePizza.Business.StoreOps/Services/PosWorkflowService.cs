using System;
using System.Linq;
using Fabrikam.EnterprisePizza.Core.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class PosWorkflowService : IPosWorkflowService
    {
        private readonly IOrderPricingEngine _pricingEngine;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IBusinessClock _businessClock;
        private readonly StoreOpsExceptionShield _exceptionShield;

        public PosWorkflowService()
            : this(new OrderPricingEngine(), new StubPaymentGateway(), new SystemBusinessClock(), new StoreOpsExceptionShield())
        {
        }

        public PosWorkflowService(IOrderPricingEngine pricingEngine, IPaymentGateway paymentGateway, IBusinessClock businessClock, StoreOpsExceptionShield exceptionShield)
        {
            _pricingEngine = pricingEngine ?? throw new ArgumentNullException(nameof(pricingEngine));
            _paymentGateway = paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));
            _businessClock = businessClock ?? throw new ArgumentNullException(nameof(businessClock));
            _exceptionShield = exceptionShield ?? throw new ArgumentNullException(nameof(exceptionShield));
        }

        public PosCart OpenCart(OrderRequest request)
        {
            return _exceptionShield.Execute(() =>
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var cart = new PosCart
                {
                    StoreNumber = request.StoreNumber,
                    CashierId = string.IsNullOrWhiteSpace(request.CashierId) ? "LANE-01" : request.CashierId,
                    CustomerName = request.CustomerName,
                    ServiceMode = request.ServiceMode,
                    PromoCode = request.PromoCode,
                    DeliveryZone = request.DeliveryZone,
                    OpenedAtLocal = request.RequestedFulfillmentTimeLocal == default(DateTime) ? _businessClock.GetCurrentTime() : request.RequestedFulfillmentTimeLocal,
                    PaymentMethod = request.PaymentMethod
                };

                foreach (var item in request.LineItems.Where(item => item != null))
                {
                    cart.Items.Add(Clone(item));
                }

                return UpdateTotals(cart);
            }, StoreOpsExceptionShield.PosWorkflowPolicy);
        }

        public PosCart AddItem(PosCart cart, OrderLineItem item)
        {
            return _exceptionShield.Execute(() =>
            {
                if (cart == null)
                {
                    throw new ArgumentNullException(nameof(cart));
                }

                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                var updated = Clone(cart);
                updated.Items.Add(Clone(item));
                return UpdateTotals(updated);
            }, StoreOpsExceptionShield.PosWorkflowPolicy);
        }

        public PosCart RemoveItem(PosCart cart, string menuItemCode, int quantity)
        {
            return _exceptionShield.Execute(() =>
            {
                if (cart == null)
                {
                    throw new ArgumentNullException(nameof(cart));
                }

                var updated = Clone(cart);
                var line = updated.Items.FirstOrDefault(item => string.Equals(item.MenuItemCode, menuItemCode, StringComparison.OrdinalIgnoreCase));
                if (line == null)
                {
                    return updated;
                }

                if (quantity <= 0 || quantity >= line.Quantity)
                {
                    updated.Items.Remove(line);
                }
                else
                {
                    line.Quantity -= quantity;
                }

                return UpdateTotals(updated);
            }, StoreOpsExceptionShield.PosWorkflowPolicy);
        }

        public PaymentProcessingResult ProcessPayment(PosCart cart, PaymentRequest request)
        {
            return _exceptionShield.Execute(() =>
            {
                if (cart == null)
                {
                    throw new ArgumentNullException(nameof(cart));
                }

                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var refreshedCart = UpdateTotals(Clone(cart));
                return _paymentGateway.Authorize(request, refreshedCart.EstimatedTotal);
            }, StoreOpsExceptionShield.PosWorkflowPolicy);
        }

        public PosReceipt GenerateReceipt(OrderProcessingResult order, PaymentProcessingResult paymentResult)
        {
            return _exceptionShield.Execute(() =>
            {
                if (order == null)
                {
                    throw new ArgumentNullException(nameof(order));
                }

                if (paymentResult == null)
                {
                    throw new ArgumentNullException(nameof(paymentResult));
                }

                var receipt = new PosReceipt
                {
                    OrderNumber = order.OrderNumber,
                    StoreNumber = order.StoreNumber,
                    ReceiptNumber = string.Format("{0}-{1}", order.StoreNumber, order.OrderNumber),
                    CustomerName = order.CustomerName,
                    CashierId = order.Cart == null ? string.Empty : order.Cart.CashierId,
                    GeneratedAtLocal = _businessClock.GetCurrentTime(),
                    Total = order.Pricing.Total
                };

                receipt.Lines.Add("Fabrikam Enterprise Pizza");
                receipt.Lines.Add(string.Format("Order #{0}", order.OrderNumber));
                receipt.Lines.Add(string.Format("Customer: {0}", order.CustomerName));
                receipt.Lines.Add(string.Format("Status: {0}", order.Status));
                receipt.Lines.Add(string.Format("Subtotal: {0:C}", order.Pricing.Subtotal));
                receipt.Lines.Add(string.Format("Discounts: {0:C}", order.Pricing.VolumeDiscount + order.Pricing.PromoDiscount));
                receipt.Lines.Add(string.Format("Tax: {0:C}", order.Pricing.TaxAmount));
                receipt.Lines.Add(string.Format("Total: {0:C}", order.Pricing.Total));
                receipt.Lines.Add(string.Format("Payment: {0} ({1})", paymentResult.PaymentMethod, paymentResult.StatusMessage));
                return receipt;
            }, StoreOpsExceptionShield.PosWorkflowPolicy);
        }

        private PosCart UpdateTotals(PosCart cart)
        {
            var pricing = _pricingEngine.Calculate(new OrderRequest
            {
                StoreNumber = cart.StoreNumber,
                CustomerName = cart.CustomerName,
                ServiceMode = cart.ServiceMode,
                DeliveryZone = cart.DeliveryZone,
                PromoCode = cart.PromoCode,
                RequestedFulfillmentTimeLocal = cart.OpenedAtLocal,
                PaymentMethod = cart.PaymentMethod,
                CashierId = cart.CashierId,
                LineItems = cart.Items.ToList()
            });

            cart.EstimatedSubtotal = pricing.Subtotal;
            cart.EstimatedTotal = pricing.Total;
            return cart;
        }

        private static PosCart Clone(PosCart cart)
        {
            var clone = new PosCart
            {
                StoreNumber = cart.StoreNumber,
                CashierId = cart.CashierId,
                CustomerName = cart.CustomerName,
                ServiceMode = cart.ServiceMode,
                PromoCode = cart.PromoCode,
                DeliveryZone = cart.DeliveryZone,
                OpenedAtLocal = cart.OpenedAtLocal,
                PaymentMethod = cart.PaymentMethod
            };

            foreach (var item in cart.Items.Where(item => item != null))
            {
                clone.Items.Add(Clone(item));
            }

            return clone;
        }

        private static OrderLineItem Clone(OrderLineItem item)
        {
            return new OrderLineItem
            {
                MenuItemCode = item.MenuItemCode,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };
        }
    }
}
