using System;
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
        private readonly LegacyConnectionCatalog connectionCatalog;
        private readonly EnterpriseLibraryConfigurationReader configurationReader;

        public LegacyDatabaseFactory()
            : this(new LegacyConnectionCatalog(), new EnterpriseLibraryConfigurationReader())
        {
        }

        public LegacyDatabaseFactory(LegacyConnectionCatalog connectionCatalog)
            : this(connectionCatalog, new EnterpriseLibraryConfigurationReader())
        {
        }

        public LegacyDatabaseFactory(EnterpriseLibraryConfigurationReader configurationReader)
            : this(new LegacyConnectionCatalog(configurationReader), configurationReader)
        {
        }

        private LegacyDatabaseFactory(LegacyConnectionCatalog connectionCatalog, EnterpriseLibraryConfigurationReader configurationReader)
        {
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
            this.configurationReader = configurationReader ?? throw new ArgumentNullException(nameof(configurationReader));
        }

        public LegacyDatabase CreateDatabase(LegacyDatabaseArea area)
        {
            return CreateDatabase(connectionCatalog.GetConnectionName(area));
        }

        public LegacyDatabase CreateDatabase(string areaOrConnectionName)
        {
            if (string.IsNullOrWhiteSpace(areaOrConnectionName))
            {
                throw new ArgumentException("A connection name is required.", nameof(areaOrConnectionName));
            }

            var trimmedValue = areaOrConnectionName.Trim();
            string connectionName;
            try
            {
                connectionName = connectionCatalog.GetConnectionName(trimmedValue);
            }
            catch (ArgumentOutOfRangeException)
            {
                connectionName = trimmedValue;
            }

            var settings = configurationReader.GetConnectionStringSettings(connectionName);
            if (settings == null)
            {
                return new LegacyDatabase(
                    connectionName,
                    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=" + connectionName + ";Integrated Security=True;",
                    "System.Data.SqlClient");
            }

            return new LegacyDatabase(
                settings.Name,
                settings.ConnectionString,
                string.IsNullOrWhiteSpace(settings.ProviderName) ? "System.Data.SqlClient" : settings.ProviderName);
        }
    }
}
