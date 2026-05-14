using System;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Data.Gateways;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class WorkforceReportingFixture
    {
        [Test]
        public void GetSnapshot_returns_empty_sections_when_store_has_no_reporting_rows()
        {
            var service = new WorkforceReportingService();

            var snapshot = service.GetSnapshot("999", new DateTime(2026, 5, 14));

            Assert.That(snapshot.LaborCost, Is.Null);
            Assert.That(snapshot.Turnover, Is.Null);
            Assert.That(snapshot.Staffing, Is.Null);
            Assert.That(snapshot.OvertimeTrend, Is.Empty);
        }

        [Test]
        public void Nightly_pos_import_job_preserves_legacy_connection_name_shape()
        {
            var gateway = new LegacyDbGateway();

            Assert.That(gateway.GetConnectionName("StoreOps"), Is.EqualTo("FabrikamPizza_StoreOps"));
        }
    }
}
