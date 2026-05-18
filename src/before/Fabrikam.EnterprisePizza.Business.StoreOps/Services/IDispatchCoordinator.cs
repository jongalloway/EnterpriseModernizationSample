using System;
using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IDispatchCoordinator
    {
        IList<DispatchTicket> GetActiveTickets(string storeNumber);

        IList<RoutePlan> GetRoutePlans(string storeNumber);

        IList<DriverAvailability> GetAvailableDrivers(string storeNumber);

        IList<DriverAssignmentRecommendation> GetDriverAssignments(string storeNumber);

        IList<DispatchQueueEntry> GetDispatchQueue(string storeNumber);

        IList<DispatchBatch> BuildDispatchBatches(string storeNumber);

        DeliveryTrackingRecord CreateTrackingRecord(DispatchTicket ticket, string dispatcherId);

        DeliveryTrackingRecord UpdateTrackingStatus(DeliveryTrackingRecord trackingRecord, DeliveryStatus nextStatus, DateTime changedAtLocal, string changedBy, string note);

        DeliveryTrackingRecord RecordDeliveryCompletion(DeliveryTrackingRecord trackingRecord, DateTime completedAtLocal, string completedBy, string note);
    }
}
