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
        }
    }
}
