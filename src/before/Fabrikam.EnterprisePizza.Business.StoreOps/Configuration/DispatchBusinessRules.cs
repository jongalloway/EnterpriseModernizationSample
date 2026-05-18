using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Configuration
{
    public sealed class DispatchBusinessRules
    {
        private readonly IDictionary<string, DispatchZoneBoundary> _zoneBoundaries;
        private readonly IList<DriverRosterEntry> _driverRoster;

        public DispatchBusinessRules()
            : this(ConfigurationManager.AppSettings)
        {
        }

        public DispatchBusinessRules(NameValueCollection settings)
        {
            settings = settings ?? new NameValueCollection();
            MaxDeliveriesPerDriver = ReadInt(settings, "storeOps:dispatch:maxDeliveriesPerDriver", 3);
            MaxBatchSize = ReadInt(settings, "storeOps:dispatch:maxBatchSize", 2);
            StopServiceMinutes = ReadInt(settings, "storeOps:dispatch:stopServiceMinutes", 4);
            BatchWindowMinutes = ReadInt(settings, "storeOps:dispatch:batchWindowMinutes", 12);
            UrgentLeadMinutesThreshold = ReadInt(settings, "storeOps:dispatch:urgentLeadMinutesThreshold", 18);
            AverageSpeedMph = ReadDecimal(settings, "storeOps:dispatch:averageSpeedMph", 24.0m);
            _zoneBoundaries = ParseZones(settings["storeOps:dispatch:zones"]);
            _driverRoster = ParseDrivers(settings["storeOps:dispatch:drivers"]);
        }

        public int MaxDeliveriesPerDriver { get; private set; }

        public int MaxBatchSize { get; private set; }

        public int StopServiceMinutes { get; private set; }

        public int BatchWindowMinutes { get; private set; }

        public int UrgentLeadMinutesThreshold { get; private set; }

        public decimal AverageSpeedMph { get; private set; }

        public DispatchZoneBoundary GetZoneBoundary(string routeZone)
        {
            DispatchZoneBoundary configuredBoundary;
            if (string.IsNullOrWhiteSpace(routeZone))
            {
                return _zoneBoundaries["*"];
            }

            if (_zoneBoundaries.TryGetValue(routeZone.Trim(), out configuredBoundary))
            {
                return configuredBoundary;
            }

            return _zoneBoundaries["*"];
        }

        public IList<DriverRosterEntry> GetDriverRoster(string storeNumber)
        {
            return _driverRoster
                .Select(driver => driver.Clone())
                .ToList();
        }

        private static IDictionary<string, DispatchZoneBoundary> ParseZones(string configuredValue)
        {
            var boundaries = new Dictionary<string, DispatchZoneBoundary>(StringComparer.OrdinalIgnoreCase);
            foreach (var rawZone in Split(configuredValue, GetDefaultZones()))
            {
                var parts = rawZone.Split('|');
                if (parts.Length < 5)
                {
                    continue;
                }

                var zone = parts[0].Trim();
                boundaries[zone] = new DispatchZoneBoundary(
                    zone,
                    parts[1].Trim(),
                    ParseDecimal(parts[2], 5.0m),
                    ParseDecimal(parts[3], 1.0m),
                    ParseInt(parts[4], 5));
            }

            boundaries["*"] = new DispatchZoneBoundary("General Delivery", "GEN-01", 5.0m, 1.0m, 9);
            return boundaries;
        }

        private static IList<DriverRosterEntry> ParseDrivers(string configuredValue)
        {
            return Split(configuredValue, GetDefaultDrivers())
                .Select(rawDriver => rawDriver.Split('|'))
                .Where(parts => parts.Length >= 4)
                .Select(parts => new DriverRosterEntry(
                    parts[0].Trim(),
                    parts[1].Trim(),
                    parts[2].Trim(),
                    ParseTime(parts[3], 17, 0)))
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

        private static DateTime ParseTime(string value, int fallbackHour, int fallbackMinute)
        {
            DateTime parsedValue;
            if (DateTime.TryParseExact(value, new[] { "H:mm", "HH:mm", "h:mm tt" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedValue))
            {
                return DateTime.Today.AddHours(parsedValue.Hour).AddMinutes(parsedValue.Minute);
            }

            return DateTime.Today.AddHours(fallbackHour).AddMinutes(fallbackMinute);
        }

        private static string GetDefaultZones()
        {
            return string.Join(";", new[]
            {
                "Northwest Corporate Corridor|NW-12|6.8|1.10|1",
                "Mall Annex|CT-03|4.2|0.95|2",
                "College Commons|CP-07|5.6|1.05|2",
                "Airport Service Road|AR-04|7.4|1.20|3"
            });
        }

        private static string GetDefaultDrivers()
        {
            return string.Join(";", new[]
            {
                "DRV-17|Northwest Corporate Corridor|NW-12|17:05",
                "DRV-03|Mall Annex|CT-03|17:10",
                "DRV-11|College Commons|CP-07|17:08",
                "DRV-22|Airport Service Road|AR-04|17:12"
            });
        }
    }

    public sealed class DispatchZoneBoundary
    {
        public DispatchZoneBoundary(string routeZone, string boundaryCode, decimal defaultDistanceMiles, decimal travelFactor, int priorityRank)
        {
            RouteZone = routeZone;
            BoundaryCode = boundaryCode;
            DefaultDistanceMiles = defaultDistanceMiles;
            TravelFactor = travelFactor;
            PriorityRank = priorityRank;
        }

        public string RouteZone { get; private set; }

        public string BoundaryCode { get; private set; }

        public decimal DefaultDistanceMiles { get; private set; }

        public decimal TravelFactor { get; private set; }

        public int PriorityRank { get; private set; }
    }

    public sealed class DriverRosterEntry
    {
        public DriverRosterEntry(string driverCode, string homeZone, string boundaryCode, DateTime lastCompletedAtLocal)
        {
            DriverCode = driverCode;
            HomeZone = homeZone;
            BoundaryCode = boundaryCode;
            LastCompletedAtLocal = lastCompletedAtLocal;
        }

        public string DriverCode { get; private set; }

        public string HomeZone { get; private set; }

        public string BoundaryCode { get; private set; }

        public DateTime LastCompletedAtLocal { get; private set; }

        public DriverRosterEntry Clone()
        {
            return new DriverRosterEntry(DriverCode, HomeZone, BoundaryCode, LastCompletedAtLocal);
        }
    }
}
