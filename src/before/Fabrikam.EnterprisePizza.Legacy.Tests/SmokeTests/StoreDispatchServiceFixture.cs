using System.Linq;
using Fabrikam.EnterprisePizza.Services.DispatchHost;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class StoreDispatchServiceFixture
    {
        [Test]
        public void GetDispatchBoard_returns_store_specific_stubbed_tickets()
        {
            var service = new StoreDispatchService();

            var tickets = service.GetDispatchBoard("021");

            Assert.That(tickets, Has.Count.EqualTo(2));
            Assert.That(tickets.Select(ticket => ticket.StoreNumber), Is.All.EqualTo("021"));
            Assert.That(tickets.Select(ticket => ticket.DriverCode), Does.Contain("DRV-17"));
        }
    }
}
