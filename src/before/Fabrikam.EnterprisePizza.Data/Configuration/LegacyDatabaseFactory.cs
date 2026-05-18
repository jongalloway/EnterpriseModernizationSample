using System;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Fabrikam.EnterprisePizza.Data.Configuration
{
    public class LegacyDatabaseFactory
    {
        private readonly LegacyConnectionCatalog connectionCatalog;
        private readonly Func<string, Database> databaseResolver;

        public LegacyDatabaseFactory()
            : this(new LegacyConnectionCatalog())
        {
        }

        public LegacyDatabaseFactory(LegacyConnectionCatalog connectionCatalog)
            : this(connectionCatalog, CreateDatabaseWithEnterpriseLibrary)
        {
        }

        public LegacyDatabaseFactory(LegacyConnectionCatalog connectionCatalog, Func<string, Database> databaseResolver)
        {
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
            this.databaseResolver = databaseResolver ?? throw new ArgumentNullException(nameof(databaseResolver));
        }

        public Database CreateDatabase(LegacyDatabaseArea area)
        {
            return databaseResolver(connectionCatalog.GetConnectionName(area));
        }

        public Database CreateDatabase(string area)
        {
            return databaseResolver(connectionCatalog.GetConnectionName(area));
        }

        private static Database CreateDatabaseWithEnterpriseLibrary(string connectionName)
        {
            var providerFactory = new DatabaseProviderFactory();
            var database = providerFactory.Create(connectionName);
            if (database == null)
            {
                throw new InvalidOperationException("Enterprise Library could not resolve database '" + connectionName + "'.");
            }

            return database;
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
