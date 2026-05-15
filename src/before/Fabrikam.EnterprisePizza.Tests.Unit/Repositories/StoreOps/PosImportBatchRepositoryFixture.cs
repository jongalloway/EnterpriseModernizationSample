using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.Repositories.StoreOps;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Repositories.StoreOps
{
    [TestFixture]
    public class PosImportBatchRepositoryFixture
    {
        [Test]
        public void GetLatestBatch_maps_store_specific_batch_details()
        {
            var repository = new PosImportBatchRepository(new LegacyDbGateway());

            var batch = repository.GetLatestBatch("022");

            Assert.That(batch, Is.Not.Null);
            Assert.That(batch.PosOrderImportBatchId, Is.EqualTo(8802));
            Assert.That(batch.StoreNumber, Is.EqualTo("022"));
            Assert.That(batch.SourceSystem, Is.EqualTo("RedmondPOS"));
            Assert.That(batch.BatchStatus, Is.EqualTo("Partial"));
            Assert.That(batch.ItemCount, Is.EqualTo(34));
        }

        [Test]
        public void GetLatestBatch_returns_null_when_stub_has_no_rows()
        {
            var repository = new PosImportBatchRepository(new LegacyDbGateway());

            var batch = repository.GetLatestBatch("999");

            Assert.That(batch, Is.Null);
        }
    }
}
