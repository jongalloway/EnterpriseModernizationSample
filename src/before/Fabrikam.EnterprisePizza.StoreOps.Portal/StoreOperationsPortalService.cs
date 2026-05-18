using System;
using System.Collections.Generic;
using System.Linq;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.StoreOps.Portal.Services
{
    public class StoreOperationsPortalService
    {
        private static readonly DateTime DefaultWeekStart = new DateTime(2026, 5, 18);
        private readonly DispatchCoordinator dispatchCoordinator;
        private readonly IList<StoreProfile> storeProfiles;

        public StoreOperationsPortalService()
            : this(new DispatchCoordinator())
        {
        }

        public StoreOperationsPortalService(DispatchCoordinator dispatchCoordinator)
        {
            this.dispatchCoordinator = dispatchCoordinator ?? throw new ArgumentNullException(nameof(dispatchCoordinator));
            storeProfiles = BuildStoreProfiles();
        }

        public IList<StoreOption> GetStores()
        {
            return storeProfiles
                .Select(profile => new StoreOption
                {
                    StoreNumber = profile.StoreNumber,
                    DisplayName = profile.StoreNumber + " - " + profile.StoreName
                })
                .ToList();
        }

        public IList<DispatchDeliveryRecord> GetDispatchBoard(string storeNumber)
        {
            var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
            var tickets = dispatchCoordinator.GetActiveTickets(normalizedStoreNumber).ToList();

            if (tickets.Count == 0)
            {
                tickets.Add(new DispatchTicket
                {
                    TicketId = 4110,
                    StoreNumber = normalizedStoreNumber,
                    DriverCode = "DRV-11",
                    RouteZone = "Westlake Apartments"
                });
                tickets.Add(new DispatchTicket
                {
                    TicketId = 4111,
                    StoreNumber = normalizedStoreNumber,
                    DriverCode = "DRV-08",
                    RouteZone = "University Row"
                });
            }

            return tickets.Select((ticket, index) => new DispatchDeliveryRecord
            {
                TicketId = ticket.TicketId,
                CustomerName = index == 0 ? "Corporate Lunch Desk" : index == 1 ? "North Corridor Office" : "Neighborhood Family Bundle",
                DriverCode = ticket.DriverCode,
                RouteZone = ticket.RouteZone,
                DispatchState = index == 0 ? "Out for delivery" : index == 1 ? "Waiting at heat rack" : "Queued for reroute",
                DriverStatus = index == 0 ? "On road" : index == 1 ? "Staging bags" : "Back at store",
                MinutesOpen = 12 + (index * 5),
                PromiseWindow = string.Format("5:{0:00} PM", 18 + (index * 7)),
                ReadyAt = string.Format("5:{0:00} PM", 8 + (index * 6))
            }).ToList();
        }

        public DispatchDriverStatusRecord GetDriverStatus(string storeNumber, string driverCode)
        {
            var selectedDriver = GetDispatchBoard(storeNumber)
                .FirstOrDefault(record => string.Equals(record.DriverCode, driverCode, StringComparison.OrdinalIgnoreCase))
                ?? GetDispatchBoard(storeNumber).First();

            return new DispatchDriverStatusRecord
            {
                DriverCode = selectedDriver.DriverCode,
                DriverName = GetDriverName(selectedDriver.DriverCode),
                Status = selectedDriver.DriverStatus,
                CurrentZone = selectedDriver.RouteZone,
                ActiveRuns = string.Equals(selectedDriver.DispatchState, "Out for delivery", StringComparison.OrdinalIgnoreCase) ? 2 : 1,
                LastCheckIn = string.Equals(selectedDriver.DispatchState, "Out for delivery", StringComparison.OrdinalIgnoreCase) ? "5:18 PM" : "5:11 PM",
                NextAvailability = string.Equals(selectedDriver.DispatchState, "Out for delivery", StringComparison.OrdinalIgnoreCase) ? "5:34 PM" : "Ready now",
                AssignedVehicle = selectedDriver.DriverCode.EndsWith("7", StringComparison.OrdinalIgnoreCase) ? "Sedan 12" : "Hatchback 4",
                ManagerNote = string.Equals(selectedDriver.DispatchState, "Out for delivery", StringComparison.OrdinalIgnoreCase)
                    ? "Hold next stacked run until the office park gate clears."
                    : "Watch bag staging; double-check drinks before release."
            };
        }

        public IList<InventoryStockRecord> GetInventoryItems(string storeNumber)
        {
            var normalizedStoreNumber = NormalizeStoreNumber(storeNumber);
            return new List<InventoryStockRecord>
            {
                new InventoryStockRecord
                {
                    Sku = "DOUGH-16",
                    ItemName = "16\" dough trays",
                    Category = "Fresh prep",
                    OnHand = 26,
                    ParLevel = 32,
                    ReorderPoint = 28,
                    SupplierName = "Contoso Food Service",
                    AlertLevel = "Reorder today"
                },
                new InventoryStockRecord
                {
                    Sku = "CHEESE-WM",
                    ItemName = "Whole milk mozzarella",
                    Category = "Cold line",
                    OnHand = 19,
                    ParLevel = 18,
                    ReorderPoint = 14,
                    SupplierName = "Northwind Dairy",
                    AlertLevel = "Healthy"
                },
                new InventoryStockRecord
                {
                    Sku = "BOX-LG",
                    ItemName = "Large pizza boxes",
                    Category = "Packaging",
                    OnHand = normalizedStoreNumber == "031" ? 9 : 15,
                    ParLevel = 20,
                    ReorderPoint = 12,
                    SupplierName = "Adventure Packaging",
                    AlertLevel = normalizedStoreNumber == "031" ? "Critical" : "Reorder tomorrow"
                },
                new InventoryStockRecord
                {
                    Sku = "WINGS-HOT",
                    ItemName = "Hot wing sauce",
                    Category = "Dry goods",
                    OnHand = 11,
                    ParLevel = 10,
                    ReorderPoint = 6,
                    SupplierName = "Fourth Coffee Distribution",
                    AlertLevel = "Healthy"
                }
            };
        }

        public SupplierContactRecord GetSupplierProfile(string storeNumber, string sku)
        {
            var selectedItem = GetInventoryItems(storeNumber)
                .FirstOrDefault(record => string.Equals(record.Sku, sku, StringComparison.OrdinalIgnoreCase))
                ?? GetInventoryItems(storeNumber).First();

            return new SupplierContactRecord
            {
                SupplierName = selectedItem.SupplierName,
                PrimaryContact = selectedItem.SupplierName == "Contoso Food Service" ? "Dana Reeves" : selectedItem.SupplierName == "Northwind Dairy" ? "Luis Ortega" : "Mia Chen",
                ContactPhone = selectedItem.SupplierName == "Contoso Food Service" ? "(425) 555-0148" : selectedItem.SupplierName == "Northwind Dairy" ? "(425) 555-0161" : "(425) 555-0188",
                DeliveryWindow = selectedItem.SupplierName == "Contoso Food Service" ? "5:30 AM - 7:00 AM" : "6:00 AM - 8:00 AM",
                MinimumOrder = selectedItem.SupplierName == "Adventure Packaging" ? "$180 packaging drop" : "$120 mixed order",
                BackupSupplier = selectedItem.SupplierName == "Adventure Packaging" ? "Litware Packaging Co." : "Wide World Importers",
                ReorderRecommendation = selectedItem.AlertLevel == "Critical"
                    ? "Escalate to district ops and place same-night replenishment request."
                    : selectedItem.AlertLevel.StartsWith("Reorder", StringComparison.OrdinalIgnoreCase)
                        ? "Add this item to the next morning truck and verify par after dinner rush."
                        : "No action required beyond the standard next-count cycle."
            };
        }

        public StoreConfigurationRecord GetStoreConfiguration(string storeNumber)
        {
            var profile = GetStoreProfile(storeNumber);
            return new StoreConfigurationRecord
            {
                StoreNumber = profile.StoreNumber,
                StoreName = profile.StoreName,
                District = profile.District,
                WeekdayHours = profile.WeekdayHours,
                WeekendHours = profile.WeekendHours,
                DeliveryRadiusMiles = profile.DeliveryRadiusMiles,
                DispatchMode = profile.DispatchMode,
                DriverCap = profile.DriverCap,
                MakeLineCap = profile.MakeLineCap,
                CounterCap = profile.CounterCap,
                LastManagerReview = profile.LastManagerReview
            };
        }

        public IList<DeliveryZoneSettingRecord> GetDeliveryZones(string storeNumber)
        {
            var profile = GetStoreProfile(storeNumber);
            return new List<DeliveryZoneSettingRecord>
            {
                new DeliveryZoneSettingRecord
                {
                    ZoneCode = profile.StoreNumber + "-NW",
                    ZoneName = "Northwest corporate corridor",
                    RadiusMiles = profile.DeliveryRadiusMiles,
                    DispatchRule = "Stack only when gate access is confirmed",
                    DriverCap = 2
                },
                new DeliveryZoneSettingRecord
                {
                    ZoneCode = profile.StoreNumber + "-CTR",
                    ZoneName = "Center city core",
                    RadiusMiles = profile.DeliveryRadiusMiles - 1,
                    DispatchRule = "Hold drinks in the front pouch during lunch rush",
                    DriverCap = 1
                },
                new DeliveryZoneSettingRecord
                {
                    ZoneCode = profile.StoreNumber + "-SVC",
                    ZoneName = "Service road spillover",
                    RadiusMiles = profile.DeliveryRadiusMiles + 1,
                    DispatchRule = "Supervisor approval required after 8:30 PM",
                    DriverCap = 1
                }
            };
        }

        public IList<StaffScheduleRecord> GetWeeklySchedule(string storeNumber, DateTime weekOf)
        {
            var normalizedWeek = NormalizeWeek(weekOf);
            var profile = GetStoreProfile(storeNumber);
            return new List<StaffScheduleRecord>
            {
                new StaffScheduleRecord
                {
                    EmployeeCode = profile.StoreNumber + "-DRV1",
                    EmployeeName = "M. Delgado",
                    Role = "Shift supervisor",
                    Monday = "11a-7p",
                    Tuesday = "11a-7p",
                    Wednesday = "11a-7p",
                    Thursday = "Off",
                    Friday = "2p-10p",
                    Saturday = "2p-10p",
                    Sunday = "12p-8p",
                    WeeklyHours = 46,
                    Certification = "Manager serve-safe"
                },
                new StaffScheduleRecord
                {
                    EmployeeCode = profile.StoreNumber + "-DRV2",
                    EmployeeName = "J. Patel",
                    Role = "Driver lead",
                    Monday = "4p-10p",
                    Tuesday = "4p-10p",
                    Wednesday = "Off",
                    Thursday = "4p-10p",
                    Friday = "4p-11p",
                    Saturday = "2p-11p",
                    Sunday = "2p-9p",
                    WeeklyHours = 39,
                    Certification = "Driver trainer"
                },
                new StaffScheduleRecord
                {
                    EmployeeCode = profile.StoreNumber + "-MK1",
                    EmployeeName = "R. Nguyen",
                    Role = "Make-line captain",
                    Monday = "10a-6p",
                    Tuesday = "10a-6p",
                    Wednesday = "10a-6p",
                    Thursday = "10a-6p",
                    Friday = "12p-8p",
                    Saturday = "12p-8p",
                    Sunday = "Off",
                    WeeklyHours = 40,
                    Certification = "Oven + prep cross-trained"
                },
                new StaffScheduleRecord
                {
                    EmployeeCode = profile.StoreNumber + "-CTR1",
                    EmployeeName = "T. Morris",
                    Role = "Counter flex",
                    Monday = "Off",
                    Tuesday = "5p-9p",
                    Wednesday = "5p-9p",
                    Thursday = "5p-9p",
                    Friday = "5p-10p",
                    Saturday = "1p-9p",
                    Sunday = "1p-7p",
                    WeeklyHours = 29,
                    Certification = "Counter + phones"
                }
            };
        }

        public ShiftCoverageSummaryRecord GetShiftCoverage(string storeNumber, DateTime weekOf, string employeeCode)
        {
            var normalizedWeek = NormalizeWeek(weekOf);
            var selectedShift = GetWeeklySchedule(storeNumber, normalizedWeek)
                .FirstOrDefault(record => string.Equals(record.EmployeeCode, employeeCode, StringComparison.OrdinalIgnoreCase))
                ?? GetWeeklySchedule(storeNumber, normalizedWeek).First();

            return new ShiftCoverageSummaryRecord
            {
                EmployeeName = selectedShift.EmployeeName,
                Role = selectedShift.Role,
                WeekOf = normalizedWeek,
                WeeklyHours = selectedShift.WeeklyHours,
                Certification = selectedShift.Certification,
                CoverageNote = selectedShift.WeeklyHours > 40
                    ? "Review overtime buffer before approving another close."
                    : "Coverage is within the weekly staffing cap.",
                ManagerNote = selectedShift.Role.Contains("Driver")
                    ? "Keep this lead on stacked runs for the Friday dinner window."
                    : "Use as the first call-in if a lunch prep shift slips."
            };
        }

        private StoreProfile GetStoreProfile(string storeNumber)
        {
            return storeProfiles.FirstOrDefault(profile => string.Equals(profile.StoreNumber, NormalizeStoreNumber(storeNumber), StringComparison.OrdinalIgnoreCase))
                ?? storeProfiles.First();
        }

        private static IList<StoreProfile> BuildStoreProfiles()
        {
            return new List<StoreProfile>
            {
                new StoreProfile
                {
                    StoreNumber = "014",
                    StoreName = "Downtown Dispatch",
                    District = "North Metro",
                    WeekdayHours = "10:30 AM - 11:00 PM",
                    WeekendHours = "10:30 AM - 12:00 AM",
                    DeliveryRadiusMiles = 5,
                    DispatchMode = "Balanced",
                    DriverCap = 6,
                    MakeLineCap = 5,
                    CounterCap = 3,
                    LastManagerReview = "05/18/2026 5:10 PM"
                },
                new StoreProfile
                {
                    StoreNumber = "031",
                    StoreName = "Mall Annex",
                    District = "North Metro",
                    WeekdayHours = "10:00 AM - 10:00 PM",
                    WeekendHours = "10:00 AM - 11:00 PM",
                    DeliveryRadiusMiles = 4,
                    DispatchMode = "Rush Recovery",
                    DriverCap = 4,
                    MakeLineCap = 4,
                    CounterCap = 2,
                    LastManagerReview = "05/18/2026 4:54 PM"
                },
                new StoreProfile
                {
                    StoreNumber = "057",
                    StoreName = "Airport Service Road",
                    District = "South Metro",
                    WeekdayHours = "10:30 AM - 11:30 PM",
                    WeekendHours = "10:30 AM - 12:30 AM",
                    DeliveryRadiusMiles = 6,
                    DispatchMode = "Delivery Heavy",
                    DriverCap = 7,
                    MakeLineCap = 5,
                    CounterCap = 2,
                    LastManagerReview = "05/18/2026 5:02 PM"
                }
            };
        }

        private static string NormalizeStoreNumber(string storeNumber)
        {
            return string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
        }

        private static DateTime NormalizeWeek(DateTime weekOf)
        {
            if (weekOf == DateTime.MinValue)
            {
                return DefaultWeekStart;
            }

            var normalized = weekOf.Date;
            var offset = normalized.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)normalized.DayOfWeek - 1);
            return normalized.AddDays(-offset);
        }

        private static string GetDriverName(string driverCode)
        {
            switch (driverCode)
            {
                case "DRV-17":
                    return "C. Benson";
                case "DRV-03":
                    return "A. Harper";
                case "DRV-11":
                    return "S. Ortiz";
                default:
                    return "L. Kim";
            }
        }

        private class StoreProfile
        {
            public string StoreNumber { get; set; }

            public string StoreName { get; set; }

            public string District { get; set; }

            public string WeekdayHours { get; set; }

            public string WeekendHours { get; set; }

            public int DeliveryRadiusMiles { get; set; }

            public string DispatchMode { get; set; }

            public int DriverCap { get; set; }

            public int MakeLineCap { get; set; }

            public int CounterCap { get; set; }

            public string LastManagerReview { get; set; }
        }
    }

    public class StoreOption
    {
        public string StoreNumber { get; set; }

        public string DisplayName { get; set; }
    }

    public class DispatchDeliveryRecord
    {
        public int TicketId { get; set; }

        public string CustomerName { get; set; }

        public string DriverCode { get; set; }

        public string RouteZone { get; set; }

        public string DispatchState { get; set; }

        public string DriverStatus { get; set; }

        public int MinutesOpen { get; set; }

        public string PromiseWindow { get; set; }

        public string ReadyAt { get; set; }
    }

    public class DispatchDriverStatusRecord
    {
        public string DriverCode { get; set; }

        public string DriverName { get; set; }

        public string Status { get; set; }

        public string CurrentZone { get; set; }

        public int ActiveRuns { get; set; }

        public string LastCheckIn { get; set; }

        public string NextAvailability { get; set; }

        public string AssignedVehicle { get; set; }

        public string ManagerNote { get; set; }
    }

    public class InventoryStockRecord
    {
        public string Sku { get; set; }

        public string ItemName { get; set; }

        public string Category { get; set; }

        public int OnHand { get; set; }

        public int ParLevel { get; set; }

        public int ReorderPoint { get; set; }

        public string SupplierName { get; set; }

        public string AlertLevel { get; set; }
    }

    public class SupplierContactRecord
    {
        public string SupplierName { get; set; }

        public string PrimaryContact { get; set; }

        public string ContactPhone { get; set; }

        public string DeliveryWindow { get; set; }

        public string MinimumOrder { get; set; }

        public string BackupSupplier { get; set; }

        public string ReorderRecommendation { get; set; }
    }

    public class StoreConfigurationRecord
    {
        public string StoreNumber { get; set; }

        public string StoreName { get; set; }

        public string District { get; set; }

        public string WeekdayHours { get; set; }

        public string WeekendHours { get; set; }

        public int DeliveryRadiusMiles { get; set; }

        public string DispatchMode { get; set; }

        public int DriverCap { get; set; }

        public int MakeLineCap { get; set; }

        public int CounterCap { get; set; }

        public string LastManagerReview { get; set; }
    }

    public class DeliveryZoneSettingRecord
    {
        public string ZoneCode { get; set; }

        public string ZoneName { get; set; }

        public int RadiusMiles { get; set; }

        public string DispatchRule { get; set; }

        public int DriverCap { get; set; }
    }

    public class StaffScheduleRecord
    {
        public string EmployeeCode { get; set; }

        public string EmployeeName { get; set; }

        public string Role { get; set; }

        public string Monday { get; set; }

        public string Tuesday { get; set; }

        public string Wednesday { get; set; }

        public string Thursday { get; set; }

        public string Friday { get; set; }

        public string Saturday { get; set; }

        public string Sunday { get; set; }

        public int WeeklyHours { get; set; }

        public string Certification { get; set; }
    }

    public class ShiftCoverageSummaryRecord
    {
        public string EmployeeName { get; set; }

        public string Role { get; set; }

        public DateTime WeekOf { get; set; }

        public int WeeklyHours { get; set; }

        public string Certification { get; set; }

        public string CoverageNote { get; set; }

        public string ManagerNote { get; set; }
    }
}
