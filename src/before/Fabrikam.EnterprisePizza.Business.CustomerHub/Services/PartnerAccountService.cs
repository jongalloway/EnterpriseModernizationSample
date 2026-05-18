using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Domain.CustomerHub;
using System.Globalization;
using System.Linq;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Configuration;
using Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub;
using Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub;
using Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync;
using CustomerHubPartnerContractRecord = Fabrikam.EnterprisePizza.Shared.Contracts.CustomerHub.PartnerContractRecord;
using PartnerSyncPartnerContractRecord = Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync.PartnerContractRecord;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Services
{
    public class PartnerAccountService : CustomerHubServiceBase, IPartnerAccountService
    {
        private static readonly DateTime DefaultEffectiveDateUtc = new DateTime(2006, 9, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly IDictionary<string, CustomerHubPartnerContractRecord> _contracts;
        private readonly IDictionary<string, CorporateAccountProfile> _accounts;
        private readonly IPartnerAccountRepository _partnerAccountRepository;
        private readonly PartnerAccountBusinessRules _businessRules;

        public PartnerAccountService()
            : this(new PartnerAccountRepository(), new PartnerAccountBusinessRules())
        {
        }

        public PartnerAccountService(IPartnerAccountRepository partnerAccountRepository)
            : this(partnerAccountRepository, new PartnerAccountBusinessRules())
        {
        }

        public PartnerAccountService(IPartnerAccountRepository partnerAccountRepository, PartnerAccountBusinessRules businessRules)
        {
            _partnerAccountRepository = partnerAccountRepository ?? throw new ArgumentNullException(nameof(partnerAccountRepository));
            _businessRules = businessRules ?? throw new ArgumentNullException(nameof(businessRules));
            _contracts = new Dictionary<string, CustomerHubPartnerContractRecord>(StringComparer.OrdinalIgnoreCase);
            _accounts = new Dictionary<string, CorporateAccountProfile>(StringComparer.OrdinalIgnoreCase);
            SeedCorporateAccounts();
            SeedContracts();
        }

        public IList<string> GetPreferredPartners()
        {
            return Execute(() => _partnerAccountRepository.GetPreferredPartners(), "GetPreferredPartners");
        }

        public CustomerHubPartnerContractRecord CreateContract(PartnerContractRequest request)
        {
            return Execute(delegate
            {
                ValidateContractRequest(request);
                var effectiveDate = request.EffectiveDateUtc == default(DateTime) ? DefaultEffectiveDateUtc : request.EffectiveDateUtc;
                var termMonths = request.TermMonths > 0 ? request.TermMonths : _businessRules.DefaultContractTermMonths;
                var relationshipTier = _businessRules.DetermineRelationshipTier(request.MinimumMonthlyCommitment, GetLocationCount(request.AccountCode));
                var contract = new CustomerHubPartnerContractRecord
                {
                    ContractNumber = BuildContractNumber(request.PartnerCode, request.AccountCode, effectiveDate),
                    PartnerCode = NormalizeCode(request.PartnerCode, "PARTNER"),
                    PartnerName = request.PartnerName.Trim(),
                    AccountCode = NormalizeCode(request.AccountCode, "ACCOUNT"),
                    RelationshipTier = relationshipTier,
                    ContractStatus = "Active",
                    EffectiveDateUtc = effectiveDate,
                    ExpirationDateUtc = effectiveDate.AddMonths(termMonths),
                    ReferralChannelCode = NormalizeCode(request.ReferralChannelCode, "DIRECT"),
                    CommissionRatePercent = _businessRules.GetCommissionRate(request.ReferralChannelCode),
                    MinimumMonthlyCommitment = request.MinimumMonthlyCommitment,
                    PricingPlan = string.IsNullOrWhiteSpace(request.PricingPlan) ? "Corporate Menu" : request.PricingPlan.Trim()
                };

                _contracts[contract.ContractNumber] = Clone(contract);
                return Clone(contract);
            }, "CreateContract");
        }

        public CustomerHubPartnerContractRecord RenewContract(PartnerContractRenewalRequest request)
        {
            return Execute(delegate
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var contract = GetStoredContract(request.ContractNumber);
                var renewalDate = request.RenewalDateUtc == default(DateTime) ? contract.ExpirationDateUtc : request.RenewalDateUtc;
                var renewalTermMonths = request.RenewalTermMonths > 0 ? request.RenewalTermMonths : _businessRules.DefaultContractTermMonths;
                contract.ContractStatus = "Active";
                contract.LastRenewedDateUtc = renewalDate;
                contract.ExpirationDateUtc = renewalDate.AddMonths(renewalTermMonths);
                contract.TerminatedDateUtc = null;
                contract.TerminationReason = null;
                return Clone(contract);
            }, "RenewContract");
        }

        public CustomerHubPartnerContractRecord TerminateContract(PartnerContractTerminationRequest request)
        {
            return Execute(delegate
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var contract = GetStoredContract(request.ContractNumber);
                contract.ContractStatus = "Terminated";
                contract.TerminatedDateUtc = request.TerminatedDateUtc == default(DateTime) ? contract.ExpirationDateUtc : request.TerminatedDateUtc;
                contract.TerminationReason = string.IsNullOrWhiteSpace(request.Reason) ? "Relationship review closed the agreement." : request.Reason.Trim();
                return Clone(contract);
            }, "TerminateContract");
        }

        public ReferralAttributionRecord TrackReferral(ReferralTrackingRequest request)
        {
            return Execute(delegate
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                if (string.IsNullOrWhiteSpace(request.PartnerCode))
                {
                    throw new ArgumentException("Partner code is required.", nameof(request));
                }

                if (string.IsNullOrWhiteSpace(request.ReferredAccountCode))
                {
                    throw new ArgumentException("Referred account code is required.", nameof(request));
                }

                if (request.OrderSubtotal < 0m)
                {
                    throw new ArgumentOutOfRangeException(nameof(request.OrderSubtotal));
                }

                var commissionRate = _businessRules.GetCommissionRate(request.ReferralChannelCode);
                var multiplier = request.LocationCount >= _businessRules.MultiLocationBonusThreshold
                    ? _businessRules.MultiLocationCommissionMultiplier
                    : 1.0m;
                var qualified = request.OrderSubtotal >= _businessRules.MinimumQualifiedReferralSubtotal;
                var commissionAmount = qualified
                    ? decimal.Round(request.OrderSubtotal * commissionRate * multiplier, 2, MidpointRounding.AwayFromZero)
                    : 0m;

                return new ReferralAttributionRecord
                {
                    ReferralId = BuildReferralId(request.PartnerCode, request.ReferredAccountCode, request.FirstOrderDateUtc),
                    PartnerCode = NormalizeCode(request.PartnerCode, "PARTNER"),
                    ReferredAccountCode = NormalizeCode(request.ReferredAccountCode, "ACCOUNT"),
                    ReferredAccountName = request.ReferredAccountName,
                    ReferralChannelCode = NormalizeCode(request.ReferralChannelCode, "DIRECT"),
                    AttributionStatus = qualified ? "Qualified" : "Pending Volume",
                    CommissionRatePercent = commissionRate,
                    CommissionAmount = commissionAmount,
                    AttributedOnUtc = request.FirstOrderDateUtc == default(DateTime) ? DefaultEffectiveDateUtc : request.FirstOrderDateUtc,
                    Notes = qualified && multiplier > 1.0m
                        ? "Multi-location bonus applied to qualified referral."
                        : qualified ? "Referral met the minimum order threshold." : "Referral is waiting for the first qualified order."
                };
            }, "TrackReferral");
        }

        public CorporateAccountProfile OnboardCorporateAccount(CorporateAccountOnboardingRequest request)
        {
            return Execute(delegate
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                if (string.IsNullOrWhiteSpace(request.AccountName))
                {
                    throw new ArgumentException("Account name is required.", nameof(request));
                }

                if (request.AverageMonthlyVolume < 0m)
                {
                    throw new ArgumentOutOfRangeException(nameof(request.AverageMonthlyVolume));
                }

                var accountCode = NormalizeCode(request.AccountCode, GenerateAccountCode(request.AccountName));
                var locationNames = request.LocationNames != null && request.LocationNames.Count > 0
                    ? request.LocationNames.Where(name => !string.IsNullOrWhiteSpace(name)).ToList()
                    : new List<string>();
                if (locationNames.Count == 0)
                {
                    locationNames.Add(request.AccountName + " Headquarters");
                }

                var tier = ClassifyAccountTier(accountCode, request.AverageMonthlyVolume, locationNames.Count);
                var creditTerms = DetermineCreditTerms(accountCode, tier.RelationshipTier, request.AverageMonthlyVolume, request.RequiresPurchaseOrder);
                var profile = new CorporateAccountProfile
                {
                    AccountCode = accountCode,
                    AccountName = request.AccountName.Trim(),
                    WorkflowStatus = creditTerms.RequiresManualReview ? "Credit Review" : "Onboarding Approved",
                    RelationshipTier = tier.RelationshipTier,
                    PaymentTermsDays = creditTerms.PaymentTermsDays,
                    CreditLimit = creditTerms.CreditLimit,
                    VolumeDiscountPercent = _businessRules.GetVolumeDiscount(request.AverageMonthlyVolume, locationNames.Count),
                    PrimaryBillingContact = string.IsNullOrWhiteSpace(request.PrimaryBillingContact) ? "Accounts Payable" : request.PrimaryBillingContact.Trim(),
                    LeadReferralChannelCode = NormalizeCode(request.LeadReferralChannelCode, "DIRECT"),
                    AccountManager = _businessRules.GetAccountManagerAlias(tier.RelationshipTier),
                    NextReviewDateUtc = DefaultEffectiveDateUtc.AddMonths(6)
                };

                for (var index = 0; index < locationNames.Count; index++)
                {
                    profile.Locations.Add(new CorporateLocationRecord
                    {
                        LocationCode = accountCode + "-L" + (index + 1).ToString("00", CultureInfo.InvariantCulture),
                        LocationName = locationNames[index].Trim(),
                        City = index == 0 ? "Seattle" : "Regional Hub " + (index + 1).ToString(CultureInfo.InvariantCulture),
                        ServiceWindow = index == 0 ? "Weekday Lunch" : "Weekday Lunch / Event Support",
                        ParticipatesInVolumePricing = true
                    });
                }

                _accounts[profile.AccountCode] = Clone(profile);
                return Clone(profile);
            }, "OnboardCorporateAccount");
        }

        public AccountTierClassification ClassifyAccountTier(string accountCode, decimal averageMonthlyVolume, int locationCount)
        {
            return Execute(delegate
            {
                if (string.IsNullOrWhiteSpace(accountCode))
                {
                    throw new ArgumentException("Account code is required.", nameof(accountCode));
                }

                var relationshipTier = _businessRules.DetermineRelationshipTier(averageMonthlyVolume, locationCount);
                var note = relationshipTier == "Premier"
                    ? "Volume and footprint qualify for the national account desk."
                    : relationshipTier == "Preferred"
                        ? "Account qualifies for partner-managed pricing and invoice billing."
                        : "Account remains on the standard local-business schedule.";

                return new AccountTierClassification
                {
                    AccountCode = NormalizeCode(accountCode, "ACCOUNT"),
                    RelationshipTier = relationshipTier,
                    AverageMonthlyVolume = averageMonthlyVolume,
                    LocationCount = locationCount,
                    ReviewNote = note
                };
            }, "ClassifyAccountTier");
        }

        public CreditTermsDecision DetermineCreditTerms(string accountCode, string relationshipTier, decimal averageMonthlyVolume, bool requiresPurchaseOrder)
        {
            return Execute(delegate
            {
                if (string.IsNullOrWhiteSpace(accountCode))
                {
                    throw new ArgumentException("Account code is required.", nameof(accountCode));
                }

                var normalizedTier = string.IsNullOrWhiteSpace(relationshipTier) ? "Standard" : relationshipTier.Trim();
                var creditLimit = _businessRules.GetCreditLimit(normalizedTier);
                var requiresManualReview = !requiresPurchaseOrder && averageMonthlyVolume > creditLimit * 0.75m;
                var reviewReason = requiresManualReview
                    ? "High invoice exposure without a purchase-order workflow."
                    : "Auto-approved using configured CustomerHub credit rules.";

                return new CreditTermsDecision
                {
                    AccountCode = NormalizeCode(accountCode, "ACCOUNT"),
                    RelationshipTier = normalizedTier,
                    PaymentTermsDays = _businessRules.GetPaymentTermsDays(normalizedTier),
                    CreditLimit = creditLimit,
                    RequiresManualReview = requiresManualReview,
                    ReviewReason = reviewReason
                };
            }, "DetermineCreditTerms");
        }

        public CorporateAccountProfile GetCorporateAccount(string accountCode)
        {
            return Execute(delegate
            {
                if (string.IsNullOrWhiteSpace(accountCode))
                {
                    throw new ArgumentException("Account code is required.", nameof(accountCode));
                }

                CorporateAccountProfile account;
                var normalizedCode = NormalizeCode(accountCode, "ACCOUNT");
                if (!_accounts.TryGetValue(normalizedCode, out account))
                {
                    throw new KeyNotFoundException("No corporate account exists for code " + normalizedCode + ".");
                }

                return Clone(account);
            }, "GetCorporateAccount");
        }

        private CustomerHubPartnerContractRecord GetStoredContract(string contractNumber)
        {
            if (string.IsNullOrWhiteSpace(contractNumber))
            {
                throw new ArgumentException("Contract number is required.", nameof(contractNumber));
            }

            CustomerHubPartnerContractRecord contract;
            if (!_contracts.TryGetValue(contractNumber.Trim(), out contract))
            {
                throw new KeyNotFoundException("No contract exists for number " + contractNumber + ".");
            }

            return contract;
        }

        private void SeedCorporateAccounts()
        {
            _accounts["CORP-1000"] = new CorporateAccountProfile
            {
                AccountCode = "CORP-1000",
                AccountName = "Contoso Office Parks",
                WorkflowStatus = "Active",
                RelationshipTier = "Premier",
                PaymentTermsDays = _businessRules.GetPaymentTermsDays("Premier"),
                CreditLimit = _businessRules.GetCreditLimit("Premier"),
                VolumeDiscountPercent = _businessRules.GetVolumeDiscount(9600m, 3),
                PrimaryBillingContact = "AP Central",
                LeadReferralChannelCode = "BROKER",
                AccountManager = _businessRules.GetAccountManagerAlias("Premier"),
                NextReviewDateUtc = new DateTime(2007, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                Locations = new List<CorporateLocationRecord>
                {
                    new CorporateLocationRecord
                    {
                        LocationCode = "CORP-1000-L01",
                        LocationName = "Contoso Tower",
                        City = "Seattle",
                        ServiceWindow = "Weekday Lunch",
                        ParticipatesInVolumePricing = true
                    },
                    new CorporateLocationRecord
                    {
                        LocationCode = "CORP-1000-L02",
                        LocationName = "Harbor Campus",
                        City = "Bellevue",
                        ServiceWindow = "Weekday Lunch / Event Support",
                        ParticipatesInVolumePricing = true
                    }
                }
            };
        }

        private void SeedContracts()
        {
            var contract = new CustomerHubPartnerContractRecord
            {
                ContractNumber = "CTR-CONT-CORP-200609",
                PartnerCode = "CONTOSO",
                PartnerName = "Contoso Office Parks",
                AccountCode = "CORP-1000",
                RelationshipTier = "Premier",
                ContractStatus = "Active",
                EffectiveDateUtc = DefaultEffectiveDateUtc,
                ExpirationDateUtc = DefaultEffectiveDateUtc.AddMonths(12),
                ReferralChannelCode = "BROKER",
                CommissionRatePercent = _businessRules.GetCommissionRate("BROKER"),
                MinimumMonthlyCommitment = 9600m,
                PricingPlan = "Campus Catering"
            };

            _contracts[contract.ContractNumber] = contract;
        }

        private int GetLocationCount(string accountCode)
        {
            if (string.IsNullOrWhiteSpace(accountCode))
            {
                return 1;
            }

            CorporateAccountProfile account;
            return _accounts.TryGetValue(NormalizeCode(accountCode, "ACCOUNT"), out account)
                ? Math.Max(account.Locations.Count, 1)
                : 1;
        }

        private static void ValidateContractRequest(PartnerContractRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.PartnerCode))
            {
                throw new ArgumentException("Partner code is required.", nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.PartnerName))
            {
                throw new ArgumentException("Partner name is required.", nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.AccountCode))
            {
                throw new ArgumentException("Account code is required.", nameof(request));
            }

            if (request.MinimumMonthlyCommitment < 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(request.MinimumMonthlyCommitment));
            }
        }

        private static string BuildContractNumber(string partnerCode, string accountCode, DateTime effectiveDateUtc)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "CTR-{0}-{1}-{2}",
                ShortCode(partnerCode, 4),
                ShortCode(accountCode, 4),
                effectiveDateUtc.ToString("yyyyMM", CultureInfo.InvariantCulture));
        }

        private static string BuildReferralId(string partnerCode, string accountCode, DateTime firstOrderDateUtc)
        {
            var effectiveDate = firstOrderDateUtc == default(DateTime) ? DefaultEffectiveDateUtc : firstOrderDateUtc;
            return string.Format(
                CultureInfo.InvariantCulture,
                "REF-{0}-{1}-{2}",
                ShortCode(partnerCode, 4),
                ShortCode(accountCode, 4),
                effectiveDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        }

        private static string GenerateAccountCode(string accountName)
        {
            var suffix = string.IsNullOrWhiteSpace(accountName)
                ? string.Empty
                : new string(accountName
                    .ToUpperInvariant()
                    .Where(char.IsLetterOrDigit)
                    .Take(6)
                    .ToArray());
            return "CORP-" + (suffix.Length > 0 ? suffix : "0000");
        }

        private static string NormalizeCode(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            return value.Trim().ToUpperInvariant();
        }

        private static string ShortCode(string value, int length)
        {
            var normalized = NormalizeCode(value, "CODE")
                .Where(char.IsLetterOrDigit)
                .Take(length)
                .ToArray();
            var shortCode = new string(normalized);
            return shortCode.Length > 0 ? shortCode : "CODE";
        }

        private static CustomerHubPartnerContractRecord Clone(CustomerHubPartnerContractRecord contract)
        {
            return new CustomerHubPartnerContractRecord
            {
                ContractNumber = contract.ContractNumber,
                PartnerCode = contract.PartnerCode,
                PartnerName = contract.PartnerName,
                AccountCode = contract.AccountCode,
                RelationshipTier = contract.RelationshipTier,
                ContractStatus = contract.ContractStatus,
                EffectiveDateUtc = contract.EffectiveDateUtc,
                ExpirationDateUtc = contract.ExpirationDateUtc,
                LastRenewedDateUtc = contract.LastRenewedDateUtc,
                TerminatedDateUtc = contract.TerminatedDateUtc,
                TerminationReason = contract.TerminationReason,
                ReferralChannelCode = contract.ReferralChannelCode,
                CommissionRatePercent = contract.CommissionRatePercent,
                MinimumMonthlyCommitment = contract.MinimumMonthlyCommitment,
                PricingPlan = contract.PricingPlan
            };
        }

        private static CorporateAccountProfile Clone(CorporateAccountProfile profile)
        {
            var clone = new CorporateAccountProfile
            {
                AccountCode = profile.AccountCode,
                AccountName = profile.AccountName,
                WorkflowStatus = profile.WorkflowStatus,
                RelationshipTier = profile.RelationshipTier,
                PaymentTermsDays = profile.PaymentTermsDays,
                CreditLimit = profile.CreditLimit,
                VolumeDiscountPercent = profile.VolumeDiscountPercent,
                PrimaryBillingContact = profile.PrimaryBillingContact,
                LeadReferralChannelCode = profile.LeadReferralChannelCode,
                AccountManager = profile.AccountManager,
                NextReviewDateUtc = profile.NextReviewDateUtc
            };

            foreach (var location in profile.Locations)
            {
                clone.Locations.Add(new CorporateLocationRecord
                {
                    LocationCode = location.LocationCode,
                    LocationName = location.LocationName,
                    City = location.City,
                    ServiceWindow = location.ServiceWindow,
                    ParticipatesInVolumePricing = location.ParticipatesInVolumePricing
                });
            }

            return clone;
        }

        public PartnerProfile GetPartner(string partnerId)
        {
            return Execute(() => _partnerAccountRepository.GetPartner(partnerId), "GetPartner");
        }

        public PartnerProfile RegisterPartner(PartnerRegistrationRequest request)
        {
            return Execute(() => _partnerAccountRepository.RegisterPartner(request), "RegisterPartner");
        }

        public PartnerSyncPartnerContractRecord UpdateContract(PartnerContractUpdateRequest request)
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

        public IList<PartnerAccountSnapshot> GetPreferredPartnerSnapshots()
        {
            return _partnerAccountRepository.GetPreferredPartnerSnapshots();
        }
    }
}
