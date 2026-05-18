using System.Linq;
using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Integrations.PosSync.Jobs;
using Fabrikam.EnterprisePizza.Services.DispatchHost;
using Fabrikam.EnterprisePizza.Services.PartnerSync;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.IntegrationStubs
{
    [TestFixture]
    [Category("IntegrationStub")]
    public class CrossSystemWorkflowStubFixture
    {
        [Test]
        public void Dispatch_service_stub_preserves_dispatch_ticket_contract_shape_for_store()
        {
            const string storeNumber = "014";

            var coordinatorTickets = new DispatchCoordinator().GetActiveTickets(storeNumber);
            var serviceTickets = new StoreDispatchService().GetDispatchBoard(storeNumber);

            Assert.Multiple(() =>
            {
                Assert.That(serviceTickets.Count, Is.EqualTo(coordinatorTickets.Count));
                Assert.That(serviceTickets.Select(ticket => ticket.TicketId), Is.EqualTo(coordinatorTickets.Select(ticket => ticket.TicketId)));
                Assert.That(serviceTickets.Select(ticket => ticket.StoreNumber).Distinct(), Is.EquivalentTo(new[] { storeNumber }));
                Assert.That(serviceTickets.All(ticket => !string.IsNullOrWhiteSpace(ticket.RouteZone)), Is.True);
            });
        }

        [Test]
        public void Partner_sync_stub_exposes_customerhub_partner_list()
        {
            var accountService = new PartnerAccountService();
            var syncService = new PartnerSyncService();

            Assert.That(syncService.GetPreferredPartners(), Is.EqualTo(accountService.GetPreferredPartners()));
        }

        [Test]
        public void Pos_import_stub_keeps_storeops_database_alias_stable()
        {
            var gateway = new LegacyDbGateway();
            var job = new NightlyPosImportJob();

            Assert.That(job.GetTargetDatabase(), Is.EqualTo(gateway.GetConnectionName("StoreOps")));
            Assert.That(job.GetLatestImportedBatch("014"), Is.Not.Null);
        }

        [Test]
        [Explicit("Scenario stub only until the desktop shell and reporting batch consume a shared dispatch snapshot seam.")]
        public void Dispatch_board_snapshot_should_flow_from_wcf_to_winforms_and_batch_exports()
        {
            Assert.That(new StoreDispatchService().GetDispatchBoard("014"), Is.Not.Empty);

            ScenarioStub.Pending(
                "Verify the WCF dispatch board matches the WinForms dispatch shell and nightly reporting export.",
                @"DispatchBoardForm and Reporting.Batch still instantiate DispatchCoordinator directly instead of sharing a harnessable snapshot seam.");
        }

        [Test]
        [Explicit("Scenario stub only until Web CustomerHub stops binding its own partner table.")]
        public void Corporate_partner_directory_should_match_partner_sync_service_payload()
        {
            Assert.That(new PartnerSyncService().GetPreferredPartners(), Does.Contain("Contoso Office Parks"));

            ScenarioStub.Pending(
                "Verify the CustomerHub corporate accounts page preserves the ASMX partner list without dropping rows.",
                @"Web.CustomerHub\CorporateAccounts.aspx.cs still binds a hard-coded DataTable instead of consuming PartnerSyncService.");
        }
    }
}
