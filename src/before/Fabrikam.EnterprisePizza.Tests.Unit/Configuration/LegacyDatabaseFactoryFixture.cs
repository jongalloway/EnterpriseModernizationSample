using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
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
        }
    }
}
