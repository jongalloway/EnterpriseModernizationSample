using System;
using System.Web.Services;
using System.Web.Services.Protocols;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync;

namespace Fabrikam.EnterprisePizza.Services.PartnerSync
{
    [WebService(Namespace = "http://fabrikam.com/pizza/partners/legacy/2005/06")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement, ParameterStyle = SoapParameterStyle.Wrapped)]
    public class PartnerLookupService : WebService
    {
        private readonly IPartnerAccountService _partnerAccountService;

        public PartnerLookupService()
            : this(LegacyServiceLocator.Resolve<IPartnerAccountService>())
        {
        }

        public PartnerLookupService(IPartnerAccountService partnerAccountService)
        {
            _partnerAccountService = partnerAccountService ?? throw new ArgumentNullException(nameof(partnerAccountService));
        }

        [WebMethod(Description = "Looks up the latest CustomerHub profile for a legacy partner account.")]
        public PartnerProfile LookupPartner(string partnerId)
        {
            return _partnerAccountService.GetPartner(partnerId);
        }

        [WebMethod(Description = "Returns the contract and approval status used by pre-WCF franchise clients.")]
        public PartnerStatusRecord GetPartnerStatus(string partnerId)
        {
            return _partnerAccountService.GetPartnerStatus(partnerId);
        }

        [WebMethod(Description = "Submits a referral from a legacy SOAP client into the CustomerHub partner queue.")]
        public PartnerReferralRecord SubmitReferral(string partnerId, string referredAccountName, string channelCode, string submittedBy)
        {
            return _partnerAccountService.SubmitReferral(new PartnerReferralSubmission
            {
                PartnerId = partnerId,
                ReferredAccountName = referredAccountName,
                ChannelCode = channelCode,
                SubmittedBy = submittedBy
            });
        }
    }
}
