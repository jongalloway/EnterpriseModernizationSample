using System.Web;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.Repositories.StoreOps;

namespace Fabrikam.EnterprisePizza.Services.DispatchHost
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, System.EventArgs e)
        {
            LegacyServiceLocator.InitializeFromConfiguration("DispatchHost");
        }
    }
}
