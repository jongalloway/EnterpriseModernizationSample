using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Domain.CustomerHub;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Services
{
    public interface IPartnerAccountService
    {
        IList<string> GetPreferredPartners();

        IList<PartnerAccountSnapshot> GetPreferredPartnerSnapshots();
    }
}
