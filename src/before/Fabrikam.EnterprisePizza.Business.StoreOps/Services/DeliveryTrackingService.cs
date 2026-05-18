using System;
using System.Collections.Generic;
using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Configuration;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class DeliveryTrackingService : StoreOpsServiceBase, IDeliveryTrackingService
    {
        private static readonly IDictionary<DeliveryStatus, DeliveryStatus[]> AllowedTransitions = new Dictionary<DeliveryStatus, DeliveryStatus[]>
        {
            { DeliveryStatus.Routed, new[] { DeliveryStatus.ReadyForDispatch, DeliveryStatus.Assigned } },
            { DeliveryStatus.ReadyForDispatch, new[] { DeliveryStatus.Assigned, DeliveryStatus.Dispatched, DeliveryStatus.DeliveryException } },
            { DeliveryStatus.Assigned, new[] { DeliveryStatus.Dispatched, DeliveryStatus.DeliveryException } },
            { DeliveryStatus.Dispatched, new[] { DeliveryStatus.EnRoute, DeliveryStatus.DeliveryException } },
            { DeliveryStatus.EnRoute, new[] { DeliveryStatus.Delivered, DeliveryStatus.DeliveryException } },
            { DeliveryStatus.Delivered, new[] { DeliveryStatus.Closed } },
            { DeliveryStatus.DeliveryException, new[] { DeliveryStatus.Closed } },
            { DeliveryStatus.Closed, new DeliveryStatus[0] }
        };

        private readonly DispatchBusinessRules _rules;

        public DeliveryTrackingService()
            : this(new DispatchBusinessRules())
        {
        }

        public DeliveryTrackingService(DispatchBusinessRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public DeliveryTrackingRecord CreateTrackingRecord(DispatchTicket ticket, string dispatcherId)
        {
            return Execute(delegate
            {
                if (ticket == null)
                {
                    throw new ArgumentNullException(nameof(ticket));
                }

                var dispatchedAtLocal = ticket.ReadyAtLocal == default(DateTime)
                    ? DateTime.Today.AddHours(17).AddMinutes(15)
                    : ticket.ReadyAtLocal;
                var travelMinutes = ticket.EstimatedTravelMinutes > 0
                    ? ticket.EstimatedTravelMinutes
                    : EstimateTravelMinutes(ticket.RouteDistanceMiles);
                var startingStatus = ticket.Status == 0 ? DeliveryStatus.ReadyForDispatch : ticket.Status;

                var trackingRecord = new DeliveryTrackingRecord
                {
                    TicketId = ticket.TicketId,
                    StoreNumber = ticket.StoreNumber,
                    DriverCode = ticket.DriverCode,
                    CurrentStatus = startingStatus,
                    EstimatedArrivalLocal = dispatchedAtLocal.AddMinutes(travelMinutes),
                    DispatchedAtLocal = startingStatus >= DeliveryStatus.Dispatched ? dispatchedAtLocal : (DateTime?)null,
                    LastUpdatedAtLocal = dispatchedAtLocal
                };

                trackingRecord.History.Add(new DeliveryStatusEvent
                {
                    Status = DeliveryStatus.Routed,
                    ChangedAtLocal = dispatchedAtLocal.AddMinutes(-_rules.StopServiceMinutes),
                    ChangedBy = string.IsNullOrWhiteSpace(dispatcherId) ? "dispatch-board" : dispatcherId,
                    Note = "Ticket entered the route desk."
                });
                trackingRecord.History.Add(new DeliveryStatusEvent
                {
                    Status = startingStatus,
                    ChangedAtLocal = dispatchedAtLocal,
                    ChangedBy = string.IsNullOrWhiteSpace(dispatcherId) ? "dispatch-board" : dispatcherId,
                    Note = "Tracking record initialized for live delivery monitoring."
                });
                return trackingRecord;
            }, "DeliveryTrackingService.CreateTrackingRecord");
        }

        public DeliveryTrackingRecord UpdateStatus(DeliveryTrackingRecord trackingRecord, DeliveryStatus nextStatus, DateTime changedAtLocal, string changedBy, string note)
        {
            return Execute(delegate
            {
                if (trackingRecord == null)
                {
                    throw new ArgumentNullException(nameof(trackingRecord));
                }

                if (!AllowedTransitions[trackingRecord.CurrentStatus].Contains(nextStatus))
                {
                    throw new InvalidOperationException(string.Format(
                        "The transition from {0} to {1} is not permitted for delivery ticket {2}.",
                        trackingRecord.CurrentStatus,
                        nextStatus,
                        trackingRecord.TicketId));
                }

                trackingRecord.CurrentStatus = nextStatus;
                trackingRecord.LastUpdatedAtLocal = changedAtLocal;
                if (nextStatus == DeliveryStatus.Dispatched && !trackingRecord.DispatchedAtLocal.HasValue)
                {
                    trackingRecord.DispatchedAtLocal = changedAtLocal;
                }

                if (nextStatus == DeliveryStatus.Delivered)
                {
                    trackingRecord.DeliveredAtLocal = changedAtLocal;
                    trackingRecord.CompletionNote = note;
                }

                trackingRecord.History.Add(new DeliveryStatusEvent
                {
                    Status = nextStatus,
                    ChangedAtLocal = changedAtLocal,
                    ChangedBy = string.IsNullOrWhiteSpace(changedBy) ? "dispatch-board" : changedBy,
                    Note = note ?? string.Empty
                });
                return trackingRecord;
            }, "DeliveryTrackingService.UpdateStatus");
        }

        public DeliveryTrackingRecord RecordCompletion(DeliveryTrackingRecord trackingRecord, DateTime completedAtLocal, string completedBy, string note)
        {
            return Execute(delegate
            {
                if (trackingRecord == null)
                {
                    throw new ArgumentNullException(nameof(trackingRecord));
                }

                if (trackingRecord.CurrentStatus != DeliveryStatus.EnRoute && trackingRecord.CurrentStatus != DeliveryStatus.Dispatched)
                {
                    throw new InvalidOperationException("Completion can only be recorded after the order has left the store.");
                }

                if (trackingRecord.CurrentStatus == DeliveryStatus.Dispatched)
                {
                    UpdateStatus(trackingRecord, DeliveryStatus.EnRoute, completedAtLocal.AddMinutes(-Math.Max(1, _rules.StopServiceMinutes)), completedBy, "Driver confirmed route departure.");
                }

                return UpdateStatus(trackingRecord, DeliveryStatus.Delivered, completedAtLocal, completedBy, note);
            }, "DeliveryTrackingService.RecordCompletion");
        }

        private int EstimateTravelMinutes(decimal routeDistanceMiles)
        {
            if (routeDistanceMiles <= 0m)
            {
                routeDistanceMiles = 5.0m;
            }

            var minutes = routeDistanceMiles / (_rules.AverageSpeedMph <= 0m ? 24.0m : _rules.AverageSpeedMph) * 60m;
            return (int)Math.Ceiling(minutes) + _rules.StopServiceMinutes;
        }
    }
}
