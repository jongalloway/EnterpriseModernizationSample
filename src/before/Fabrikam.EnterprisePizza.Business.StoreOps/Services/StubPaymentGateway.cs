using System;
using Fabrikam.EnterprisePizza.Core.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class StubPaymentGateway : IPaymentGateway
    {
        private readonly IBusinessClock _businessClock;

        public StubPaymentGateway()
            : this(new SystemBusinessClock())
        {
        }

        public StubPaymentGateway(IBusinessClock businessClock)
        {
            _businessClock = businessClock ?? throw new ArgumentNullException(nameof(businessClock));
        }

        public PaymentProcessingResult Authorize(PaymentRequest request, decimal totalAmount)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var amountAuthorized = request.AmountTendered > 0m ? request.AmountTendered : totalAmount;
            var approved = request.Method != PaymentMethod.Cash || amountAuthorized >= totalAmount;
            var statusMessage = approved ? "Legacy payment host approved the transaction." : "Cash tender was lower than the cart total.";

            return new PaymentProcessingResult
            {
                Approved = approved,
                ApprovalCode = approved ? string.Format("AUTH-{0}", _businessClock.GetCurrentTime().ToString("HHmmss")) : string.Empty,
                StatusMessage = statusMessage,
                AmountAuthorized = approved ? totalAmount : 0m,
                ProcessedAtLocal = _businessClock.GetCurrentTime(),
                PaymentMethod = request.Method
            };
        }
    }
}
