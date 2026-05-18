using System;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IDeliveryTrackingService
    {
        DeliveryTrackingRecord CreateTrackingRecord(DispatchTicket ticket, string dispatcherId);

        DeliveryTrackingRecord UpdateStatus(DeliveryTrackingRecord trackingRecord, DeliveryStatus nextStatus, DateTime changedAtLocal, string changedBy, string note);

        DeliveryTrackingRecord RecordCompletion(DeliveryTrackingRecord trackingRecord, DateTime completedAtLocal, string completedBy, string note);
    }
}
