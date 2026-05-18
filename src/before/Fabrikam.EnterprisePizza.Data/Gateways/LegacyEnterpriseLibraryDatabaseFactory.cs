using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Fabrikam.EnterprisePizza.Data.Gateways
{
    public class LegacyEnterpriseLibraryDatabaseFactory
    {
        private readonly LegacyConnectionCatalog connectionCatalog;
        private readonly DatabaseProviderFactory databaseProviderFactory;

        public LegacyEnterpriseLibraryDatabaseFactory()
            : this(new LegacyConnectionCatalog(), new DatabaseProviderFactory())
        {
        }

        public LegacyEnterpriseLibraryDatabaseFactory(LegacyConnectionCatalog connectionCatalog, DatabaseProviderFactory databaseProviderFactory)
        {
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
            this.databaseProviderFactory = databaseProviderFactory ?? throw new ArgumentNullException(nameof(databaseProviderFactory));
        }

        public Database Create(LegacyDatabaseArea area)
        {
            return Create(connectionCatalog.GetConnectionName(area));
        }

        public Database Create(string connectionName)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ArgumentException("A connection name is required.", nameof(connectionName));
            }

            return databaseProviderFactory.Create(connectionName.Trim());
        }
    }
}
