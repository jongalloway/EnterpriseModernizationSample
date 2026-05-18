using System;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Gateways
{
    [TestFixture]
    public class LegacyDbGatewayFixture
    {
        private LegacyDbGateway gateway;

        [SetUp]
        public void SetUp()
        {
            gateway = new LegacyDbGateway();
        }

        [TearDown]
        public void TearDown()
        {
            gateway = null;
        }

        [Test]
        public void CreateStoredProcedureCall_preserves_connection_procedure_and_parameters()
        {
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
            var call = gateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.StoreOps,
                LegacyStoredProcedures.StoreOps.GetActiveDispatchTickets,
                new GatewayParameter("@StoreNumber", "021"));

            var dataSet = gateway.ExecuteDataSet(call);
            var table = dataSet.Tables["DispatchTickets"];

            Assert.That(table, Is.Not.Null);
            Assert.That(table.Columns["TicketId"].DataType, Is.EqualTo(typeof(int)));
            Assert.That(table.Rows, Has.Count.EqualTo(2));
            Assert.That(table.Rows[0]["StoreNumber"], Is.EqualTo("021"));
            Assert.That(table.Rows[1]["RouteZone"], Is.EqualTo("Mall Annex"));
        }

        [Test]
        public void ExecuteDataSet_returns_empty_batch_table_for_unknown_store()
        {
            var call = gateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.StoreOps,
                LegacyStoredProcedures.StoreOps.GetLatestPosImportBatch,
                new GatewayParameter("@StoreNumber", "999"));

            var dataSet = gateway.ExecuteDataSet(call);
            var table = dataSet.Tables["LatestPosImportBatch"];

            Assert.That(table, Is.Not.Null);
            Assert.That(table.Rows, Has.Count.EqualTo(0));
            Assert.That(table.Columns["BatchStatus"].DataType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void ExecuteDataSet_rejects_unknown_procedure_wrappers()
        {
            var call = new StoredProcedureCall("FabrikamPizza_StoreOps", "dbo.usp_Unknown");

            var ex = Assert.Throws<InvalidOperationException>(() => gateway.ExecuteDataSet(call));

            Assert.That(ex.Message, Does.Contain("dbo.usp_Unknown"));
        }

        [Test]
        public void ExecuteDataSet_rejects_null_call()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => gateway.ExecuteDataSet(null));

            Assert.That(ex.ParamName, Is.EqualTo("call"));
        }
    }
}
