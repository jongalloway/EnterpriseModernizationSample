using System;
using System.Collections.Generic;
using System.Linq;
using Fabrikam.EnterprisePizza.Data.Repositories.StoreOps;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class DispatchCoordinator : StoreOpsServiceBase, IDispatchCoordinator
    {
        private readonly IDispatchTicketRepository _dispatchTicketRepository;
        private readonly IRoutePlanningService _routePlanningService;
        private readonly IDriverAssignmentService _driverAssignmentService;
        private readonly IDispatchQueueService _dispatchQueueService;
        private readonly IDeliveryTrackingService _deliveryTrackingService;

        public DispatchCoordinator()
            : this(
                new DispatchTicketRepository(),
                new RoutePlanningService(),
                new DriverAssignmentService(),
                new DispatchQueueService(),
                new DeliveryTrackingService())
        {
        }

        public DispatchCoordinator(IDispatchTicketRepository dispatchTicketRepository)
            : this(
                dispatchTicketRepository,
                new RoutePlanningService(),
                new DriverAssignmentService(),
                new DispatchQueueService(),
                new DeliveryTrackingService())
        {
        }

        public DispatchCoordinator(
            IDispatchTicketRepository dispatchTicketRepository,
            IRoutePlanningService routePlanningService,
            IDriverAssignmentService driverAssignmentService,
            IDispatchQueueService dispatchQueueService,
            IDeliveryTrackingService deliveryTrackingService)
        {
            _dispatchTicketRepository = dispatchTicketRepository ?? throw new ArgumentNullException(nameof(dispatchTicketRepository));
            _routePlanningService = routePlanningService ?? throw new ArgumentNullException(nameof(routePlanningService));
            _driverAssignmentService = driverAssignmentService ?? throw new ArgumentNullException(nameof(driverAssignmentService));
            _dispatchQueueService = dispatchQueueService ?? throw new ArgumentNullException(nameof(dispatchQueueService));
            _deliveryTrackingService = deliveryTrackingService ?? throw new ArgumentNullException(nameof(deliveryTrackingService));
        }

        public IList<DispatchTicket> GetActiveTickets(string storeNumber)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                return _dispatchTicketRepository.GetActiveTickets(normalizedStoreNumber);
            }, "DispatchCoordinator.GetActiveTickets");
        }

        public IList<RoutePlan> GetRoutePlans(string storeNumber)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                return _routePlanningService.BuildRoutePlans(normalizedStoreNumber, GetActiveTickets(normalizedStoreNumber));
            }, "DispatchCoordinator.GetRoutePlans");
        }

        public IList<DriverAvailability> GetAvailableDrivers(string storeNumber)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                return _driverAssignmentService.GetAvailableDrivers(normalizedStoreNumber, GetActiveTickets(normalizedStoreNumber));
            }, "DispatchCoordinator.GetAvailableDrivers");
        }

        public IList<DriverAssignmentRecommendation> GetDriverAssignments(string storeNumber)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                var tickets = GetActiveTickets(normalizedStoreNumber);
                var drivers = _driverAssignmentService.GetAvailableDrivers(normalizedStoreNumber, tickets);
                return _driverAssignmentService.RecommendAssignments(normalizedStoreNumber, tickets, drivers);
            }, "DispatchCoordinator.GetDriverAssignments");
        }

        public IList<DispatchQueueEntry> GetDispatchQueue(string storeNumber)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                var tickets = GetActiveTickets(normalizedStoreNumber);
                var routePlans = _routePlanningService.BuildRoutePlans(normalizedStoreNumber, tickets);
                var assignments = _driverAssignmentService.RecommendAssignments(
                    normalizedStoreNumber,
                    tickets,
                    _driverAssignmentService.GetAvailableDrivers(normalizedStoreNumber, tickets));
                return _dispatchQueueService.BuildDispatchQueue(normalizedStoreNumber, tickets, routePlans, assignments);
            }, "DispatchCoordinator.GetDispatchQueue");
        }

        public IList<DispatchBatch> BuildDispatchBatches(string storeNumber)
        {
            return Execute(delegate
            {
                var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
                return _dispatchQueueService.BuildDispatchBatches(normalizedStoreNumber, GetDispatchQueue(normalizedStoreNumber));
            }, "DispatchCoordinator.BuildDispatchBatches");
        }

        public DeliveryTrackingRecord CreateTrackingRecord(DispatchTicket ticket, string dispatcherId)
        {
            return Execute(delegate
            {
                if (ticket == null)
                {
                    throw new ArgumentNullException(nameof(ticket));
                }

                return _deliveryTrackingService.CreateTrackingRecord(ticket, dispatcherId);
            }, "DispatchCoordinator.CreateTrackingRecord");
        }

        public DeliveryTrackingRecord UpdateTrackingStatus(DeliveryTrackingRecord trackingRecord, DeliveryStatus nextStatus, DateTime changedAtLocal, string changedBy, string note)
        {
            return Execute(delegate { return _deliveryTrackingService.UpdateStatus(trackingRecord, nextStatus, changedAtLocal, changedBy, note); }, "DispatchCoordinator.UpdateTrackingStatus");
        }

        public DeliveryTrackingRecord RecordDeliveryCompletion(DeliveryTrackingRecord trackingRecord, DateTime completedAtLocal, string completedBy, string note)
        {
            return Execute(delegate { return _deliveryTrackingService.RecordCompletion(trackingRecord, completedAtLocal, completedBy, note); }, "DispatchCoordinator.RecordDeliveryCompletion");
        }

        private static string NormalizeStoreNumber(string storeNumber)
        {
            return string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
        }
    }
}
