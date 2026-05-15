using Fabrikam.EnterprisePizza.Core.Domain;
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

        [Test]
        public void GetLatestImportedBatch_returns_stubbed_storeops_batch_snapshot()
        {
            var job = new NightlyPosImportJob();

            PosImportBatchSnapshot batch = job.GetLatestImportedBatch("014");

            Assert.That(batch, Is.Not.Null);
            Assert.That(batch.StoreNumber, Is.EqualTo("014"));
            Assert.That(batch.SourceSystem, Is.EqualTo("CampusPOS"));
            Assert.That(batch.BatchStatus, Is.EqualTo("Complete"));
            Assert.That(batch.ItemCount, Is.EqualTo(57));
        }
    }
}
