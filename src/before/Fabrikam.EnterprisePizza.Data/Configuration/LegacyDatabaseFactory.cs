using System;
using System.Configuration;
using Fabrikam.EnterprisePizza.Core.Configuration;

namespace Fabrikam.EnterprisePizza.Data.Configuration
{
    public sealed class LegacyDatabase
    {
        public LegacyDatabase(string connectionName, string connectionString, string providerInvariantName)
        {
            ConnectionName = connectionName;
            ConnectionString = connectionString;
            ProviderInvariantName = providerInvariantName;
        }

        public string ConnectionName { get; private set; }

        public string ConnectionString { get; private set; }

        public string ProviderInvariantName { get; private set; }
    }

    public class LegacyDatabaseFactory
    {
        private readonly EnterpriseLibraryConfigurationReader configurationReader;

        public LegacyDatabaseFactory()
            : this(new EnterpriseLibraryConfigurationReader())
        {
        }

        public LegacyDatabaseFactory(EnterpriseLibraryConfigurationReader configurationReader)
        {
            this.configurationReader = configurationReader ?? throw new ArgumentNullException(nameof(configurationReader));
        }

        public LegacyDatabase CreateDatabase(LegacyDatabaseArea area)
        {
            return CreateDatabase(configurationReader.GetConnectionName(area.ToString(), LegacyConnectionCatalog.GetDefaultConnectionName(area)));
        }

        public LegacyDatabase CreateDatabase(string connectionName)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ArgumentException("A connection name is required.", nameof(connectionName));
            }

            var settings = configurationReader.GetConnectionStringSettings(connectionName.Trim());
            if (settings == null)
            {
                return new LegacyDatabase(
                    connectionName.Trim(),
                    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=" + connectionName.Trim() + ";Integrated Security=True;",
                    "System.Data.SqlClient");
            }

            return new LegacyDatabase(
                settings.Name,
                settings.ConnectionString,
                string.IsNullOrWhiteSpace(settings.ProviderName) ? "System.Data.SqlClient" : settings.ProviderName);
        }
    }
}
