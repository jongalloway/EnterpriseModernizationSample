using Fabrikam.EnterprisePizza.Core.Domain;

namespace Fabrikam.EnterprisePizza.Core.Services
{
    public interface IOrderFulfillmentPolicy
    {
        DeliveryPromise CreatePromise(OrderSummary order);
    }
}
