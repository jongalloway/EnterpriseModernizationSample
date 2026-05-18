using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Services
{
    public interface IPartnerAccountService
    {
        IList<string> GetPreferredPartners();

        PartnerProfile GetPartner(string partnerId);

        PartnerProfile RegisterPartner(PartnerRegistrationRequest request);

        PartnerContractRecord UpdateContract(PartnerContractUpdateRequest request);

        PartnerReferralRecord[] GetReferrals(string partnerId);

        CommissionProcessingResult ProcessCommission(CommissionProcessingRequest request);

        PartnerStatusRecord GetPartnerStatus(string partnerId);

        PartnerReferralRecord SubmitReferral(PartnerReferralSubmission request);
    }
}
