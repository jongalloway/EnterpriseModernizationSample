using System.Collections.Generic;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public interface IPartnerAccountRepository
    {
        IList<string> GetPreferredPartners();
    }
}
