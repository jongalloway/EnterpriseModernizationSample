using System.Collections.Specialized;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Core.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Composition
{
    [TestFixture]
    public class LegacyServiceLocatorFixture
    {
        [Test]
        public void Initialize_uses_configuration_reader_to_build_container_with_expected_lifetimes()
        {
            var settings = new NameValueCollection
            {
                ["enterpriseLibrary:typeAlias:LegacyDbGateway"] = "Fabrikam.EnterprisePizza.Data.Gateways.LegacyDbGateway, Fabrikam.EnterprisePizza.Data",
                ["enterpriseLibrary:typeAlias:DispatchTicketRepository"] = "Fabrikam.EnterprisePizza.Data.Repositories.StoreOps.DispatchTicketRepository, Fabrikam.EnterprisePizza.Data",
                ["enterpriseLibrary:typeAlias:DispatchCoordinator"] = "Fabrikam.EnterprisePizza.Business.StoreOps.Services.DispatchCoordinator, Fabrikam.EnterprisePizza.Business.StoreOps",
                ["enterpriseLibrary:container:DispatchHost:001"] = "service=LegacyDbGateway;mapTo=LegacyDbGateway;lifetime=Singleton",
                ["enterpriseLibrary:container:DispatchHost:002"] = "service=Fabrikam.EnterprisePizza.Data.Repositories.StoreOps.IDispatchTicketRepository, Fabrikam.EnterprisePizza.Data;mapTo=DispatchTicketRepository;lifetime=Transient",
                ["enterpriseLibrary:container:DispatchHost:003"] = "service=Fabrikam.EnterprisePizza.Business.StoreOps.Services.IDispatchCoordinator, Fabrikam.EnterprisePizza.Business.StoreOps;mapTo=DispatchCoordinator;lifetime=Transient"
            };

            LegacyServiceLocator.Initialize(new EnterpriseLibraryConfigurationReader(settings), "DispatchHost");

            var gateway = LegacyServiceLocator.Resolve<LegacyDbGateway>();
            var secondGateway = LegacyServiceLocator.Resolve<LegacyDbGateway>();
            var coordinator = LegacyServiceLocator.Resolve<IDispatchCoordinator>();

            Assert.That(gateway, Is.SameAs(secondGateway));
            Assert.That(coordinator, Is.TypeOf<DispatchCoordinator>());
        }
    }
}
