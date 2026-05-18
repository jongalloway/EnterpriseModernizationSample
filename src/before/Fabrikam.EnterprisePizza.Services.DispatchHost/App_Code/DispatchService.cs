using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Core.Composition;
using Fabrikam.EnterprisePizza.Core.ExceptionHandling;
using Fabrikam.EnterprisePizza.Core.Logging;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Services.DispatchHost
{
    [ServiceContract]
    public interface IDispatchService
    {
        [OperationContract]
        IList<RoutePlan> PlanRoute(StoreDispatchRequest request);

        [OperationContract]
        DriverAssignmentRecommendation AssignDriver(DispatchTicket ticket);

        [OperationContract]
        IList<DispatchQueueEntry> GetDispatchQueue(StoreDispatchRequest request);

        [OperationContract]
        DeliveryTrackingRecord UpdateDeliveryStatus(DispatchTicket ticket, DeliveryTrackingRecord trackingRecord, DeliveryStatus nextStatus, DateTime changedAtLocal, string changedBy, string note);

        [OperationContract]
        IList<DriverAvailability> GetDriverAvailability(StoreDispatchRequest request);
    }

    public class DispatchService : IDispatchService
    {
        private readonly IDispatchCoordinator dispatchCoordinator;

        public DispatchService()
            : this(LegacyServiceLocator.Resolve<IDispatchCoordinator>())
        {
        }

        public DispatchService(IDispatchCoordinator dispatchCoordinator)
        {
            this.dispatchCoordinator = dispatchCoordinator ?? throw new ArgumentNullException(nameof(dispatchCoordinator));
        }

        public IList<RoutePlan> PlanRoute(StoreDispatchRequest request)
        {
            try
            {
                var storeNumber = NormalizeStoreNumber(request);
                LogWriter.Write("PlanRoute requested for store " + storeNumber + ".", "DispatchService", TraceEventType.Information, null);
                var routePlans = dispatchCoordinator.GetRoutePlans(storeNumber);
                LogWriter.Write("PlanRoute returned " + routePlans.Count + " route plans for store " + storeNumber + ".", "DispatchService", TraceEventType.Information, null);
                return routePlans;
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        public DriverAssignmentRecommendation AssignDriver(DispatchTicket ticket)
        {
            try
            {
                if (ticket == null)
                {
                    throw new ArgumentNullException(nameof(ticket));
                }

                var storeNumber = NormalizeStoreNumber(ticket.StoreNumber);
                LogWriter.Write("AssignDriver requested for ticket " + ticket.TicketId + " in store " + storeNumber + ".", "DispatchService", TraceEventType.Information, null);
                var recommendation = dispatchCoordinator.GetDriverAssignments(storeNumber).FirstOrDefault(item => item.TicketId == ticket.TicketId);
                if (recommendation == null)
                {
                    throw new InvalidOperationException("No driver assignment recommendation was generated for delivery ticket " + ticket.TicketId + ".");
                }

                LogWriter.Write("AssignDriver recommended " + recommendation.DriverCode + " for ticket " + recommendation.TicketId + ".", "DispatchService", TraceEventType.Information, null);
                return recommendation;
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        public IList<DispatchQueueEntry> GetDispatchQueue(StoreDispatchRequest request)
        {
            try
            {
                var storeNumber = NormalizeStoreNumber(request);
                LogWriter.Write("GetDispatchQueue requested for store " + storeNumber + ".", "DispatchService", TraceEventType.Information, null);
                var queue = dispatchCoordinator.GetDispatchQueue(storeNumber);
                LogWriter.Write("GetDispatchQueue returned " + queue.Count + " queue entries for store " + storeNumber + ".", "DispatchService", TraceEventType.Information, null);
                return queue;
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        public DeliveryTrackingRecord UpdateDeliveryStatus(DispatchTicket ticket, DeliveryTrackingRecord trackingRecord, DeliveryStatus nextStatus, DateTime changedAtLocal, string changedBy, string note)
        {
            try
            {
                if (trackingRecord == null && ticket == null)
                {
                    throw new ArgumentNullException(nameof(ticket), "A dispatch ticket is required when no tracking record has been provided.");
                }

                var effectiveChangedAtLocal = changedAtLocal == default(DateTime) ? DateTime.Now : changedAtLocal;
                var effectiveTrackingRecord = trackingRecord;
                if (effectiveTrackingRecord == null)
                {
                    effectiveTrackingRecord = dispatchCoordinator.CreateTrackingRecord(ticket, changedBy);
                    if (effectiveTrackingRecord.CurrentStatus == nextStatus)
                    {
                        LogWriter.Write("UpdateDeliveryStatus initialized tracking for ticket " + effectiveTrackingRecord.TicketId + " at status " + nextStatus + ".", "DispatchService", TraceEventType.Information, null);
                        return effectiveTrackingRecord;
                    }
                }

                LogWriter.Write("UpdateDeliveryStatus requested for ticket " + effectiveTrackingRecord.TicketId + " to " + nextStatus + ".", "DispatchService", TraceEventType.Information, null);
                var updatedRecord = nextStatus == DeliveryStatus.Delivered
                    ? dispatchCoordinator.RecordDeliveryCompletion(effectiveTrackingRecord, effectiveChangedAtLocal, changedBy, note)
                    : dispatchCoordinator.UpdateTrackingStatus(effectiveTrackingRecord, nextStatus, effectiveChangedAtLocal, changedBy, note);
                LogWriter.Write("UpdateDeliveryStatus moved ticket " + updatedRecord.TicketId + " to " + updatedRecord.CurrentStatus + ".", "DispatchService", TraceEventType.Information, null);
                return updatedRecord;
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        public IList<DriverAvailability> GetDriverAvailability(StoreDispatchRequest request)
        {
            try
            {
                var storeNumber = NormalizeStoreNumber(request);
                LogWriter.Write("GetDriverAvailability requested for store " + storeNumber + ".", "DispatchService", TraceEventType.Information, null);
                var availability = dispatchCoordinator.GetAvailableDrivers(storeNumber);
                LogWriter.Write("GetDriverAvailability returned " + availability.Count + " driver rows for store " + storeNumber + ".", "DispatchService", TraceEventType.Information, null);
                return availability;
            }
            catch (Exception ex)
            {
                throw HandleServiceBoundaryException(ex);
            }
        }

        private static string NormalizeStoreNumber(StoreDispatchRequest request)
        {
            return NormalizeStoreNumber(request == null ? null : request.StoreNumber);
        }

        private static string NormalizeStoreNumber(string storeNumber)
        {
            return string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
        }

        private static Exception HandleServiceBoundaryException(Exception exception)
        {
            Exception exceptionToThrow;
            if (ExceptionPolicy.HandleException(exception, "ServiceBoundaryPolicy", out exceptionToThrow) && exceptionToThrow != null)
            {
                return exceptionToThrow;
            }

            return exception;
        }
    }
}
