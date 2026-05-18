using System;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Repositories.StoreOps;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Fabrikam.EnterprisePizza.Tests.Unit.Testing;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Repositories.StoreOps
{
    [TestFixture]
    public class DispatchTicketRepositoryInteractionFixture
    {
        private RecordingLegacyDbGateway gateway;
        private DispatchTicketRepository repository;

        [SetUp]
        public void SetUp()
        {
            gateway = new RecordingLegacyDbGateway();
            repository = new DispatchTicketRepository(gateway);
        }

        [TearDown]
        public void TearDown()
        {
            gateway = null;
            repository = null;
        }

        [Test]
        public void GetActiveTickets_builds_storeops_lookup_and_maps_dispatch_rows()
        {
            gateway.DataSetToReturn = CreateDispatchTicketsDataSet();

            var tickets = repository.GetActiveTickets("031");

            Assert.That(gateway.LastArea, Is.EqualTo(LegacyDatabaseArea.StoreOps));
            Assert.That(gateway.LastProcedureName, Is.EqualTo(LegacyStoredProcedures.StoreOps.GetActiveDispatchTickets));
            Assert.That(gateway.LastCreateParameters, Has.Length.EqualTo(1));
            Assert.That(gateway.LastCreateParameters[0].Name, Is.EqualTo("@StoreNumber"));
            Assert.That(gateway.LastCreateParameters[0].Value, Is.EqualTo("031"));
            Assert.That(gateway.LastExecutedCall.ConnectionName, Is.EqualTo("FabrikamPizza_StoreOps"));
            Assert.That(tickets, Has.Count.EqualTo(2));
            Assert.That(tickets[0].TicketId, Is.EqualTo(5001));
            Assert.That(tickets[1].DriverCode, Is.EqualTo("DRV-88"));
        }

        [Test]
        public void GetActiveTickets_returns_empty_list_when_dispatch_table_missing()
        {
            gateway.DataSetToReturn = new DataSet();

            var tickets = repository.GetActiveTickets("031");

            Assert.That(tickets, Is.Empty);
        }

        [Test]
        public void Constructor_rejects_null_gateway()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new DispatchTicketRepository(null));

            Assert.That(ex.ParamName, Is.EqualTo("dbGateway"));
        }

        private static DataSet CreateDispatchTicketsDataSet()
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add("DispatchTickets");
            table.Columns.Add("TicketId", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("DriverCode", typeof(string));
            table.Columns.Add("RouteZone", typeof(string));
            table.Rows.Add(5001, "031", "DRV-17", "Airport Corridor");
            table.Rows.Add(5002, "031", "DRV-88", "Conference Center");
            return dataSet;
        }
    }
}
