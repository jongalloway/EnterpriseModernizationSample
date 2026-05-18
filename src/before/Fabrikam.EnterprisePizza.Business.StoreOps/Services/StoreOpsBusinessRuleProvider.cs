using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class StoreOpsBusinessRuleProvider : IStoreOpsBusinessRuleProvider
    {
        private const string TaxRateKey = "StoreOps.BusinessRules.TaxRate.Default";
        private const string MinimumOrderKey = "StoreOps.BusinessRules.MinimumOrder.Default";
        private const string MinimumOrderByStoreKey = "StoreOps.BusinessRules.MinimumOrder.ByStore";
        private const string OpenHourKey = "StoreOps.BusinessRules.StoreHours.Open";
        private const string CloseHourKey = "StoreOps.BusinessRules.StoreHours.Close";
        private const string DeliveryZonesKey = "StoreOps.BusinessRules.DeliveryZones";
        private const string MenuPricesKey = "StoreOps.BusinessRules.MenuPrices";
        private const string VolumeDiscountsKey = "StoreOps.BusinessRules.VolumeDiscounts";
        private const string PromoCodesKey = "StoreOps.BusinessRules.PromoCodes";
        private const string FranchiseOverridesKey = "StoreOps.BusinessRules.FranchiseOverrides";

        private readonly decimal _defaultTaxRate;
        private readonly decimal _defaultMinimumOrder;
        private readonly int _openHour;
        private readonly int _closeHour;
        private readonly Dictionary<string, decimal> _minimumOrderByStore;
        private readonly HashSet<string> _deliveryZones;
        private readonly Dictionary<string, decimal> _menuPrices;
        private readonly SortedDictionary<int, decimal> _volumeDiscounts;
        private readonly Dictionary<string, PromoDefinition> _promoDefinitions;
        private readonly Dictionary<string, decimal> _franchiseOverrides;

        public StoreOpsBusinessRuleProvider()
            : this(ConfigurationManager.AppSettings)
        {
        }

        public StoreOpsBusinessRuleProvider(NameValueCollection settings)
        {
            settings = settings ?? new NameValueCollection();
            _defaultTaxRate = ReadDecimal(settings, TaxRateKey, 0.0825m);
            _defaultMinimumOrder = ReadDecimal(settings, MinimumOrderKey, 15.00m);
            _openHour = ReadInteger(settings, OpenHourKey, 10);
            _closeHour = ReadInteger(settings, CloseHourKey, 23);
            _minimumOrderByStore = ParseDecimalMap(settings[MinimumOrderByStoreKey], '|', '=');
            _deliveryZones = ParseStringSet(settings[DeliveryZonesKey], "NORTH|SOUTH|CAMPUS|AIRPORT|DOWNTOWN");
            _menuPrices = ParseDecimalMap("PIZZA-LG=15.99|WINGS-12=11.49|PASTA-BAKE=9.49|SODA-2L=3.99|SALAD-CAESAR=6.99|BREADSTICKS=5.49", '|', '=')
                .Concat(ParseDecimalMap(settings[MenuPricesKey], '|', '='))
                .GroupBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Last().Value, StringComparer.OrdinalIgnoreCase);
            _volumeDiscounts = ParseVolumeDiscounts(settings[VolumeDiscountsKey] ?? "3=0.05|5=0.10|8=0.15");
            _promoDefinitions = ParsePromoDefinitions(settings[PromoCodesKey] ?? "TUESDAY10=percent,0.10,Extreme Value Tuesday|FAMILY5=amount,5.00,Family Feast coupon|CAMPUS15=percent,0.15,Campus carryout deal");
            _franchiseOverrides = ParseFranchiseOverrides(settings[FranchiseOverridesKey] ?? "014:PIZZA-LG=14.49|031:WINGS-12=10.99|081:SALAD-CAESAR=5.99");
        }

        public decimal GetTaxRate(string storeNumber)
        {
            return _defaultTaxRate;
        }

        public decimal GetMinimumOrder(string storeNumber)
        {
            decimal minimumOrder;
            return _minimumOrderByStore.TryGetValue(Normalize(storeNumber), out minimumOrder) ? minimumOrder : _defaultMinimumOrder;
        }

        public bool IsDeliveryZoneAllowed(string deliveryZone)
        {
            return !string.IsNullOrWhiteSpace(deliveryZone) && _deliveryZones.Contains(deliveryZone.Trim().ToUpperInvariant());
        }

        public bool IsWithinStoreHours(DateTime requestedFulfillmentTimeLocal)
        {
            var value = requestedFulfillmentTimeLocal.TimeOfDay;
            var open = TimeSpan.FromHours(_openHour);
            var close = TimeSpan.FromHours(_closeHour);
            if (close <= open)
            {
                return value >= open || value <= close;
            }

            return value >= open && value <= close;
        }

        public bool TryGetConfiguredPrice(string storeNumber, string menuItemCode, out decimal unitPrice)
        {
            var normalizedStore = Normalize(storeNumber);
            var normalizedItem = Normalize(menuItemCode);
            decimal overridePrice;
            if (_franchiseOverrides.TryGetValue(normalizedStore + ":" + normalizedItem, out overridePrice))
            {
                unitPrice = overridePrice;
                return true;
            }

            return _menuPrices.TryGetValue(normalizedItem, out unitPrice);
        }

        public decimal GetVolumeDiscountRate(int totalQuantity)
        {
            decimal rate = 0m;
            foreach (var pair in _volumeDiscounts)
            {
                if (totalQuantity >= pair.Key)
                {
                    rate = pair.Value;
                }
            }

            return rate;
        }

        public PromoDefinition GetPromo(string promoCode)
        {
            if (string.IsNullOrWhiteSpace(promoCode))
            {
                return null;
            }

            PromoDefinition definition;
            return _promoDefinitions.TryGetValue(Normalize(promoCode), out definition) ? definition : null;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();
        }

        private static decimal ReadDecimal(NameValueCollection settings, string key, decimal fallback)
        {
            decimal parsed;
            return decimal.TryParse(settings[key], NumberStyles.Number, CultureInfo.InvariantCulture, out parsed) ? parsed : fallback;
        }

        private static int ReadInteger(NameValueCollection settings, string key, int fallback)
        {
            int parsed;
            return int.TryParse(settings[key], NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed) ? parsed : fallback;
        }

        private static Dictionary<string, decimal> ParseDecimalMap(string configuredValue, char entrySeparator, char assignmentSeparator)
        {
            var values = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(configuredValue))
            {
                return values;
            }

            foreach (var entry in configuredValue.Split(new[] { entrySeparator }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = entry.Split(new[] { assignmentSeparator }, 2);
                decimal parsed;
                if (parts.Length == 2 && decimal.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
                {
                    values[Normalize(parts[0])] = parsed;
                }
            }

            return values;
        }

        private static HashSet<string> ParseStringSet(string configuredValue, string fallback)
        {
            var source = string.IsNullOrWhiteSpace(configuredValue) ? fallback : configuredValue;
            return new HashSet<string>(source.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(Normalize), StringComparer.OrdinalIgnoreCase);
        }

        private static SortedDictionary<int, decimal> ParseVolumeDiscounts(string configuredValue)
        {
            var discounts = new SortedDictionary<int, decimal>();
            foreach (var entry in configuredValue.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = entry.Split(new[] { '=' }, 2);
                int quantity;
                decimal rate;
                if (parts.Length == 2
                    && int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out quantity)
                    && decimal.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out rate))
                {
                    discounts[quantity] = rate;
                }
            }

            return discounts;
        }

        private static Dictionary<string, PromoDefinition> ParsePromoDefinitions(string configuredValue)
        {
            var definitions = new Dictionary<string, PromoDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in configuredValue.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var keyParts = entry.Split(new[] { '=' }, 2);
                if (keyParts.Length != 2)
                {
                    continue;
                }

                var valueParts = keyParts[1].Split(new[] { ',' }, 3);
                if (valueParts.Length < 3)
                {
                    continue;
                }

                decimal value;
                if (!decimal.TryParse(valueParts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    continue;
                }

                var discountType = valueParts[0].Trim().Equals("amount", StringComparison.OrdinalIgnoreCase)
                    ? PromoDiscountType.FixedAmount
                    : PromoDiscountType.Percentage;

                definitions[Normalize(keyParts[0])] = new PromoDefinition
                {
                    Code = Normalize(keyParts[0]),
                    Description = valueParts[2].Trim(),
                    DiscountType = discountType,
                    Value = value
                };
            }

            return definitions;
        }

        private static Dictionary<string, decimal> ParseFranchiseOverrides(string configuredValue)
        {
            var overrides = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in configuredValue.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var storeAndItem = entry.Split(new[] { ':' }, 2);
                if (storeAndItem.Length != 2)
                {
                    continue;
                }

                var itemAndPrice = storeAndItem[1].Split(new[] { '=' }, 2);
                decimal parsed;
                if (itemAndPrice.Length == 2 && decimal.TryParse(itemAndPrice[1], NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
                {
                    overrides[Normalize(storeAndItem[0]) + ":" + Normalize(itemAndPrice[0])] = parsed;
                }
            }

            return overrides;
        }
    }
}
