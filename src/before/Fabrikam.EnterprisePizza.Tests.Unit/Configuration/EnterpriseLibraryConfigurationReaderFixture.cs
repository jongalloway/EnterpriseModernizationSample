using System.Collections.Specialized;
using System.Linq;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Core.Configuration;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Configuration
{
    [TestFixture]
    public class EnterpriseLibraryConfigurationReaderFixture
    {
        [Test]
        public void ReadContainerRegistrations_parses_enterprise_library_style_registration_rows()
        {
            var settings = new NameValueCollection
            {
                ["enterpriseLibrary:container:DispatchHost:001"] = "service=LegacyDbGateway;mapTo=LegacyDbGateway;lifetime=Singleton",
                ["enterpriseLibrary:typeAlias:LegacyDbGateway"] = "Fabrikam.EnterprisePizza.Data.Gateways.LegacyDbGateway, Fabrikam.EnterprisePizza.Data"
            };

            var reader = new EnterpriseLibraryConfigurationReader(settings);
            var registrations = reader.ReadContainerRegistrations("DispatchHost").ToArray();

            Assert.That(registrations, Has.Length.EqualTo(1));
            Assert.That(registrations[0].ServiceTypeToken, Is.EqualTo("LegacyDbGateway"));
            Assert.That(registrations[0].MapToTypeToken, Is.EqualTo("LegacyDbGateway"));
            Assert.That(registrations[0].Lifetime, Is.EqualTo(LegacyLifetime.Singleton));
        }

        [Test]
        public void GetConnectionName_returns_configured_database_alias_when_present()
        {
            var settings = new NameValueCollection
            {
                ["enterpriseLibrary:database:StoreOps"] = "StoreOpsConnection"
            };

            var reader = new EnterpriseLibraryConfigurationReader(settings);
            var connectionName = reader.GetConnectionName("StoreOps", "FabrikamPizza_StoreOps");

            Assert.That(connectionName, Is.EqualTo("StoreOpsConnection"));
        }
    }
}
