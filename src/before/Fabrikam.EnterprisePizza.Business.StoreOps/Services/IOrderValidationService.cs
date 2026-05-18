using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IOrderValidationService
    {
        OrderValidationResult Validate(OrderRequest request);
    }
}
