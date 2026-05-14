using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Fabrikam.EnterprisePizza.Services.DispatchHost;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class StoreDispatchServiceFixture
    {
        [Test]
        public void GetDispatchBoard_returns_stubbed_dispatch_tickets_for_requested_store()
        {
            var service = new StoreDispatchService();

            var tickets = service.GetDispatchBoard("027");

            Assert.That(tickets, Has.Count.EqualTo(2));
            Assert.That(tickets.Select(ticket => ticket.StoreNumber), Is.All.EqualTo("027"));
            Assert.That(tickets.Select(ticket => ticket.DriverCode), Does.Contain("DRV-17"));
            Assert.That(tickets.Select(ticket => ticket.RouteZone), Does.Contain("Northwest Corporate Corridor"));
        }

        [Test]
        public void DispatchHost_exposes_service_contract_host_directive_and_metadata_endpoint()
        {
            Assert.That(typeof(IStoreDispatchService).GetCustomAttribute<ServiceContractAttribute>(), Is.Not.Null);

            var dispatchBoardOperation = typeof(IStoreDispatchService).GetMethod("GetDispatchBoard");
            Assert.That(dispatchBoardOperation, Is.Not.Null);
            Assert.That(dispatchBoardOperation.GetCustomAttribute<OperationContractAttribute>(), Is.Not.Null);

            var hostDirective = File.ReadAllText(LegacyServiceSmokePaths.Combine("Fabrikam.EnterprisePizza.Services.DispatchHost", "StoreDispatchService.svc"));
            Assert.That(hostDirective, Does.Contain("Service=\"Fabrikam.EnterprisePizza.Services.DispatchHost.StoreDispatchService\""));

            var webConfig = File.ReadAllText(LegacyServiceSmokePaths.Combine("Fabrikam.EnterprisePizza.Services.DispatchHost", "web.config"));
            Assert.That(webConfig, Does.Contain("basicHttpBinding"));
            Assert.That(webConfig, Does.Contain("bindingConfiguration=\"DispatchBasicHttpBinding\""));
            Assert.That(webConfig, Does.Contain("serviceMetadata httpGetEnabled=\"true\""));
            Assert.That(webConfig, Does.Contain("endpoint address=\"mex\""));
        }
    }
}
