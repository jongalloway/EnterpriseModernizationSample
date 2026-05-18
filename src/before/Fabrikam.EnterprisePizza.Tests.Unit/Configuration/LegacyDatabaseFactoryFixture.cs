using System.Collections.Specialized;
using System.Configuration;
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
            var factory = new LegacyDatabaseFactory(connectionCatalog);

            var database = factory.CreateDatabase(LegacyDatabaseArea.Reporting);

            Assert.That(database.ConnectionName, Is.EqualTo("FabrikamPizza_Reporting"));
            Assert.That(database.ProviderInvariantName, Is.EqualTo("System.Data.SqlClient"));
        }

        [Test]
        public void CreateDatabase_resolves_named_area_to_default_connection_name()
        {
            var factory = new LegacyDatabaseFactory(connectionCatalog);

            var database = factory.CreateDatabase("StoreOps");

            Assert.That(database.ConnectionName, Is.EqualTo("FabrikamPizza_StoreOps"));
            Assert.That(database.ConnectionString, Does.Contain("FabrikamPizza_StoreOps"));
        }

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
