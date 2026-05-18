using System;
using System.Collections.Specialized;
using Fabrikam.EnterprisePizza.Business.StoreOps.Configuration;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class DeliveryTrackingServiceFixture
    {
        [Test]
        public void RecordCompletion_sets_delivered_timestamp_and_history_entry()
        {
            var service = new DeliveryTrackingService(new DispatchBusinessRules(new NameValueCollection()));
            var ticket = CreateTicket();
            var trackingRecord = service.CreateTrackingRecord(ticket, "desk-01");
            var dispatchedAt = ticket.ReadyAtLocal.AddMinutes(2);

            service.UpdateStatus(trackingRecord, DeliveryStatus.Assigned, ticket.ReadyAtLocal, "desk-01", "Driver staged.");
            service.UpdateStatus(trackingRecord, DeliveryStatus.Dispatched, dispatchedAt, "desk-01", "Driver left the store.");
            var completedRecord = service.RecordCompletion(trackingRecord, dispatchedAt.AddMinutes(18), "drv-17", "Customer signed the receipt.");

            Assert.That(completedRecord.CurrentStatus, Is.EqualTo(DeliveryStatus.Delivered));
            Assert.That(completedRecord.DeliveredAtLocal, Is.EqualTo(dispatchedAt.AddMinutes(18)));
            Assert.That(completedRecord.History[completedRecord.History.Count - 1].Note, Is.EqualTo("Customer signed the receipt."));
        }

        [Test]
        public void UpdateStatus_rejects_invalid_transition()
        {
            var service = new DeliveryTrackingService(new DispatchBusinessRules(new NameValueCollection()));
            var trackingRecord = service.CreateTrackingRecord(CreateTicket(), "desk-01");

            var exception = Assert.Throws<InvalidOperationException>(() => service.UpdateStatus(trackingRecord, DeliveryStatus.Delivered, trackingRecord.LastUpdatedAtLocal.AddMinutes(1), "desk-01", "Skipped steps."));

            Assert.That(exception.Message, Does.Contain("not permitted"));
        }

        private static DispatchTicket CreateTicket()
        {
            var readyAt = DateTime.Today.AddHours(17).AddMinutes(12);
            return new DispatchTicket
            {
                TicketId = 701,
                StoreNumber = "014",
                DriverCode = "DRV-17",
                RouteZone = "Northwest Corporate Corridor",
                CustomerName = "Contoso",
                ReadyAtLocal = readyAt,
                PromiseTimeLocal = readyAt.AddMinutes(20),
                RouteDistanceMiles = 6.0m,
                EstimatedTravelMinutes = 18,
                Status = DeliveryStatus.ReadyForDispatch,
                PriorityScore = 88
            };
        }
    }
}
