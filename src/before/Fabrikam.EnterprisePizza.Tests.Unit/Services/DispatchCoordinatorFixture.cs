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
            Assert.That(tickets.Any(ticket => ticket.PriorityScore >= 80), Is.True);
        }

        [Test]
        public void GetDispatchQueue_returns_priority_ordered_entries_for_store()
        {
            var coordinator = new DispatchCoordinator();

            var queue = coordinator.GetDispatchQueue("014");

            Assert.That(queue, Is.Not.Empty);
            Assert.That(queue[0].PriorityRank, Is.GreaterThanOrEqualTo(queue[queue.Count - 1].PriorityRank));
            Assert.That(queue.Select(entry => entry.SuggestedDriverCode), Does.Contain("DRV-17"));
        }
    }
}
