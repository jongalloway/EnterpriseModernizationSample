using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Domain.CustomerHub;
using Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public interface IPartnerAccountRepository
    {
        IList<string> GetPreferredPartners();

        IList<PartnerAccountSnapshot> GetPreferredPartnerSnapshots();
        PartnerProfile GetPartner(string partnerId);

        PartnerProfile RegisterPartner(PartnerRegistrationRequest request);

        PartnerContractRecord UpdateContract(PartnerContractUpdateRequest request);

        PartnerReferralRecord[] GetReferrals(string partnerId);

        CommissionProcessingResult ProcessCommission(CommissionProcessingRequest request);

        PartnerReferralRecord SubmitReferral(PartnerReferralSubmission request);
    }
}
