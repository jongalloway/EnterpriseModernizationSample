using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class StoreOperationsWorkbenchServiceFixture
    {
        [Test]
        public void GetOrderLookupRecords_returns_store_specific_grid_rows()
        {
            var service = new StoreOperationsWorkbenchService();
            var orders = service.GetOrderLookupRecords("014");

            Assert.That(orders.Count, Is.GreaterThanOrEqualTo(4));
            Assert.That(orders.All(order => order.StoreNumber == "014"), Is.True);
        }

        [Test]
        public void GetStoreManagementRecords_returns_follow_up_and_normal_stores()
        {
            var service = new StoreOperationsWorkbenchService();
            var stores = service.GetStoreManagementRecords();

            Assert.That(stores.Count, Is.GreaterThanOrEqualTo(4));
            Assert.That(stores.Any(store => store.StoreStatus == "Needs Follow-Up"), Is.True);
            Assert.That(stores.Any(store => store.StoreStatus == "Normal"), Is.True);
        }
    }
}
