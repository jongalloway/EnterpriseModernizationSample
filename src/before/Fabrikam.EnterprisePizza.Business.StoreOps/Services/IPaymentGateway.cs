using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IPaymentGateway
    {
        PaymentProcessingResult Authorize(PaymentRequest request, decimal totalAmount);
    }
}
