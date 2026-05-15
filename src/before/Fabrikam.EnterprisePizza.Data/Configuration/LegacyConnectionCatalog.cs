using System;
using System.Collections.Generic;

namespace Fabrikam.EnterprisePizza.Data.Configuration
{
    public class LegacyConnectionCatalog
    {
        private static readonly IDictionary<string, LegacyDatabaseArea> AreaAliases =
            new Dictionary<string, LegacyDatabaseArea>(StringComparer.OrdinalIgnoreCase)
            {
                { "StoreOps", LegacyDatabaseArea.StoreOps },
                { "CustomerHub", LegacyDatabaseArea.CustomerHub },
                { "Reporting", LegacyDatabaseArea.Reporting }
            };

        private readonly IDictionary<LegacyDatabaseArea, string> connectionNames;

        public LegacyConnectionCatalog()
        {
            connectionNames = new Dictionary<LegacyDatabaseArea, string>
            {
                { LegacyDatabaseArea.StoreOps, "FabrikamPizza_StoreOps" },
                { LegacyDatabaseArea.CustomerHub, "FabrikamPizza_CustomerHub" },
                { LegacyDatabaseArea.Reporting, "FabrikamPizza_Reporting" }
            };
        }

        public string GetConnectionName(LegacyDatabaseArea area)
        {
            if (!connectionNames.ContainsKey(area))
            {
                throw new ArgumentOutOfRangeException(nameof(area), area, "Unknown legacy database area.");
            }

            return connectionNames[area];
        }

        public string GetConnectionName(string area)
        {
            if (string.IsNullOrWhiteSpace(area))
            {
                throw new ArgumentException("A legacy database area is required.", nameof(area));
            }

            LegacyDatabaseArea databaseArea;
            if (!AreaAliases.TryGetValue(area.Trim(), out databaseArea))
            {
                throw new ArgumentOutOfRangeException(nameof(area), area, "Unknown legacy database area.");
            }

            return GetConnectionName(databaseArea);
        }
    }
}
