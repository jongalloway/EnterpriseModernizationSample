using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class DispatchCoordinatorFixture
    {
        [Test]
        public void GetActiveTickets_returns_sample_routes_for_store()
        {
            var coordinator = new DispatchCoordinator();
            var tickets = coordinator.GetActiveTickets("014");

            Assert.That(tickets, Has.Count.EqualTo(2));
            Assert.That(tickets.Select(ticket => ticket.StoreNumber), Is.All.EqualTo("014"));
            Assert.That(tickets.Select(ticket => ticket.DriverCode), Does.Contain("DRV-17"));
            Assert.That(tickets.Select(ticket => ticket.RouteZone), Does.Contain("Mall Annex"));
        }
    }
}
