using System;
using System.Collections.Generic;

namespace Fabrikam.EnterprisePizza.Data.Configuration
{
    public class LegacyConnectionCatalog
    {
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
    }
}
