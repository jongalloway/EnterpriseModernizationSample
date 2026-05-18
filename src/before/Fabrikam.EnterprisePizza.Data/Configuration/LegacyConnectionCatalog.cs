using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Core.Configuration;

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

        private readonly EnterpriseLibraryConfigurationReader configurationReader;

        public LegacyConnectionCatalog()
            : this(new EnterpriseLibraryConfigurationReader())
        {
        }

        public LegacyConnectionCatalog(EnterpriseLibraryConfigurationReader configurationReader)
        {
            this.configurationReader = configurationReader ?? throw new ArgumentNullException(nameof(configurationReader));
        }

        public string GetConnectionName(LegacyDatabaseArea area)
        {
            return configurationReader.GetConnectionName(area.ToString(), GetDefaultConnectionName(area));
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

        public static string GetDefaultConnectionName(LegacyDatabaseArea area)
        {
            switch (area)
            {
                case LegacyDatabaseArea.StoreOps:
                    return "FabrikamPizza_StoreOps";
                case LegacyDatabaseArea.CustomerHub:
                    return "FabrikamPizza_CustomerHub";
                case LegacyDatabaseArea.Reporting:
                    return "FabrikamPizza_Reporting";
                default:
                    throw new ArgumentOutOfRangeException(nameof(area), area, "Unknown legacy database area.");
            }
        }
    }
}
