using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Configuration;
using System.Collections.Specialized;
using Fabrikam.EnterprisePizza.Core.Configuration;
using Fabrikam.EnterprisePizza.Data.Configuration;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Configuration
{
    [TestFixture]
    public class LegacyDatabaseFactoryFixture
    {
        private LegacyConnectionCatalog connectionCatalog;

        [SetUp]
        public void SetUp()
        {
            connectionCatalog = new LegacyConnectionCatalog();
        }

        [TearDown]
        public void TearDown()
        {
            connectionCatalog = null;
        }

        [Test]
        public void CreateDatabase_uses_catalog_mapping_for_enum_area()
        {
            string resolvedName = null;
            var factory = new LegacyDatabaseFactory(
                connectionCatalog,
                name =>
                {
                    resolvedName = name;
                    return new SqlDatabase("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=" + name + ";Integrated Security=True");
                });

            var database = factory.CreateDatabase(LegacyDatabaseArea.Reporting);

            Assert.That(resolvedName, Is.EqualTo("FabrikamPizza_Reporting"));
            Assert.That(database, Is.Not.Null);
            Assert.That(database.GetType().Name, Is.EqualTo("SqlDatabase"));
        }

        [Test]
        public void CreateDatabase_resolves_enterprise_library_database_for_named_area()
        {
            var factory = new LegacyDatabaseFactory(connectionCatalog);

            var database = factory.CreateDatabase("StoreOps");

            Assert.That(database, Is.Not.Null);
            Assert.That(database.GetType().Name, Is.EqualTo("SqlDatabase"));
        }

        [Test]
        public void Constructor_rejects_null_database_resolver()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new LegacyDatabaseFactory(connectionCatalog, null));

            Assert.That(ex.ParamName, Is.EqualTo("databaseResolver"));
        [Test]
        public void CreateDatabase_prefers_enterprise_library_mapping_and_connection_string_catalog()
        {
            var settings = new NameValueCollection
            {
                ["enterpriseLibrary:database:StoreOps"] = "StoreOpsConnection"
            };
            var connectionStrings = new ConnectionStringSettingsCollection();
            connectionStrings.Add(new ConnectionStringSettings(
                "StoreOpsConnection",
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FabrikamPizza_StoreOps;Integrated Security=True;",
                "System.Data.SqlClient"));

            var factory = new LegacyDatabaseFactory(new EnterpriseLibraryConfigurationReader(settings, connectionStrings));
            var database = factory.CreateDatabase(LegacyDatabaseArea.StoreOps);

            Assert.That(database.ConnectionName, Is.EqualTo("StoreOpsConnection"));
            Assert.That(database.ProviderInvariantName, Is.EqualTo("System.Data.SqlClient"));
            Assert.That(database.ConnectionString, Does.Contain("FabrikamPizza_StoreOps"));
        }
    }
}
