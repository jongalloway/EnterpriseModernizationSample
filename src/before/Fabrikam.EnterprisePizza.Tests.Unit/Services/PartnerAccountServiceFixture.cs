using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Configuration;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;
using Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub;
using Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class PartnerAccountServiceFixture
    {
        [Test]
        public void GetPreferredPartners_returns_seeded_partner_names()
        {
            var service = CreateService();

            var partners = service.GetPreferredPartners();

            Assert.That(partners, Has.Count.EqualTo(3));
            Assert.That(partners, Does.Contain("Contoso Office Parks"));
            Assert.That(partners, Does.Contain("Adventure Works Bike Expo"));
        }

        [Test]
        public void CreateRenewAndTerminateContract_manages_partner_contract_lifecycle()
        {
            var service = CreateService();
            var created = service.CreateContract(new PartnerContractRequest
            {
                PartnerCode = "contoso",
                PartnerName = "Contoso Office Parks",
                AccountCode = "corp-3400",
                EffectiveDateUtc = new DateTime(2006, 5, 18, 0, 0, 0, DateTimeKind.Utc),
                TermMonths = 12,
                ReferralChannelCode = "BROKER",
                MinimumMonthlyCommitment = 4200m,
                PricingPlan = "Campus Lunch"
            });

            Assert.That(created.ContractStatus, Is.EqualTo("Active"));
            Assert.That(created.ContractNumber, Is.EqualTo("CTR-CONT-CORP-200605"));
            Assert.That(created.RelationshipTier, Is.EqualTo("Preferred"));
            Assert.That(created.CommissionRatePercent, Is.EqualTo(0.035m));

            var renewed = service.RenewContract(new PartnerContractRenewalRequest
            {
                ContractNumber = created.ContractNumber,
                RenewalDateUtc = new DateTime(2007, 5, 18, 0, 0, 0, DateTimeKind.Utc),
                RenewalTermMonths = 18
            });

            Assert.That(renewed.LastRenewedDateUtc, Is.EqualTo(new DateTime(2007, 5, 18, 0, 0, 0, DateTimeKind.Utc)));
            Assert.That(renewed.ExpirationDateUtc, Is.EqualTo(new DateTime(2008, 11, 18, 0, 0, 0, DateTimeKind.Utc)));

            var terminated = service.TerminateContract(new PartnerContractTerminationRequest
            {
                ContractNumber = created.ContractNumber,
                TerminatedDateUtc = new DateTime(2008, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                Reason = "Consolidated under national agreement"
            });

            Assert.That(terminated.ContractStatus, Is.EqualTo("Terminated"));
            Assert.That(terminated.TerminatedDateUtc, Is.EqualTo(new DateTime(2008, 1, 15, 0, 0, 0, DateTimeKind.Utc)));
            Assert.That(terminated.TerminationReason, Is.EqualTo("Consolidated under national agreement"));
        }

        [Test]
        public void TrackReferral_uses_configured_channel_rates_and_multilocation_bonus()
        {
            var service = CreateService(new PartnerAccountBusinessRules(new NameValueCollection
            {
                { "customerHub:referrals:channels", "DIRECT|0.020;BROKER|0.035" },
                { "customerHub:referrals:minimumQualifiedSubtotal", "150" },
                { "customerHub:referrals:multiLocationBonusThreshold", "3" },
                { "customerHub:referrals:multiLocationBonusMultiplier", "1.25" }
            }));

            var referral = service.TrackReferral(new ReferralTrackingRequest
            {
                PartnerCode = "broker-17",
                ReferredAccountCode = "corp-5000",
                ReferredAccountName = "Northwind Campus Catering",
                ReferralChannelCode = "BROKER",
                OrderSubtotal = 1000m,
                FirstOrderDateUtc = new DateTime(2006, 6, 2, 0, 0, 0, DateTimeKind.Utc),
                LocationCount = 4
            });

            Assert.That(referral.AttributionStatus, Is.EqualTo("Qualified"));
            Assert.That(referral.CommissionRatePercent, Is.EqualTo(0.035m));
            Assert.That(referral.CommissionAmount, Is.EqualTo(43.75m));
            Assert.That(referral.Notes, Does.Contain("Multi-location bonus"));
        }

        [Test]
        public void OnboardCorporateAccount_assigns_tier_credit_terms_and_volume_pricing()
        {
            var service = CreateService();

            var profile = service.OnboardCorporateAccount(new CorporateAccountOnboardingRequest
            {
                AccountCode = "corp-8800",
                AccountName = "Adventure Works Campus Services",
                AverageMonthlyVolume = 8500m,
                RequiresPurchaseOrder = true,
                PrimaryBillingContact = "A. Porter",
                LeadReferralChannelCode = "PORTAL",
                LocationNames = new List<string>
                {
                    "Seattle HQ",
                    "Redmond Annex",
                    "Bellevue Events",
                    "Tacoma Warehouse"
                }
            });

            Assert.That(profile.WorkflowStatus, Is.EqualTo("Onboarding Approved"));
            Assert.That(profile.RelationshipTier, Is.EqualTo("Premier"));
            Assert.That(profile.PaymentTermsDays, Is.EqualTo(45));
            Assert.That(profile.CreditLimit, Is.EqualTo(20000m));
            Assert.That(profile.VolumeDiscountPercent, Is.EqualTo(0.065m));
            Assert.That(profile.Locations, Has.Count.EqualTo(4));
            Assert.That(service.GetCorporateAccount("corp-8800").AccountManager, Is.EqualTo("C. Reynolds"));
        }

        private static PartnerAccountService CreateService(PartnerAccountBusinessRules rules = null)
        {
            return new PartnerAccountService(new StubPartnerAccountRepository(), rules ?? new PartnerAccountBusinessRules());
        }

        private sealed class StubPartnerAccountRepository : IPartnerAccountRepository
        {
            public IList<string> GetPreferredPartners()
            {
                return new List<string>
                {
                    "Contoso Office Parks",
                    "Northwind Youth Sports League",
                    "Adventure Works Bike Expo"
                };
            }

            public IList<Fabrikam.EnterprisePizza.Core.Domain.CustomerHub.PartnerAccountSnapshot> GetPreferredPartnerSnapshots()
            {
                return new List<Fabrikam.EnterprisePizza.Core.Domain.CustomerHub.PartnerAccountSnapshot>();
            }

            public Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerProfile GetPartner(string partnerId)
            {
                return new Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerProfile();
            }

            public Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerProfile RegisterPartner(Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerRegistrationRequest request)
            {
                return new Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerProfile();
            }

            public Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerContractRecord UpdateContract(Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerContractUpdateRequest request)
            {
                return new Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerContractRecord();
            }

            public Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerReferralRecord[] GetReferrals(string partnerId)
            {
                return Array.Empty<Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerReferralRecord>();
            }

            public Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.CommissionProcessingResult ProcessCommission(Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.CommissionProcessingRequest request)
            {
                return new Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.CommissionProcessingResult();
            }

            public Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerReferralRecord SubmitReferral(Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerReferralSubmission request)
            {
                return new Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerReferralRecord();
            }
        }
    }
}
