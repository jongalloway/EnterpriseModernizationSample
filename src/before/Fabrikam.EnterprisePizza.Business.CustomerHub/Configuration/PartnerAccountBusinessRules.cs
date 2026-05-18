using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Configuration
{
    public sealed class PartnerAccountBusinessRules
    {
        private readonly IDictionary<string, decimal> _commissionRates;
        private readonly IList<VolumeDiscountRule> _volumeDiscountRules;

        public PartnerAccountBusinessRules()
            : this(ConfigurationManager.AppSettings)
        {
        }

        public PartnerAccountBusinessRules(NameValueCollection settings)
        {
            settings = settings ?? new NameValueCollection();
            DefaultContractTermMonths = ReadInt(settings, "customerHub:contracts:defaultTermMonths", 12);
            PreferredTierMonthlyVolume = ReadDecimal(settings, "customerHub:tiers:preferredMonthlyVolume", 2500m);
            PremierTierMonthlyVolume = ReadDecimal(settings, "customerHub:tiers:premierMonthlyVolume", 6000m);
            PremierLocationThreshold = ReadInt(settings, "customerHub:tiers:premierLocationThreshold", 4);
            StandardCreditLimit = ReadDecimal(settings, "customerHub:credit:standardLimit", 2500m);
            PreferredCreditLimit = ReadDecimal(settings, "customerHub:credit:preferredLimit", 7500m);
            PremierCreditLimit = ReadDecimal(settings, "customerHub:credit:premierLimit", 20000m);
            StandardPaymentTermsDays = ReadInt(settings, "customerHub:credit:standardTermsDays", 15);
            PreferredPaymentTermsDays = ReadInt(settings, "customerHub:credit:preferredTermsDays", 30);
            PremierPaymentTermsDays = ReadInt(settings, "customerHub:credit:premierTermsDays", 45);
            MinimumQualifiedReferralSubtotal = ReadDecimal(settings, "customerHub:referrals:minimumQualifiedSubtotal", 150m);
            MultiLocationBonusThreshold = ReadInt(settings, "customerHub:referrals:multiLocationBonusThreshold", 3);
            MultiLocationCommissionMultiplier = ReadDecimal(settings, "customerHub:referrals:multiLocationBonusMultiplier", 1.10m);
            _commissionRates = ParseCommissionRates(settings["customerHub:referrals:channels"]);
            _volumeDiscountRules = ParseVolumeDiscounts(settings["customerHub:accounts:volumeDiscounts"]);
        }

        public int DefaultContractTermMonths { get; private set; }

        public decimal PreferredTierMonthlyVolume { get; private set; }

        public decimal PremierTierMonthlyVolume { get; private set; }

        public int PremierLocationThreshold { get; private set; }

        public decimal StandardCreditLimit { get; private set; }

        public decimal PreferredCreditLimit { get; private set; }

        public decimal PremierCreditLimit { get; private set; }

        public int StandardPaymentTermsDays { get; private set; }

        public int PreferredPaymentTermsDays { get; private set; }

        public int PremierPaymentTermsDays { get; private set; }

        public decimal MinimumQualifiedReferralSubtotal { get; private set; }

        public int MultiLocationBonusThreshold { get; private set; }

        public decimal MultiLocationCommissionMultiplier { get; private set; }

        public decimal GetCommissionRate(string referralChannelCode)
        {
            if (string.IsNullOrWhiteSpace(referralChannelCode))
            {
                return _commissionRates["DIRECT"];
            }

            decimal rate;
            return _commissionRates.TryGetValue(referralChannelCode.Trim(), out rate)
                ? rate
                : _commissionRates["DIRECT"];
        }

        public string DetermineRelationshipTier(decimal averageMonthlyVolume, int locationCount)
        {
            if (averageMonthlyVolume >= PremierTierMonthlyVolume || (averageMonthlyVolume >= PreferredTierMonthlyVolume && locationCount >= PremierLocationThreshold))
            {
                return "Premier";
            }

            if (averageMonthlyVolume >= PreferredTierMonthlyVolume)
            {
                return "Preferred";
            }

            return "Standard";
        }

        public decimal GetCreditLimit(string relationshipTier)
        {
            switch ((relationshipTier ?? string.Empty).Trim())
            {
                case "Premier":
                    return PremierCreditLimit;
                case "Preferred":
                    return PreferredCreditLimit;
                default:
                    return StandardCreditLimit;
            }
        }

        public int GetPaymentTermsDays(string relationshipTier)
        {
            switch ((relationshipTier ?? string.Empty).Trim())
            {
                case "Premier":
                    return PremierPaymentTermsDays;
                case "Preferred":
                    return PreferredPaymentTermsDays;
                default:
                    return StandardPaymentTermsDays;
            }
        }

        public decimal GetVolumeDiscount(decimal averageMonthlyVolume, int locationCount)
        {
            var rule = _volumeDiscountRules
                .Where(item => averageMonthlyVolume >= item.MinimumMonthlyVolume)
                .OrderByDescending(item => item.MinimumMonthlyVolume)
                .FirstOrDefault();
            var discount = rule != null ? rule.DiscountPercent : 0m;

            if (locationCount >= PremierLocationThreshold && averageMonthlyVolume >= PreferredTierMonthlyVolume)
            {
                discount += 0.005m;
            }

            return discount;
        }

        public string GetAccountManagerAlias(string relationshipTier)
        {
            switch ((relationshipTier ?? string.Empty).Trim())
            {
                case "Premier":
                    return "C. Reynolds";
                case "Preferred":
                    return "M. Alvarez";
                default:
                    return "S. Turner";
            }
        }

        private static IDictionary<string, decimal> ParseCommissionRates(string configuredValue)
        {
            var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var rawChannel in Split(configuredValue, "DIRECT|0.020;BROKER|0.035;PORTAL|0.015;FRANCHISE|0.025"))
            {
                var parts = rawChannel.Split('|');
                if (parts.Length < 2)
                {
                    continue;
                }

                rates[parts[0].Trim()] = ParseDecimal(parts[1], 0.02m);
            }

            if (!rates.ContainsKey("DIRECT"))
            {
                rates["DIRECT"] = 0.02m;
            }

            return rates;
        }

        private static IList<VolumeDiscountRule> ParseVolumeDiscounts(string configuredValue)
        {
            return Split(configuredValue, "0|0.000;2500|0.020;5000|0.040;8000|0.060")
                .Select(rawRule => rawRule.Split('|'))
                .Where(parts => parts.Length >= 2)
                .Select(parts => new VolumeDiscountRule(ParseDecimal(parts[0], 0m), ParseDecimal(parts[1], 0m)))
                .OrderBy(rule => rule.MinimumMonthlyVolume)
                .ToList();
        }

        private static IEnumerable<string> Split(string configuredValue, string fallback)
        {
            var raw = string.IsNullOrWhiteSpace(configuredValue) ? fallback : configuredValue;
            return raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(segment => segment.Trim())
                .Where(segment => segment.Length > 0);
        }

        private static int ReadInt(NameValueCollection settings, string key, int fallback)
        {
            return ParseInt(settings[key], fallback);
        }

        private static decimal ReadDecimal(NameValueCollection settings, string key, decimal fallback)
        {
            return ParseDecimal(settings[key], fallback);
        }

        private static int ParseInt(string value, int fallback)
        {
            int parsedValue;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue)
                ? parsedValue
                : fallback;
        }

        private static decimal ParseDecimal(string value, decimal fallback)
        {
            decimal parsedValue;
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out parsedValue)
                ? parsedValue
                : fallback;
        }

        private sealed class VolumeDiscountRule
        {
            public VolumeDiscountRule(decimal minimumMonthlyVolume, decimal discountPercent)
            {
                MinimumMonthlyVolume = minimumMonthlyVolume;
                DiscountPercent = discountPercent;
            }

            public decimal MinimumMonthlyVolume { get; private set; }

            public decimal DiscountPercent { get; private set; }
        }
    }
}
