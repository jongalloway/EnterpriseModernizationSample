using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub;
using Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Services
{
    public class PartnerAccountService : CustomerHubServiceBase, IPartnerAccountService
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
            return Execute(() => _partnerAccountRepository.GetPreferredPartners(), "GetPreferredPartners");
        }

        public PartnerProfile GetPartner(string partnerId)
        {
            return Execute(() => _partnerAccountRepository.GetPartner(partnerId), "GetPartner");
        }

        public PartnerProfile RegisterPartner(PartnerRegistrationRequest request)
        {
            return Execute(() => _partnerAccountRepository.RegisterPartner(request), "RegisterPartner");
        }

        public PartnerContractRecord UpdateContract(PartnerContractUpdateRequest request)
        {
            return Execute(() => _partnerAccountRepository.UpdateContract(request), "UpdateContract");
        }

        public PartnerReferralRecord[] GetReferrals(string partnerId)
        {
            return Execute(() => _partnerAccountRepository.GetReferrals(partnerId), "GetReferrals");
        }

        public CommissionProcessingResult ProcessCommission(CommissionProcessingRequest request)
        {
            return Execute(() => _partnerAccountRepository.ProcessCommission(request), "ProcessCommission");
        }

        public PartnerStatusRecord GetPartnerStatus(string partnerId)
        {
            return Execute(() =>
            {
                var partner = _partnerAccountRepository.GetPartner(partnerId);
                return new PartnerStatusRecord
                {
                    PartnerId = partner.PartnerId,
                    AccountCode = partner.AccountCode,
                    Status = partner.Status,
                    ContractCode = partner.ContractCode,
                    StatusUpdatedAtUtc = partner.StatusUpdatedAtUtc
                };
            }, "GetPartnerStatus");
        }

        public PartnerReferralRecord SubmitReferral(PartnerReferralSubmission request)
        {
            return Execute(() => _partnerAccountRepository.SubmitReferral(request), "SubmitReferral");
        }
    }
}
