using System;
using System.Data;
using Fabrikam.EnterprisePizza.Core.Domain;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Repositories.StoreOps;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Fabrikam.EnterprisePizza.Tests.Unit.Testing;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Repositories.StoreOps
{
    [TestFixture]
    public class PosImportBatchRepositoryInteractionFixture
    {
        private RecordingLegacyDbGateway gateway;
        private PosImportBatchRepository repository;

        [SetUp]
        public void SetUp()
        {
            gateway = new RecordingLegacyDbGateway();
            repository = new PosImportBatchRepository(gateway);
        }

        [TearDown]
        public void TearDown()
        {
            gateway = null;
            repository = null;
        }

        [Test]
        public void GetLatestBatch_builds_store_lookup_and_maps_first_row()
        {
            gateway.DataSetToReturn = CreateLatestBatchDataSet(new DateTime(2026, 5, 18, 6, 15, 0, DateTimeKind.Utc));

            var batch = repository.GetLatestBatch("019");

            Assert.That(gateway.LastArea, Is.EqualTo(LegacyDatabaseArea.StoreOps));
            Assert.That(gateway.LastProcedureName, Is.EqualTo(LegacyStoredProcedures.StoreOps.GetLatestPosImportBatch));
            Assert.That(gateway.LastCreateParameters[0].Value, Is.EqualTo("019"));
            Assert.That(batch, Is.Not.Null);
            Assert.That(batch.PosOrderImportBatchId, Is.EqualTo(9004));
            Assert.That(batch.BatchStatus, Is.EqualTo("PendingReview"));
        }

        [Test]
        public void GetLatestBatch_defaults_missing_columns_without_throwing()
        {
            gateway.DataSetToReturn = CreateSparseBatchDataSet();

            PosImportBatchSnapshot batch = repository.GetLatestBatch("019");

            Assert.That(batch, Is.Not.Null);
            Assert.That(batch.SourceSystem, Is.Null);
            Assert.That(batch.ItemCount, Is.EqualTo(0));
            Assert.That(batch.BatchDate, Is.EqualTo(DateTime.MinValue));
        }

        private static DataSet CreateLatestBatchDataSet(DateTime importedUtc)
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add("LatestPosImportBatch");
            table.Columns.Add("PosOrderImportBatchId", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("SourceSystem", typeof(string));
            table.Columns.Add("BatchDate", typeof(DateTime));
            table.Columns.Add("ImportedUtc", typeof(DateTime));
            table.Columns.Add("BatchStatus", typeof(string));
            table.Columns.Add("ItemCount", typeof(int));
            table.Rows.Add(9004, "019", "StorefrontPOS", new DateTime(2026, 5, 17), importedUtc, "PendingReview", 11);
            return dataSet;
        }

        private static DataSet CreateSparseBatchDataSet()
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add("LatestPosImportBatch");
            table.Columns.Add("PosOrderImportBatchId", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Rows.Add(9005, "019");
            return dataSet;
        }
    }
}
