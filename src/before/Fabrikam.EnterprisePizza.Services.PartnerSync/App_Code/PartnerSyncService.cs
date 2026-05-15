using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync;

namespace Fabrikam.EnterprisePizza.Services.PartnerSync
{
    [WebService(Namespace = "http://fabrikam.com/pizza/partners/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement, ParameterStyle = SoapParameterStyle.Wrapped)]
    public class PartnerSyncService : WebService
    {
        private readonly IPartnerAccountService _partnerAccountService;

        public PartnerSyncService()
            : this(LegacyServiceLocator.Resolve<IPartnerAccountService>())
        {
        }

        public PartnerSyncService(IPartnerAccountService partnerAccountService)
        {
            _partnerAccountService = partnerAccountService ?? throw new ArgumentNullException(nameof(partnerAccountService));
        }

        [WebMethod]
        public IList<string> GetPreferredPartners()
        {
            return _partnerAccountService.GetPreferredPartners();
        }

        [WebMethod(Description = "Returns the preferred partner export envelope used by legacy franchise sync jobs.")]
        public PartnerSyncEnvelope GetPreferredPartnerSnapshot(PartnerSyncRequest request)
        {
            var partners = _partnerAccountService.GetPreferredPartners();
            var includeCommunityPartners = request != null && request.IncludeCommunityPartners;

            if (!includeCommunityPartners)
            {
                partners = partners.Where(partner => !partner.Contains("League")).ToList();
            }

            return new PartnerSyncEnvelope
            {
                SourceSystem = "CustomerHub.PartnerSync",
                GeneratedAtUtc = DateTime.UtcNow,
                Partners = BuildPartnerSummaries(partners, request)
            };
        }

        private static PartnerAccountSummary[] BuildPartnerSummaries(IList<string> partners, PartnerSyncRequest request)
        {
            if (partners == null || partners.Count == 0)
            {
                return Array.Empty<PartnerAccountSummary>();
            }

            var channelCode = request == null || string.IsNullOrWhiteSpace(request.ChannelCode)
                ? "FRANCHISE"
                : request.ChannelCode.Trim().ToUpperInvariant();
            var summaries = new List<PartnerAccountSummary>();

            for (var index = 0; index < partners.Count; index++)
            {
                var partnerName = partners[index];
                summaries.Add(new PartnerAccountSummary
                {
                    PartnerName = partnerName,
                    AccountCode = channelCode + "-" + (index + 1).ToString("0000"),
                    RelationshipTier = ResolveRelationshipTier(partnerName),
                    ExportEnabled = true
                });
            }

            return summaries.ToArray();
        }

        private static string ResolveRelationshipTier(string partnerName)
        {
            if (string.IsNullOrWhiteSpace(partnerName))
            {
                return "Standard";
            }

            if (partnerName.Contains("Contoso"))
            {
                return "Gold";
            }

            if (partnerName.Contains("League"))
            {
                return "Community";
            }

            return "Preferred";
        }
    }
}
