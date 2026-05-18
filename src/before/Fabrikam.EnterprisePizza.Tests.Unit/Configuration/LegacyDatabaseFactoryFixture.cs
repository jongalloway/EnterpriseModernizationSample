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
