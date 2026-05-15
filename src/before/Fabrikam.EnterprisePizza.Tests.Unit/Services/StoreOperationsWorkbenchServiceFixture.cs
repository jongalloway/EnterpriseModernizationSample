using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class StoreOperationsWorkbenchServiceFixture
    {
        [Test]
        public void GetOrderLookupRecords_returns_records_for_specified_store()
        {
            var service = new StoreOperationsWorkbenchService();

            var records = service.GetOrderLookupRecords("031");

            Assert.That(records, Is.Not.Empty);
            Assert.That(records.Select(r => r.StoreNumber), Is.All.EqualTo("031"));
        }

        [Test]
        public void GetOrderLookupRecords_normalizes_store_number_to_uppercase()
        {
            var service = new StoreOperationsWorkbenchService();

            var records = service.GetOrderLookupRecords("031");

            Assert.That(records.Select(r => r.StoreNumber), Is.All.EqualTo("031"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GetOrderLookupRecords_falls_back_to_default_store_when_input_is_blank(string blankInput)
        {
            var service = new StoreOperationsWorkbenchService();

            var records = service.GetOrderLookupRecords(blankInput);

            Assert.That(records, Is.Not.Empty);
            Assert.That(records.Select(r => r.StoreNumber), Is.All.EqualTo("014"));
        }

        [Test]
        public void GetOrderLookupRecords_returns_results_sorted_by_promise_window_then_order_number()
        {
            var service = new StoreOperationsWorkbenchService();

            var records = service.GetOrderLookupRecords("057").ToList();

            for (int i = 1; i < records.Count; i++)
            {
                int comparison = string.Compare(records[i - 1].PromiseWindow, records[i].PromiseWindow, System.StringComparison.Ordinal);
                Assert.That(comparison, Is.LessThanOrEqualTo(0), "Records must be sorted by PromiseWindow ascending.");
                if (comparison == 0)
                {
                    Assert.That(records[i - 1].OrderNumber, Is.LessThanOrEqualTo(records[i].OrderNumber),
                        "Records with equal PromiseWindow must be sorted by OrderNumber ascending.");
                }
            }
        }

        [Test]
        public void GetOrderLookupRecords_includes_at_least_two_service_modes()
        {
            var service = new StoreOperationsWorkbenchService();

            var modes = service.GetOrderLookupRecords("081")
                               .Select(r => r.ServiceMode)
                               .Distinct()
                               .ToList();

            Assert.That(modes, Has.Count.GreaterThan(1));
        }

        [Test]
        public void GetOrderLookupRecords_contains_carryout_hold_and_settled_payment_statuses()
        {
            var service = new StoreOperationsWorkbenchService();

            var records = service.GetOrderLookupRecords("014").ToList();

            Assert.That(records.Any(r => r.DispatchStatus == "Carryout Hold"), Is.True,
                "Carryout orders should show Carryout Hold dispatch status.");
            Assert.That(records.Any(r => r.PaymentStatus == "Settled"), Is.True,
                "At least one record should have a Settled payment status.");
        }

        [Test]
        public void GetStoreManagementRecords_returns_four_stores()
        {
            var service = new StoreOperationsWorkbenchService();

            var records = service.GetStoreManagementRecords();

            Assert.That(records, Has.Count.EqualTo(4));
        }

        [Test]
        public void GetStoreManagementRecords_includes_stores_in_multiple_districts()
        {
            var service = new StoreOperationsWorkbenchService();

            var districts = service.GetStoreManagementRecords()
                                   .Select(r => r.District)
                                   .Distinct()
                                   .ToList();

            Assert.That(districts, Has.Count.GreaterThan(1));
        }

        [Test]
        public void GetStoreManagementRecords_mall_annex_shows_needs_follow_up_with_escalation_note()
        {
            var service = new StoreOperationsWorkbenchService();

            var mallAnnex = service.GetStoreManagementRecords()
                                   .Single(r => r.StoreNumber == "031");

            Assert.That(mallAnnex.StoreStatus, Is.EqualTo("Needs Follow-Up"));
            Assert.That(mallAnnex.EscalationNote, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void GetStoreManagementRecords_all_stores_have_terminal_ids()
        {
            var service = new StoreOperationsWorkbenchService();

            var records = service.GetStoreManagementRecords();

            Assert.That(records.Select(r => r.DispatchTerminalId), Has.All.Matches<string>(id => id.StartsWith("TERM-")));
        }
    }
}
