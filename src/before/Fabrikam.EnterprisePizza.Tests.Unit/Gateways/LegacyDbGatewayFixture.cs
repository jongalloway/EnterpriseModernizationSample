using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Gateways
{
    [TestFixture]
    public class LegacyDbGatewayFixture
    {
        [Test]
        public void CreateStoredProcedureCall_preserves_connection_procedure_and_parameters()
        {
            var gateway = new LegacyDbGateway();

            var call = gateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.StoreOps,
                LegacyStoredProcedures.StoreOps.GetActiveDispatchTickets,
                new GatewayParameter("@StoreNumber", "021"));

            Assert.That(call.ConnectionName, Is.EqualTo("FabrikamPizza_StoreOps"));
            Assert.That(call.StoredProcedureName, Is.EqualTo(LegacyStoredProcedures.StoreOps.GetActiveDispatchTickets));
            Assert.That(call.Parameters, Has.Count.EqualTo(1));
            Assert.That(call.Parameters[0].Name, Is.EqualTo("@StoreNumber"));
            Assert.That(call.Parameters[0].Value, Is.EqualTo("021"));
        }

        [Test]
        public void ExecuteDataSet_returns_dispatch_stub_rows_for_requested_store()
        {
            var gateway = new LegacyDbGateway();
            var call = gateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.StoreOps,
                LegacyStoredProcedures.StoreOps.GetActiveDispatchTickets,
                new GatewayParameter("@StoreNumber", "021"));

            var dataSet = gateway.ExecuteDataSet(call);
            var table = dataSet.Tables["DispatchTickets"];

            Assert.That(table, Is.Not.Null);
            Assert.That(table.Rows, Has.Count.EqualTo(2));
            Assert.That(table.Rows[0]["StoreNumber"], Is.EqualTo("021"));
            Assert.That(table.Rows[1]["RouteZone"], Is.EqualTo("Mall Annex"));
        }

        [Test]
        public void ExecuteDataSet_returns_latest_pos_import_batch_for_requested_store()
        {
            var gateway = new LegacyDbGateway();
            var call = gateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.StoreOps,
                LegacyStoredProcedures.StoreOps.GetLatestPosImportBatch,
                new GatewayParameter("@StoreNumber", "022"));

            var dataSet = gateway.ExecuteDataSet(call);
            var table = dataSet.Tables["LatestPosImportBatch"];

            Assert.That(table, Is.Not.Null);
            Assert.That(table.Rows, Has.Count.EqualTo(1));
            Assert.That(table.Rows[0]["StoreNumber"], Is.EqualTo("022"));
            Assert.That(table.Rows[0]["SourceSystem"], Is.EqualTo("RedmondPOS"));
            Assert.That(table.Rows[0]["ItemCount"], Is.EqualTo(34));
        }
    }
}
