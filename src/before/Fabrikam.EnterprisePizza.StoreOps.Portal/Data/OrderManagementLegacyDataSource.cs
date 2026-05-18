using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.StoreOps.Portal.Data
{
    [DataObject]
    public class OrderManagementLegacyDataSource
    {
        private readonly StoreOperationsWorkbenchService _workbenchService;

        public OrderManagementLegacyDataSource()
            : this(new StoreOperationsWorkbenchService())
        {
        }

        internal OrderManagementLegacyDataSource(StoreOperationsWorkbenchService workbenchService)
        {
            _workbenchService = workbenchService ?? throw new ArgumentNullException(nameof(workbenchService));
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<OrderLookupRow> GetLookupOrders(string storeNumber, string searchText, string statusFilter, string serviceModeFilter, string sortExpression)
        {
            var orders = LoadLookupRows(storeNumber)
                .Where(order => MatchesSearch(order, searchText))
                .Where(order => MatchesStatus(order, statusFilter))
                .Where(order => MatchesServiceMode(order, serviceModeFilter));

            return ApplyLookupSort(orders, sortExpression).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<OrderDetailRecord> GetOrderDetails(int orderNumber, string storeNumber)
        {
            var order = LoadLookupRows(storeNumber).FirstOrDefault(row => row.OrderNumber == orderNumber);
            if (order == null)
            {
                return new List<OrderDetailRecord>();
            }

            var businessDate = BuildBusinessDate(order.OrderNumber);
            return new List<OrderDetailRecord>
            {
                new OrderDetailRecord
                {
                    OrderNumber = order.OrderNumber,
                    StoreNumber = order.StoreNumber,
                    CustomerName = order.CustomerName,
                    Channel = order.Channel,
                    ServiceMode = order.ServiceMode,
                    PromiseWindow = order.PromiseWindow,
                    TicketTotal = order.TicketTotal,
                    KitchenStatus = order.KitchenStatus,
                    DispatchStatus = order.DispatchStatus,
                    PaymentStatus = order.PaymentStatus,
                    ClerkStation = order.Channel == "Phone" ? "Counter 2" : "Web Print Desk",
                    BusinessDate = businessDate,
                    DriverOrCounter = order.ServiceMode == "Delivery" ? "Driver board release" : "Carryout shelf callback",
                    LastTouchDisplay = order.LastTouchDisplay,
                    IssueFlag = order.IssueFlag,
                    FollowUpNote = BuildFollowUpNote(order)
                }
            };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<OrderStatusTimelineRow> GetOrderStatusTimeline(int orderNumber, string storeNumber)
        {
            var order = LoadLookupRows(storeNumber).FirstOrDefault(row => row.OrderNumber == orderNumber);
            if (order == null)
            {
                return new List<OrderStatusTimelineRow>();
            }

            var businessDate = BuildBusinessDate(order.OrderNumber);
            var promiseTime = ParsePromiseWindow(order.PromiseWindow);
            var promiseDate = businessDate.Date.Add(promiseTime);

            return new List<OrderStatusTimelineRow>
            {
                new OrderStatusTimelineRow
                {
                    EventTime = promiseDate.AddMinutes(-32).ToString("MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture),
                    Step = "Order Entry",
                    Station = order.Channel == "Phone" ? "Counter" : "Web Intake",
                    Status = order.PaymentStatus,
                    Note = order.Channel + " order confirmed for " + order.CustomerName + "."
                },
                new OrderStatusTimelineRow
                {
                    EventTime = promiseDate.AddMinutes(-21).ToString("MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture),
                    Step = "Kitchen Queue",
                    Station = "Makeline",
                    Status = order.KitchenStatus,
                    Note = order.ServiceMode == "Delivery"
                        ? "Bag with hot-hold tag before dispatch release."
                        : "Stage near carryout shelf and hold the callback slip."
                },
                new OrderStatusTimelineRow
                {
                    EventTime = promiseDate.AddMinutes(-9).ToString("MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture),
                    Step = "Supervisor Review",
                    Station = "Expediter",
                    Status = order.IssueFlag,
                    Note = BuildFollowUpNote(order)
                },
                new OrderStatusTimelineRow
                {
                    EventTime = promiseDate.ToString("MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture),
                    Step = "Release",
                    Station = order.ServiceMode == "Delivery" ? "Dispatch" : "Front Counter",
                    Status = order.DispatchStatus,
                    Note = order.DispatchStatus == "Carryout Hold"
                        ? "Guest callback required before bumping promise window."
                        : "Release on promise window " + order.PromiseWindow + "."
                }
            };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<OrderHistoryRow> GetOrderHistory(string storeNumber, string startDateText, string endDateText, string serviceModeFilter, string sortExpression)
        {
            var parsedDates = ResolveDateRange(startDateText, endDateText);
            var historyRows = new List<OrderHistoryRow>();
            var lookupRows = LoadLookupRows(storeNumber);

            foreach (var order in lookupRows)
            {
                for (var dayOffset = 0; dayOffset < 3; dayOffset++)
                {
                    var businessDate = BuildBusinessDate(order.OrderNumber).AddDays(-dayOffset * 4);
                    if (businessDate.Date < parsedDates.StartDate || businessDate.Date > parsedDates.EndDate)
                    {
                        continue;
                    }

                    historyRows.Add(new OrderHistoryRow
                    {
                        OrderNumber = order.OrderNumber - (dayOffset * 37),
                        StoreNumber = order.StoreNumber,
                        BusinessDate = businessDate,
                        BusinessDateDisplay = businessDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                        CustomerName = order.CustomerName,
                        Channel = order.Channel,
                        ServiceMode = order.ServiceMode,
                        CloseStatus = ResolveCloseStatus(order, dayOffset),
                        SettlementStatus = ResolveSettlementStatus(order, dayOffset),
                        TicketTotal = order.TicketTotal + (dayOffset * 1.50m),
                        CloseNote = ResolveCloseNote(order, dayOffset)
                    });
                }
            }

            var filteredRows = historyRows
                .Where(row => MatchesServiceMode(row.ServiceMode, serviceModeFilter));

            return ApplyHistorySort(filteredRows, sortExpression).ToList();
        }

        private IList<OrderLookupRow> LoadLookupRows(string storeNumber)
        {
            return _workbenchService
                .GetOrderLookupRecords(storeNumber)
                .Select((record, index) => MapLookupRow(record, index))
                .ToList();
        }

        private static OrderLookupRow MapLookupRow(OrderLookupRecord record, int index)
        {
            var lastTouchTime = DateTime.Today.Add(ParsePromiseWindow(record.PromiseWindow)).AddMinutes(-12 - (index * 3));
            return new OrderLookupRow
            {
                OrderNumber = record.OrderNumber,
                StoreNumber = record.StoreNumber,
                CustomerName = record.CustomerName,
                Channel = record.Channel,
                ServiceMode = record.ServiceMode,
                PromiseWindow = record.PromiseWindow,
                TicketTotal = record.TicketTotal,
                KitchenStatus = record.KitchenStatus,
                DispatchStatus = record.DispatchStatus,
                PaymentStatus = record.PaymentStatus,
                LastTouchDisplay = lastTouchTime.ToString("h:mm tt", CultureInfo.InvariantCulture),
                IssueFlag = ResolveIssueFlag(record)
            };
        }

        private static IEnumerable<OrderLookupRow> ApplyLookupSort(IEnumerable<OrderLookupRow> rows, string sortExpression)
        {
            var normalizedSort = string.IsNullOrWhiteSpace(sortExpression) ? "PromiseWindow" : sortExpression.Trim();
            var descending = normalizedSort.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase);
            var propertyName = descending ? normalizedSort.Substring(0, normalizedSort.Length - 5) : normalizedSort;

            switch (propertyName)
            {
                case "OrderNumber":
                    return descending ? rows.OrderByDescending(row => row.OrderNumber) : rows.OrderBy(row => row.OrderNumber);
                case "CustomerName":
                    return descending ? rows.OrderByDescending(row => row.CustomerName) : rows.OrderBy(row => row.CustomerName);
                case "ServiceMode":
                    return descending ? rows.OrderByDescending(row => row.ServiceMode) : rows.OrderBy(row => row.ServiceMode);
                case "TicketTotal":
                    return descending ? rows.OrderByDescending(row => row.TicketTotal) : rows.OrderBy(row => row.TicketTotal);
                case "DispatchStatus":
                    return descending ? rows.OrderByDescending(row => row.DispatchStatus) : rows.OrderBy(row => row.DispatchStatus);
                case "PaymentStatus":
                    return descending ? rows.OrderByDescending(row => row.PaymentStatus) : rows.OrderBy(row => row.PaymentStatus);
                default:
                    return descending
                        ? rows.OrderByDescending(row => row.PromiseWindow).ThenByDescending(row => row.OrderNumber)
                        : rows.OrderBy(row => row.PromiseWindow).ThenBy(row => row.OrderNumber);
            }
        }

        private static IEnumerable<OrderHistoryRow> ApplyHistorySort(IEnumerable<OrderHistoryRow> rows, string sortExpression)
        {
            var normalizedSort = string.IsNullOrWhiteSpace(sortExpression) ? "BusinessDate DESC" : sortExpression.Trim();
            var descending = normalizedSort.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase);
            var propertyName = descending ? normalizedSort.Substring(0, normalizedSort.Length - 5) : normalizedSort;

            switch (propertyName)
            {
                case "OrderNumber":
                    return descending ? rows.OrderByDescending(row => row.OrderNumber) : rows.OrderBy(row => row.OrderNumber);
                case "CustomerName":
                    return descending ? rows.OrderByDescending(row => row.CustomerName) : rows.OrderBy(row => row.CustomerName);
                case "ServiceMode":
                    return descending ? rows.OrderByDescending(row => row.ServiceMode) : rows.OrderBy(row => row.ServiceMode);
                case "CloseStatus":
                    return descending ? rows.OrderByDescending(row => row.CloseStatus) : rows.OrderBy(row => row.CloseStatus);
                case "TicketTotal":
                    return descending ? rows.OrderByDescending(row => row.TicketTotal) : rows.OrderBy(row => row.TicketTotal);
                default:
                    return descending
                        ? rows.OrderByDescending(row => row.BusinessDate).ThenByDescending(row => row.OrderNumber)
                        : rows.OrderBy(row => row.BusinessDate).ThenBy(row => row.OrderNumber);
            }
        }

        private static bool MatchesSearch(OrderLookupRow row, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return true;
            }

            var search = searchText.Trim();
            return row.OrderNumber.ToString(CultureInfo.InvariantCulture).Contains(search)
                || row.CustomerName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                || row.Channel.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                || row.ServiceMode.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool MatchesStatus(OrderLookupRow row, string statusFilter)
        {
            if (string.IsNullOrWhiteSpace(statusFilter) || statusFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return row.KitchenStatus.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)
                || row.DispatchStatus.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)
                || row.PaymentStatus.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)
                || row.IssueFlag.Equals(statusFilter, StringComparison.OrdinalIgnoreCase);
        }

        private static bool MatchesServiceMode(OrderLookupRow row, string serviceModeFilter)
        {
            return MatchesServiceMode(row.ServiceMode, serviceModeFilter);
        }

        private static bool MatchesServiceMode(string serviceMode, string serviceModeFilter)
        {
            return string.IsNullOrWhiteSpace(serviceModeFilter)
                || serviceModeFilter.Equals("All", StringComparison.OrdinalIgnoreCase)
                || serviceMode.Equals(serviceModeFilter, StringComparison.OrdinalIgnoreCase);
        }

        private static string ResolveIssueFlag(OrderLookupRecord record)
        {
            if (record.DispatchStatus == "Carryout Hold")
            {
                return "Needs callback";
            }

            if (record.KitchenStatus == "Exception")
            {
                return "Kitchen remake";
            }

            if (record.PaymentStatus == "Card Hold" || record.PaymentStatus == "Cash Pending")
            {
                return "Payment follow-up";
            }

            return "Normal";
        }

        private static string BuildFollowUpNote(OrderLookupRow row)
        {
            if (row.DispatchStatus == "Carryout Hold")
            {
                return "Counter clerk should confirm pickup before clearing the hold slip.";
            }

            if (row.KitchenStatus == "Exception")
            {
                return "Supervisor sign-off is still attached to the remake ticket.";
            }

            if (row.PaymentStatus == "Card Hold" || row.PaymentStatus == "Cash Pending")
            {
                return "Payment verification should stay with the order jacket until release.";
            }

            return "No exception card is attached to this order jacket.";
        }

        private static DateTime BuildBusinessDate(int orderNumber)
        {
            return DateTime.Today.AddDays(-((orderNumber % 7) + 1));
        }

        private static TimeSpan ParsePromiseWindow(string promiseWindow)
        {
            DateTime parsedTime;
            if (DateTime.TryParseExact(promiseWindow, new[] { "H:mm", "h:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedTime))
            {
                return parsedTime.TimeOfDay;
            }

            return new TimeSpan(17, 30, 0);
        }

        private static DateRange ResolveDateRange(string startDateText, string endDateText)
        {
            DateTime parsedStartDate;
            DateTime parsedEndDate;

            if (!DateTime.TryParse(startDateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartDate))
            {
                parsedStartDate = DateTime.Today.AddDays(-14);
            }

            if (!DateTime.TryParse(endDateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEndDate))
            {
                parsedEndDate = DateTime.Today;
            }

            if (parsedEndDate < parsedStartDate)
            {
                var swap = parsedStartDate;
                parsedStartDate = parsedEndDate;
                parsedEndDate = swap;
            }

            return new DateRange(parsedStartDate.Date, parsedEndDate.Date);
        }

        private static string ResolveCloseStatus(OrderLookupRow row, int dayOffset)
        {
            if (row.DispatchStatus == "Carryout Hold" && dayOffset == 0)
            {
                return "Callback logged";
            }

            if (row.KitchenStatus == "Exception")
            {
                return dayOffset == 0 ? "Exception reviewed" : "Closed with remake";
            }

            return row.ServiceMode == "Delivery" ? "Delivered" : "Picked up";
        }

        private static string ResolveSettlementStatus(OrderLookupRow row, int dayOffset)
        {
            if (row.PaymentStatus == "Cash Pending" && dayOffset == 0)
            {
                return "Drawer verification";
            }

            if (row.PaymentStatus == "Card Hold" && dayOffset == 0)
            {
                return "Manual settlement";
            }

            return "Closed";
        }

        private static string ResolveCloseNote(OrderLookupRow row, int dayOffset)
        {
            if (row.DispatchStatus == "Carryout Hold" && dayOffset == 0)
            {
                return "Guest callback entered by counter supervisor before shelf release.";
            }

            if (row.KitchenStatus == "Exception")
            {
                return dayOffset == 0
                    ? "Makeline exception card stapled to the close packet."
                    : "Remake cleared after supervisor review on the night sheet.";
            }

            return row.ServiceMode == "Delivery"
                ? "Driver close slip posted after route settlement."
                : "Carryout receipt filed with the nightly cash-out packet.";
        }

        private sealed class DateRange
        {
            public DateRange(DateTime startDate, DateTime endDate)
            {
                StartDate = startDate;
                EndDate = endDate;
            }

            public DateTime StartDate { get; private set; }

            public DateTime EndDate { get; private set; }
        }
    }

    public class OrderLookupRow
    {
        public int OrderNumber { get; set; }

        public string StoreNumber { get; set; }

        public string CustomerName { get; set; }

        public string Channel { get; set; }

        public string ServiceMode { get; set; }

        public string PromiseWindow { get; set; }

        public decimal TicketTotal { get; set; }

        public string KitchenStatus { get; set; }

        public string DispatchStatus { get; set; }

        public string PaymentStatus { get; set; }

        public string LastTouchDisplay { get; set; }

        public string IssueFlag { get; set; }
    }

    public class OrderDetailRecord
    {
        public int OrderNumber { get; set; }

        public string StoreNumber { get; set; }

        public string CustomerName { get; set; }

        public string Channel { get; set; }

        public string ServiceMode { get; set; }

        public string PromiseWindow { get; set; }

        public decimal TicketTotal { get; set; }

        public string KitchenStatus { get; set; }

        public string DispatchStatus { get; set; }

        public string PaymentStatus { get; set; }

        public string ClerkStation { get; set; }

        public DateTime BusinessDate { get; set; }

        public string DriverOrCounter { get; set; }

        public string LastTouchDisplay { get; set; }

        public string IssueFlag { get; set; }

        public string FollowUpNote { get; set; }
    }

    public class OrderStatusTimelineRow
    {
        public string EventTime { get; set; }

        public string Step { get; set; }

        public string Station { get; set; }

        public string Status { get; set; }

        public string Note { get; set; }
    }

    public class OrderHistoryRow
    {
        public int OrderNumber { get; set; }

        public string StoreNumber { get; set; }

        public DateTime BusinessDate { get; set; }

        public string BusinessDateDisplay { get; set; }

        public string CustomerName { get; set; }

        public string Channel { get; set; }

        public string ServiceMode { get; set; }

        public string CloseStatus { get; set; }

        public string SettlementStatus { get; set; }

        public decimal TicketTotal { get; set; }

        public string CloseNote { get; set; }
    }
}
