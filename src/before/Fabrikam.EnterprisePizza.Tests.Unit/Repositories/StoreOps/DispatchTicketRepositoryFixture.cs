using System.Linq;
using Fabrikam.EnterprisePizza.Data.Repositories.StoreOps;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Repositories.StoreOps
{
    [TestFixture]
    public class DispatchTicketRepositoryFixture
    {
        [Test]
        public void GetActiveTickets_maps_dispatch_rows_for_requested_store()
        {
            var repository = new DispatchTicketRepository();

            var tickets = repository.GetActiveTickets("042");

            Assert.That(tickets, Has.Count.EqualTo(2));
            Assert.That(tickets.Select(ticket => ticket.StoreNumber), Is.All.EqualTo("042"));
            Assert.That(tickets.Select(ticket => ticket.TicketId), Is.EqualTo(new[] { 4105, 4106 }));
            Assert.That(tickets[1].RouteZone, Is.EqualTo("Mall Annex"));
        }
    }
}
