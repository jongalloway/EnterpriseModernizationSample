using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class DispatchCoordinatorFixture
    {
        [Test]
        public void GetActiveTickets_returns_stubbed_ticket_details_for_requested_store()
        {
            var coordinator = new DispatchCoordinator();

            var tickets = coordinator.GetActiveTickets("027");

            Assert.That(tickets, Has.Count.EqualTo(2));
            Assert.That(tickets.Select(ticket => ticket.StoreNumber), Is.All.EqualTo("027"));
            Assert.That(tickets.Select(ticket => ticket.DriverCode), Does.Contain("DRV-17"));
        }
    }
}
