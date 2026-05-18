using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub;
using Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Services
{
    public interface IPartnerAccountService
    {
        IList<string> GetPreferredPartners();

        PartnerContractRecord CreateContract(PartnerContractRequest request);

        PartnerContractRecord RenewContract(PartnerContractRenewalRequest request);

        PartnerContractRecord TerminateContract(PartnerContractTerminationRequest request);

        ReferralAttributionRecord TrackReferral(ReferralTrackingRequest request);

        CorporateAccountProfile OnboardCorporateAccount(CorporateAccountOnboardingRequest request);

        AccountTierClassification ClassifyAccountTier(string accountCode, decimal averageMonthlyVolume, int locationCount);

        CreditTermsDecision DetermineCreditTerms(string accountCode, string relationshipTier, decimal averageMonthlyVolume, bool requiresPurchaseOrder);

        CorporateAccountProfile GetCorporateAccount(string accountCode);
        PartnerProfile GetPartner(string partnerId);

        PartnerProfile RegisterPartner(PartnerRegistrationRequest request);

        PartnerContractRecord UpdateContract(PartnerContractUpdateRequest request);

        PartnerReferralRecord[] GetReferrals(string partnerId);

        CommissionProcessingResult ProcessCommission(CommissionProcessingRequest request);

        PartnerStatusRecord GetPartnerStatus(string partnerId);

        PartnerReferralRecord SubmitReferral(PartnerReferralSubmission request);
    }
}
