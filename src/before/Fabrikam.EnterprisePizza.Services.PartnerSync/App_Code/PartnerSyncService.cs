using System.Collections.Generic;
using System.Web.Services;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;

namespace Fabrikam.EnterprisePizza.Services.PartnerSync
{
    [WebService(Namespace = "http://fabrikam.com/pizza/partners/")]
    public class PartnerSyncService : WebService
    {
        private readonly PartnerAccountService _partnerAccountService = new PartnerAccountService();

        [WebMethod]
        public IList<string> GetPreferredPartners()
        {
            return _partnerAccountService.GetPreferredPartners();
        }
    }
}
