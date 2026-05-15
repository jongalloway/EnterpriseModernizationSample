using System.Collections.Generic;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Services
{
    public interface IPartnerAccountService
    {
        IList<string> GetPreferredPartners();
    }
}
