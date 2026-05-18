using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IOrderPricingEngine
    {
        OrderPricingResult Calculate(OrderRequest request);
    }
}
