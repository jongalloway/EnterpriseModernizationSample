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

            Assert.That(tickets.Count, Is.GreaterThan(0));
        }
    }
}
