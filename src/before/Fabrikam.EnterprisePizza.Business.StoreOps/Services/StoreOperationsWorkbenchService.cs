using System;
using System.Collections.Generic;
using System.Linq;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class StoreOperationsWorkbenchService
    {
        private readonly DispatchCoordinator _dispatchCoordinator;

        public StoreOperationsWorkbenchService()
            : this(new DispatchCoordinator())
        {
        }

        public StoreOperationsWorkbenchService(DispatchCoordinator dispatchCoordinator)
        {
            _dispatchCoordinator = dispatchCoordinator ?? throw new ArgumentNullException(nameof(dispatchCoordinator));
        }

        public IList<OrderLookupRecord> GetOrderLookupRecords(string storeNumber)
        {
            var normalizedStoreNumber = string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
            var tickets = _dispatchCoordinator.GetActiveTickets(normalizedStoreNumber).ToList();
            var orders = new List<OrderLookupRecord>();

            foreach (var ticket in tickets)
            {
                var holdForPair = ticket.TicketId % 2 == 0;
                orders.Add(new OrderLookupRecord
                {
                    OrderNumber = 70000 + ticket.TicketId,
                    StoreNumber = normalizedStoreNumber,
                    CustomerName = holdForPair ? "Corporate Lunch Desk" : "North Corridor Office",
                    Channel = holdForPair ? "Call Center" : "Web",
                    ServiceMode = "Delivery",
                    PromiseWindow = "5:" + (14 + (ticket.TicketId % 4) * 4).ToString("00"),
                    TicketTotal = 27.50m + (ticket.TicketId % 3) * 6.25m,
                    KitchenStatus = holdForPair ? "Ready" : "Make Line",
                    DispatchStatus = holdForPair ? "Ready" : "Staged",
                    PaymentStatus = holdForPair ? "Settled" : "Card Hold"
                });
            }

            orders.Add(new OrderLookupRecord
            {
                OrderNumber = 71510,
                StoreNumber = normalizedStoreNumber,
                CustomerName = "Lobby Pickup - Harris",
                Channel = "Phone",
                ServiceMode = "Carryout",
                PromiseWindow = "5:26",
                TicketTotal = 18.75m,
                KitchenStatus = "Ready",
                DispatchStatus = "Carryout Hold",
                PaymentStatus = "Settled"
            });
            orders.Add(new OrderLookupRecord
            {
                OrderNumber = 71518,
                StoreNumber = normalizedStoreNumber,
                CustomerName = "School Night Bundle",
                Channel = "POS",
                ServiceMode = "Carryout",
                PromiseWindow = "5:34",
                TicketTotal = 32.00m,
                KitchenStatus = "Exception",
                DispatchStatus = "Carryout Hold",
                PaymentStatus = "Cash Pending"
            });

            return orders.OrderBy(order => order.PromiseWindow).ThenBy(order => order.OrderNumber).ToList();
        }

        public IList<StoreManagementRecord> GetStoreManagementRecords()
        {
            return new List<StoreManagementRecord>
            {
                new StoreManagementRecord
                {
                    StoreNumber = "014",
                    StoreName = "Downtown Dispatch",
                    District = "North Metro",
                    DispatchTerminalId = "TERM-02",
                    ManagerOnDuty = "M. Delgado",
                    BoardMode = "Balanced",
                    OpenOrderCount = 8,
                    DriverCount = 4,
                    LastSyncTime = "5:18 PM",
                    StoreStatus = "Normal",
                    EscalationNote = "No escalations waiting."
                },
                new StoreManagementRecord
                {
                    StoreNumber = "031",
                    StoreName = "Mall Annex",
                    District = "North Metro",
                    DispatchTerminalId = "TERM-05",
                    ManagerOnDuty = "J. Patel",
                    BoardMode = "Rush Recovery",
                    OpenOrderCount = 11,
                    DriverCount = 3,
                    LastSyncTime = "5:16 PM",
                    StoreStatus = "Needs Follow-Up",
                    EscalationNote = "Driver board printer is running behind on reprint slips."
                },
                new StoreManagementRecord
                {
                    StoreNumber = "057",
                    StoreName = "Airport Service Road",
                    District = "South Metro",
                    DispatchTerminalId = "TERM-03",
                    ManagerOnDuty = "T. Morris",
                    BoardMode = "Delivery Heavy",
                    OpenOrderCount = 10,
                    DriverCount = 5,
                    LastSyncTime = "5:17 PM",
                    StoreStatus = "Normal",
                    EscalationNote = "No escalations waiting."
                },
                new StoreManagementRecord
                {
                    StoreNumber = "081",
                    StoreName = "College Commons",
                    District = "Campus",
                    DispatchTerminalId = "TERM-07",
                    ManagerOnDuty = "R. Nguyen",
                    BoardMode = "Counter Hold",
                    OpenOrderCount = 6,
                    DriverCount = 2,
                    LastSyncTime = "5:13 PM",
                    StoreStatus = "Needs Follow-Up",
                    EscalationNote = "Clock drift on the carryout counter is stretching promise windows."
                }
            };
        }
    }
}
