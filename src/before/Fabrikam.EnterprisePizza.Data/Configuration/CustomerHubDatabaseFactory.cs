using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Fabrikam.EnterprisePizza.Data.Configuration
{
    public class CustomerHubDatabaseFactory
    {
        private readonly LegacyConnectionCatalog _connectionCatalog;

        public CustomerHubDatabaseFactory()
            : this(new LegacyConnectionCatalog())
        {
        }

        public CustomerHubDatabaseFactory(LegacyConnectionCatalog connectionCatalog)
        {
            _connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
        }

        public string ConnectionName
        {
            get { return _connectionCatalog.GetConnectionName(LegacyDatabaseArea.CustomerHub); }
        }

        public Database CreateDatabase()
        {
            return new DatabaseProviderFactory().Create(ConnectionName);
        }
    }
}
