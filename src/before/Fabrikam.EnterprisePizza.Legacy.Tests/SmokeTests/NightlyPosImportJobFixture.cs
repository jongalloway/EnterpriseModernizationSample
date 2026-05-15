using Fabrikam.EnterprisePizza.Integrations.PosSync.Jobs;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class NightlyPosImportJobFixture
    {
        [Test]
        public void GetTargetDatabase_routes_imports_to_storeops_connection()
        {
            var job = new NightlyPosImportJob();

            var connectionName = job.GetTargetDatabase();

            Assert.That(connectionName, Is.EqualTo("FabrikamPizza_StoreOps"));
        }
    }
}
