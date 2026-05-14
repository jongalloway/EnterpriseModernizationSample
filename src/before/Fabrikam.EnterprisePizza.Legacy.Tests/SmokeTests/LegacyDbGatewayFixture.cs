using Fabrikam.EnterprisePizza.Data.Gateways;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class LegacyDbGatewayFixture
    {
        [TestCase("StoreOps", "FabrikamPizza_StoreOps")]
        [TestCase("CustomerHub", "FabrikamPizza_CustomerHub")]
        [TestCase("Reporting", "FabrikamPizza_Reporting")]
        public void GetConnectionName_returns_expected_catalog_alias(string area, string expectedConnectionName)
        {
            var gateway = new LegacyDbGateway();

            var connectionName = gateway.GetConnectionName(area);

            Assert.That(connectionName, Is.EqualTo(expectedConnectionName));
        }
    }
}
