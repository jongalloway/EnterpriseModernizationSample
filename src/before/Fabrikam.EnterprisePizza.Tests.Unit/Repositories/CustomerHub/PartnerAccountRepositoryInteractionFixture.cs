using System;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Fabrikam.EnterprisePizza.Tests.Unit.Testing;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Repositories.CustomerHub
{
    [TestFixture]
    public class PartnerAccountRepositoryInteractionFixture
    {
        private RecordingLegacyDbGateway gateway;
        private PartnerAccountRepository repository;

        [SetUp]
        public void SetUp()
        {
            gateway = new RecordingLegacyDbGateway();
            repository = new PartnerAccountRepository(gateway);
        }

        [TearDown]
        public void TearDown()
        {
            gateway = null;
            repository = null;
        }

        [Test]
        public void GetPreferredPartners_calls_customerhub_proc_and_maps_partner_names()
        {
            gateway.DataSetToReturn = CreatePreferredPartnersDataSet();

            var partners = repository.GetPreferredPartners();

            Assert.That(gateway.LastArea, Is.EqualTo(LegacyDatabaseArea.CustomerHub));
            Assert.That(gateway.LastProcedureName, Is.EqualTo(LegacyStoredProcedures.CustomerHub.GetPreferredPartners));
            Assert.That(gateway.LastExecutedCall.ConnectionName, Is.EqualTo("FabrikamPizza_CustomerHub"));
            Assert.That(partners, Is.EqualTo(new[] { "Wingtip Catering", "Alpine Ski Summit" }));
        }

        [Test]
        public void GetPreferredPartners_bubbles_gateway_failures_for_repro_clarity()
        {
            gateway.ExceptionToThrow = new InvalidOperationException("Customer hub timeout");

            var ex = Assert.Throws<InvalidOperationException>(() => repository.GetPreferredPartners());

            Assert.That(ex.Message, Does.Contain("timeout"));
        }

        [Test]
        public void Constructor_rejects_null_gateway()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new PartnerAccountRepository(null));

            Assert.That(ex.ParamName, Is.EqualTo("dbGateway"));
        }

        private static DataSet CreatePreferredPartnersDataSet()
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add("PreferredPartners");
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("AccountCode", typeof(string));
            table.Rows.Add("Wingtip Catering", "CAT-100");
            table.Rows.Add("Alpine Ski Summit", "EVT-305");
            return dataSet;
        }
    }
}
