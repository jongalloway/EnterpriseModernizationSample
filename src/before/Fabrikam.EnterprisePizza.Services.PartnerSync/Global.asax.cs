using System.Web;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub;

namespace Fabrikam.EnterprisePizza.Services.PartnerSync
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, System.EventArgs e)
        {
            LegacyServiceLocator.Initialize(registry =>
            {
                registry.Register<LegacyDbGateway>();
                registry.Register<IPartnerAccountRepository, PartnerAccountRepository>();
                registry.Register<IPartnerAccountService, PartnerAccountService>();
            });
        }
    }
}
