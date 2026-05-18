using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Domain.CustomerHub;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public interface IPartnerAccountRepository
    {
        IList<string> GetPreferredPartners();

        IList<PartnerAccountSnapshot> GetPreferredPartnerSnapshots();
    }
}
