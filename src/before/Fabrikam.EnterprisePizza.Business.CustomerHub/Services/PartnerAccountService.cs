using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Services
{
    public class PartnerAccountService : IPartnerAccountService
    {
        private readonly IPartnerAccountRepository _partnerAccountRepository;

        public PartnerAccountService()
            : this(new PartnerAccountRepository())
        {
        }

        public PartnerAccountService(IPartnerAccountRepository partnerAccountRepository)
        {
            _partnerAccountRepository = partnerAccountRepository ?? throw new ArgumentNullException(nameof(partnerAccountRepository));
        }

        public IList<string> GetPreferredPartners()
        {
            return _partnerAccountRepository.GetPreferredPartners();
        }
    }
}
