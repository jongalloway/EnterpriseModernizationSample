using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Configuration
{
    [TestFixture]
    public class LegacyConnectionCatalogFixture
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

        [TestCase("StoreOps", "FabrikamPizza_StoreOps")]
        [TestCase(" CustomerHub ", "FabrikamPizza_CustomerHub")]
        [TestCase("Reporting", "FabrikamPizza_Reporting")]
        public void GetConnectionName_returns_expected_catalog_entry(string area, string expectedConnectionName)
        {
            var connectionName = connectionCatalog.GetConnectionName(area);

            Assert.That(connectionName, Is.EqualTo(expectedConnectionName));
        }

        [Test]
        public void GetConnectionName_rejects_unknown_database_area()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => connectionCatalog.GetConnectionName("Payroll"));

            Assert.That(ex.ParamName, Is.EqualTo("area"));
        }

        [Test]
        public void GetConnectionName_rejects_blank_area_values()
        {
            var ex = Assert.Throws<ArgumentException>(() => connectionCatalog.GetConnectionName("   "));

            Assert.That(ex.ParamName, Is.EqualTo("area"));
        }
    }
}
