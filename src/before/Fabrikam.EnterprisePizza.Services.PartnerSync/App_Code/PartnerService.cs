using System;
using System.ServiceModel;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync;

namespace Fabrikam.EnterprisePizza.Services.PartnerSync
{
    [ServiceContract(Namespace = "http://fabrikam.com/pizza/services/partners/2008/04")]
    public interface IPartnerService
    {
        [OperationContract]
        PartnerProfile GetPartner(string partnerId);

        [OperationContract]
        PartnerProfile RegisterPartner(PartnerRegistrationRequest request);

        [OperationContract]
        PartnerContractRecord UpdateContract(PartnerContractUpdateRequest request);

        [OperationContract]
        PartnerReferralRecord[] GetReferrals(string partnerId);

        [OperationContract]
        CommissionProcessingResult ProcessCommission(CommissionProcessingRequest request);
    }

    public class PartnerService : IPartnerService
    {
        private readonly IPartnerAccountService _partnerAccountService;

        public PartnerService()
            : this(LegacyServiceLocator.Resolve<IPartnerAccountService>())
        {
        }

        public PartnerService(IPartnerAccountService partnerAccountService)
        {
            _partnerAccountService = partnerAccountService ?? throw new ArgumentNullException(nameof(partnerAccountService));
        }

        public PartnerProfile GetPartner(string partnerId)
        {
            return _partnerAccountService.GetPartner(partnerId);
        }

        public PartnerProfile RegisterPartner(PartnerRegistrationRequest request)
        {
            return _partnerAccountService.RegisterPartner(request);
        }

        public PartnerContractRecord UpdateContract(PartnerContractUpdateRequest request)
        {
            return _partnerAccountService.UpdateContract(request);
        }

        public PartnerReferralRecord[] GetReferrals(string partnerId)
        {
            return _partnerAccountService.GetReferrals(partnerId);
        }

        public CommissionProcessingResult ProcessCommission(CommissionProcessingRequest request)
        {
            return _partnerAccountService.ProcessCommission(request);
        }
    }
}
