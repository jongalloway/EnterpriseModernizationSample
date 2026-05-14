using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;

namespace Fabrikam.EnterprisePizza.Services.PartnerSync
{
    [WebService(Namespace = "http://fabrikam.com/pizza/partners/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement, ParameterStyle = SoapParameterStyle.Wrapped)]
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
