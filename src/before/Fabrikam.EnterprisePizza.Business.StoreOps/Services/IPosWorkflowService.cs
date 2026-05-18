using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IPosWorkflowService
    {
        PosCart OpenCart(OrderRequest request);

        PosCart AddItem(PosCart cart, OrderLineItem item);

        PosCart RemoveItem(PosCart cart, string menuItemCode, int quantity);

        PaymentProcessingResult ProcessPayment(PosCart cart, PaymentRequest request);

        PosReceipt GenerateReceipt(OrderProcessingResult order, PaymentProcessingResult paymentResult);
    }
}
