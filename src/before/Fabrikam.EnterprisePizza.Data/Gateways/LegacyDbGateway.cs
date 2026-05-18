using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Fabrikam.EnterprisePizza.Data.Gateways
{
    public class LegacyDbGateway
    {
        private readonly LegacyConnectionCatalog connectionCatalog;
        private readonly LegacyEnterpriseLibraryDatabaseFactory databaseFactory;
        private readonly IDictionary<string, Database> configuredDatabases;

        public LegacyDbGateway()
            : this(new LegacyConnectionCatalog())
        {
        }

        public LegacyDbGateway(LegacyConnectionCatalog connectionCatalog)
            : this(connectionCatalog, new LegacyEnterpriseLibraryDatabaseFactory(connectionCatalog, new DatabaseProviderFactory()))
        {
        }

        public LegacyDbGateway(LegacyConnectionCatalog connectionCatalog, LegacyEnterpriseLibraryDatabaseFactory databaseFactory)
        {
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
            this.databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
            configuredDatabases = new Dictionary<string, Database>(StringComparer.OrdinalIgnoreCase);
        }

        public string GetConnectionName(string area)
        {
            return connectionCatalog.GetConnectionName(area);
        }

        public string GetConnectionName(LegacyDatabaseArea area)
        {
            return connectionCatalog.GetConnectionName(area);
        }

        public Database GetDatabase(LegacyDatabaseArea area)
        {
            return GetDatabase(GetConnectionName(area));
        }

        public Database GetDatabase(string connectionName)
        {
            if (!configuredDatabases.ContainsKey(connectionName))
            {
                configuredDatabases[connectionName] = databaseFactory.Create(connectionName);
            }

            return configuredDatabases[connectionName];
        }

        public StoredProcedureCall CreateStoredProcedureCall(LegacyDatabaseArea area, string procedureName, params GatewayParameter[] parameters)
        {
            return new StoredProcedureCall(GetConnectionName(area), procedureName, parameters);
        }

        public DataSet ExecuteDataSet(StoredProcedureCall call)
        {
            if (call == null)
            {
                throw new ArgumentNullException(nameof(call));
            }

            GetDatabase(call.ConnectionName);

            switch (call.ProcedureName)
            {
                case LegacyStoredProcedures.StoreOps.GetActiveDispatchTickets:
                    return BuildDispatchTickets(call);
                case LegacyStoredProcedures.StoreOps.GetLatestPosImportBatch:
                    return BuildLatestPosImportBatch(call);
                case LegacyStoredProcedures.CustomerHub.GetPreferredPartners:
                    return BuildPreferredPartners(call);
                case LegacyStoredProcedures.Reporting.GetDimStoreCatalog:
                    return BuildDimStoreCatalog(call);
                case LegacyStoredProcedures.Reporting.GetDimDriverCatalog:
                    return BuildDimDriverCatalog(call);
                case LegacyStoredProcedures.Reporting.GetDimPartnerCatalog:
                    return BuildDimPartnerCatalog(call);
                case LegacyStoredProcedures.Reporting.GetDimTimeRange:
                    return BuildDimTimeRange(call);
                case LegacyStoredProcedures.Reporting.GetFactDeliveryByDate:
                    return BuildFactDeliveryByDate(call);
                case LegacyStoredProcedures.Reporting.GetFactOrderByDate:
                    return BuildFactOrderByDate(call);
                case LegacyStoredProcedures.Reporting.GetFactPartnerRevenueByMonth:
                    return BuildFactPartnerRevenueByMonth(call);
                case LegacyStoredProcedures.Reporting.GetDeliveryStoreDriverSnapshot:
                    return BuildDeliveryStoreDriverSnapshot(call);
                case LegacyStoredProcedures.Reporting.GetOrderChannelMixSnapshot:
                    return BuildOrderChannelMixSnapshot(call);
                case LegacyStoredProcedures.Reporting.GetPartnerRevenueSettlementSnapshot:
                    return BuildPartnerRevenueSettlementSnapshot(call);
                case LegacyStoredProcedures.Reporting.GetLaborCostSummary:
                    return BuildLaborCostSummary(call);
                case LegacyStoredProcedures.Reporting.GetOvertimeTrend:
                    return BuildOvertimeTrend(call);
                case LegacyStoredProcedures.Reporting.GetTurnoverSummary:
                    return BuildTurnoverSummary(call);
                case LegacyStoredProcedures.Reporting.GetStaffingSummary:
                    return BuildStaffingSummary(call);
                default:
                    throw new InvalidOperationException("No legacy stub exists for procedure " + call.ProcedureName + ".");
            }
        }

        private static DataSet BuildDispatchTickets(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var dataSet = CreateDataSet("DispatchTickets");
            var table = dataSet.Tables[0];
            table.Columns.Add("TicketId", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("DriverCode", typeof(string));
            table.Columns.Add("RouteZone", typeof(string));

            table.Rows.Add(4105, storeNumber, "DRV-17", "Northwest Corporate Corridor");
            table.Rows.Add(4106, storeNumber, "DRV-03", "Mall Annex");

            return dataSet;
        }

        private static DataSet BuildPreferredPartners(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("PreferredPartners");
            var table = dataSet.Tables[0];
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("AccountCode", typeof(string));
            table.Columns.Add("RelationshipTier", typeof(string));

            table.Rows.Add("Contoso Office Parks", "CORP-1002", "Gold");
            table.Rows.Add("Northwind Youth Sports League", "COMM-8821", "Community");
            table.Rows.Add("Adventure Works Bike Expo", "EVT-4405", "Seasonal");

            return dataSet;
        }

        private static DataSet BuildLatestPosImportBatch(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var dataSet = CreateDataSet("LatestPosImportBatch");
            var table = dataSet.Tables[0];
            table.Columns.Add("PosOrderImportBatchId", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("SourceSystem", typeof(string));
            table.Columns.Add("BatchDate", typeof(DateTime));
            table.Columns.Add("ImportedUtc", typeof(DateTime));
            table.Columns.Add("BatchStatus", typeof(string));
            table.Columns.Add("ItemCount", typeof(int));

            if (string.Equals(storeNumber, "999", StringComparison.OrdinalIgnoreCase))
            {
                return dataSet;
            }

            var batchDate = DateTime.UtcNow.Date;
            var importedUtc = batchDate.AddHours(6).AddMinutes(12);

            if (string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(8802, storeNumber, "RedmondPOS", batchDate, importedUtc, "Partial", 34);
            }
            else
            {
                table.Rows.Add(8801, storeNumber, "CampusPOS", batchDate, importedUtc, "Complete", 57);
            }

            return dataSet;
        }

        private static DataSet BuildDimStoreCatalog(StoredProcedureCall call)
        {
            var regionName = GetString(call, "@RegionName", null);
            var includeInactive = GetBoolean(call, "@IncludeInactive", false);
            var dataSet = CreateDataSet("DimStore");
            var table = dataSet.Tables[0];
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("StoreName", typeof(string));
            table.Columns.Add("RegionName", typeof(string));
            table.Columns.Add("MarketName", typeof(string));
            table.Columns.Add("OpenedDate", typeof(DateTime));
            table.Columns.Add("IsActive", typeof(bool));

            if (string.IsNullOrWhiteSpace(regionName)
                || string.Equals(regionName, "Seattle East", StringComparison.OrdinalIgnoreCase)
                || string.Equals(regionName, "Puget Sound", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(101, "014", "Redmond Ridge", "Seattle East", "Puget Sound", new DateTime(2004, 6, 15), true);
                table.Rows.Add(102, "022", "Bellevue Campus", "Seattle East", "Puget Sound", new DateTime(2005, 3, 10), true);
            }

            if (includeInactive && string.IsNullOrWhiteSpace(regionName))
            {
                table.Rows.Add(199, "099", "Tacoma Legacy", "South Sound", "Puget Sound", new DateTime(2002, 11, 9), false);
            }

            return dataSet;
        }

        private static DataSet BuildDimDriverCatalog(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", null);
            var includeInactive = GetBoolean(call, "@IncludeInactive", false);
            var dataSet = CreateDataSet("DimDriver");
            var table = dataSet.Tables[0];
            table.Columns.Add("DriverKey", typeof(int));
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("DriverCode", typeof(string));
            table.Columns.Add("DriverName", typeof(string));
            table.Columns.Add("EmploymentStatus", typeof(string));
            table.Columns.Add("ShiftType", typeof(string));
            table.Columns.Add("HireDate", typeof(DateTime));
            table.Columns.Add("IsActive", typeof(bool));

            if (string.IsNullOrWhiteSpace(storeNumber) || string.Equals(storeNumber, "014", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(1001, 101, "014", "DRV-17", "Mario Vasquez", "Active", "Dinner", new DateTime(2024, 2, 4), true);
                table.Rows.Add(1002, 101, "014", "DRV-03", "Elena Price", "Active", "Lunch", new DateTime(2023, 8, 21), true);
            }

            if (string.IsNullOrWhiteSpace(storeNumber) || string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(2001, 102, "022", "DRV-22", "Travis Cole", "Active", "Dinner", new DateTime(2025, 1, 14), true);
            }

            if (includeInactive && string.IsNullOrWhiteSpace(storeNumber))
            {
                table.Rows.Add(2999, 199, "099", "DRV-88", "Dennis Cobb", "Terminated", "Swing", new DateTime(2021, 5, 17), false);
            }

            return dataSet;
        }

        private static DataSet BuildDimPartnerCatalog(StoredProcedureCall call)
        {
            var partnerTier = GetString(call, "@PartnerTier", null);
            var includeInactive = GetBoolean(call, "@IncludeInactive", false);
            var dataSet = CreateDataSet("DimPartner");
            var table = dataSet.Tables[0];
            table.Columns.Add("PartnerKey", typeof(int));
            table.Columns.Add("PartnerCode", typeof(string));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("PartnerTier", typeof(string));
            table.Columns.Add("SettlementModel", typeof(string));
            table.Columns.Add("IsActive", typeof(bool));

            if (string.IsNullOrWhiteSpace(partnerTier) || string.Equals(partnerTier, "Gold", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(301, "PART-100", "Contoso Office Parks", "Gold", "Weekly ACH", true);
            }

            if (string.IsNullOrWhiteSpace(partnerTier) || string.Equals(partnerTier, "Community", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(302, "PART-200", "Northwind Youth Sports League", "Community", "Monthly Check", true);
            }

            if (includeInactive && string.IsNullOrWhiteSpace(partnerTier))
            {
                table.Rows.Add(399, "PART-999", "Litware Campus Events", "Seasonal", "Manual Settlement", false);
            }

            return dataSet;
        }

        private static DataSet BuildDimTimeRange(StoredProcedureCall call)
        {
            var startDate = GetDateTime(call, "@StartDate", DateTime.UtcNow.Date).Date;
            var endDate = GetDateTime(call, "@EndDate", startDate).Date;
            if (endDate < startDate)
            {
                endDate = startDate;
            }

            var dataSet = CreateDataSet("DimTime");
            var table = dataSet.Tables[0];
            table.Columns.Add("DateKey", typeof(int));
            table.Columns.Add("CalendarDate", typeof(DateTime));
            table.Columns.Add("CalendarMonthLabel", typeof(string));
            table.Columns.Add("FiscalQuarter", typeof(string));
            table.Columns.Add("DayName", typeof(string));
            table.Columns.Add("DayPart", typeof(string));
            table.Columns.Add("IsWeekend", typeof(bool));

            for (var current = startDate; current <= endDate; current = current.AddDays(1))
            {
                table.Rows.Add(
                    ToDateKey(current),
                    current,
                    current.ToString("yyyy-MM"),
                    "FY26-Q" + (((current.Month - 1) / 3) + 1),
                    current.DayOfWeek.ToString(),
                    current.DayOfWeek == DayOfWeek.Friday || current.DayOfWeek == DayOfWeek.Saturday ? "Dinner Rush" : "Weekday Core",
                    current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday);
            }

            return dataSet;
        }

        private static DataSet BuildFactDeliveryByDate(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var summaryDate = GetDateTime(call, "@SummaryDate", DateTime.UtcNow.Date).Date;
            var dataSet = CreateDataSet("FactDelivery");
            var table = dataSet.Tables[0];
            table.Columns.Add("DeliveryFactKey", typeof(int));
            table.Columns.Add("DateKey", typeof(int));
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("DriverKey", typeof(int));
            table.Columns.Add("OrderNumber", typeof(string));
            table.Columns.Add("CompletedRuns", typeof(int));
            table.Columns.Add("LateRuns", typeof(int));
            table.Columns.Add("DeliveryMinutes", typeof(decimal));
            table.Columns.Add("RouteMiles", typeof(decimal));
            table.Columns.Add("DeliveryRevenue", typeof(decimal));

            if (string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(7001, ToDateKey(summaryDate), 102, 2001, "DEL-22019", 1, 0, 24.10m, 7.20m, 68.40m);
            }
            else if (!string.Equals(storeNumber, "999", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(6001, ToDateKey(summaryDate), 101, 1001, "DEL-14012", 1, 0, 27.50m, 6.80m, 42.50m);
                table.Rows.Add(6002, ToDateKey(summaryDate), 101, 1002, "DEL-14015", 1, 1, 34.20m, 8.10m, 53.00m);
            }

            return dataSet;
        }

        private static DataSet BuildFactOrderByDate(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var summaryDate = GetDateTime(call, "@SummaryDate", DateTime.UtcNow.Date).Date;
            var dataSet = CreateDataSet("FactOrder");
            var table = dataSet.Tables[0];
            table.Columns.Add("OrderFactKey", typeof(int));
            table.Columns.Add("DateKey", typeof(int));
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("PartnerKey", typeof(int));
            table.Columns.Add("OrderChannel", typeof(string));
            table.Columns.Add("OrderCount", typeof(int));
            table.Columns.Add("AverageTicket", typeof(decimal));
            table.Columns.Add("GrossSales", typeof(decimal));
            table.Columns.Add("DiscountAmount", typeof(decimal));
            table.Columns.Add("NetSales", typeof(decimal));

            if (string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(8101, ToDateKey(summaryDate), 102, 302, "Catering", 3, 71.80m, 215.40m, 12.00m, 203.40m);
            }
            else if (!string.Equals(storeNumber, "999", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(8001, ToDateKey(summaryDate), 101, 301, "Web", 5, 28.60m, 143.00m, 8.50m, 134.50m);
                table.Rows.Add(8002, ToDateKey(summaryDate), 101, 0, "Phone", 4, 24.25m, 97.00m, 4.00m, 93.00m);
            }

            return dataSet;
        }

        private static DataSet BuildFactPartnerRevenueByMonth(StoredProcedureCall call)
        {
            var partnerCode = GetString(call, "@PartnerCode", "PART-100");
            var summaryMonth = GetMonthStart(GetDateTime(call, "@SummaryMonth", DateTime.UtcNow.Date));
            var dataSet = CreateDataSet("FactPartnerRevenue");
            var table = dataSet.Tables[0];
            table.Columns.Add("PartnerRevenueFactKey", typeof(int));
            table.Columns.Add("DateKey", typeof(int));
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("PartnerKey", typeof(int));
            table.Columns.Add("DeliveredOrders", typeof(int));
            table.Columns.Add("PartnerSales", typeof(decimal));
            table.Columns.Add("CommissionRate", typeof(decimal));
            table.Columns.Add("SettlementAmount", typeof(decimal));
            table.Columns.Add("NetMargin", typeof(decimal));

            if (string.Equals(partnerCode, "PART-200", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(9102, ToDateKey(summaryMonth), 102, 302, 26, 1442.10m, 0.14m, 201.89m, 318.11m);
            }
            else if (!string.Equals(partnerCode, "PART-999", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add(9101, ToDateKey(summaryMonth), 101, 301, 31, 1819.80m, 0.14m, 254.77m, 502.73m);
            }

            return dataSet;
        }

        private static DataSet BuildDeliveryStoreDriverSnapshot(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var summaryDate = GetDateTime(call, "@SummaryDate", DateTime.UtcNow.Date).Date;
            var dataSet = CreateDataSet("DeliveryStoreDriverSnapshot");
            var table = dataSet.Tables[0];
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("StoreName", typeof(string));
            table.Columns.Add("CalendarDate", typeof(DateTime));
            table.Columns.Add("DriverCode", typeof(string));
            table.Columns.Add("DriverName", typeof(string));
            table.Columns.Add("CompletedRuns", typeof(int));
            table.Columns.Add("LateRuns", typeof(int));
            table.Columns.Add("DeliveryMinutes", typeof(decimal));
            table.Columns.Add("RouteMiles", typeof(decimal));
            table.Columns.Add("DeliveryRevenue", typeof(decimal));

            if (string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add("022", "Bellevue Campus", summaryDate, "DRV-22", "Travis Cole", 1, 0, 24.10m, 7.20m, 68.40m);
            }
            else if (!string.Equals(storeNumber, "999", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add("014", "Redmond Ridge", summaryDate, "DRV-17", "Mario Vasquez", 1, 0, 27.50m, 6.80m, 42.50m);
                table.Rows.Add("014", "Redmond Ridge", summaryDate, "DRV-03", "Elena Price", 1, 1, 34.20m, 8.10m, 53.00m);
            }

            return dataSet;
        }

        private static DataSet BuildOrderChannelMixSnapshot(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var summaryDate = GetDateTime(call, "@SummaryDate", DateTime.UtcNow.Date).Date;
            var dataSet = CreateDataSet("OrderChannelMixSnapshot");
            var table = dataSet.Tables[0];
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("StoreName", typeof(string));
            table.Columns.Add("CalendarDate", typeof(DateTime));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("OrderChannel", typeof(string));
            table.Columns.Add("OrderCount", typeof(int));
            table.Columns.Add("GrossSales", typeof(decimal));
            table.Columns.Add("DiscountAmount", typeof(decimal));
            table.Columns.Add("NetSales", typeof(decimal));

            if (string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add("022", "Bellevue Campus", summaryDate, "Northwind Youth Sports League", "Catering", 3, 215.40m, 12.00m, 203.40m);
            }
            else if (!string.Equals(storeNumber, "999", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add("014", "Redmond Ridge", summaryDate, "Contoso Office Parks", "Web", 5, 143.00m, 8.50m, 134.50m);
                table.Rows.Add("014", "Redmond Ridge", summaryDate, DBNull.Value, "Phone", 4, 97.00m, 4.00m, 93.00m);
            }

            return dataSet;
        }

        private static DataSet BuildPartnerRevenueSettlementSnapshot(StoredProcedureCall call)
        {
            var partnerCode = GetString(call, "@PartnerCode", "PART-100");
            var summaryMonth = GetMonthStart(GetDateTime(call, "@SummaryMonth", DateTime.UtcNow.Date));
            var dataSet = CreateDataSet("PartnerRevenueSettlementSnapshot");
            var table = dataSet.Tables[0];
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("StoreName", typeof(string));
            table.Columns.Add("CalendarMonthLabel", typeof(string));
            table.Columns.Add("PartnerCode", typeof(string));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("DeliveredOrders", typeof(int));
            table.Columns.Add("PartnerSales", typeof(decimal));
            table.Columns.Add("CommissionRate", typeof(decimal));
            table.Columns.Add("SettlementAmount", typeof(decimal));
            table.Columns.Add("NetMargin", typeof(decimal));

            if (string.Equals(partnerCode, "PART-200", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add("022", "Bellevue Campus", summaryMonth.ToString("yyyy-MM"), "PART-200", "Northwind Youth Sports League", 26, 1442.10m, 0.14m, 201.89m, 318.11m);
            }
            else if (!string.Equals(partnerCode, "PART-999", StringComparison.OrdinalIgnoreCase))
            {
                table.Rows.Add("014", "Redmond Ridge", summaryMonth.ToString("yyyy-MM"), "PART-100", "Contoso Office Parks", 31, 1819.80m, 0.14m, 254.77m, 502.73m);
            }

            return dataSet;
        }

        private static DataSet BuildLaborCostSummary(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var summaryDate = GetDateTime(call, "@SummaryDate", DateTime.UtcNow.Date);
            var dataSet = CreateDataSet("LaborCostSummary");
            var table = dataSet.Tables[0];
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("SummaryDate", typeof(DateTime));
            table.Columns.Add("ScheduledHours", typeof(decimal));
            table.Columns.Add("WorkedHours", typeof(decimal));
            table.Columns.Add("OvertimeHours", typeof(decimal));
            table.Columns.Add("RegularLaborCost", typeof(decimal));
            table.Columns.Add("OvertimeLaborCost", typeof(decimal));
            table.Columns.Add("AgencyLaborCost", typeof(decimal));
            table.Columns.Add("NetSales", typeof(decimal));
            table.Columns.Add("LaborCostPercentageOfSales", typeof(decimal));

            if (string.Equals(storeNumber, "999", StringComparison.OrdinalIgnoreCase))
            {
                return dataSet;
            }

            var row = table.NewRow();
            row["StoreNumber"] = storeNumber;
            row["SummaryDate"] = summaryDate;
            if (string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase))
            {
                row["ScheduledHours"] = 118.0m;
                row["WorkedHours"] = 120.5m;
                row["OvertimeHours"] = 2.5m;
                row["RegularLaborCost"] = 1711.00m;
                row["OvertimeLaborCost"] = 68.25m;
                row["AgencyLaborCost"] = 0.00m;
                row["NetSales"] = 5940.00m;
                row["LaborCostPercentageOfSales"] = 29.95m;
            }
            else
            {
                row["ScheduledHours"] = 164.0m;
                row["WorkedHours"] = 171.5m;
                row["OvertimeHours"] = 7.5m;
                row["RegularLaborCost"] = 2448.00m;
                row["OvertimeLaborCost"] = 213.75m;
                row["AgencyLaborCost"] = 96.00m;
                row["NetSales"] = 8200.00m;
                row["LaborCostPercentageOfSales"] = 33.63m;
            }

            table.Rows.Add(row);
            return dataSet;
        }

        private static DataSet BuildOvertimeTrend(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var weeksBack = GetInt32(call, "@WeeksBack", 4);
            var dataSet = CreateDataSet("OvertimeTrend");
            var table = dataSet.Tables[0];
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("WeekEndingDate", typeof(DateTime));
            table.Columns.Add("DriverOvertimeHours", typeof(decimal));
            table.Columns.Add("KitchenOvertimeHours", typeof(decimal));
            table.Columns.Add("ShiftLeadOvertimeHours", typeof(decimal));
            table.Columns.Add("TotalOvertimeHours", typeof(decimal));

            if (string.Equals(storeNumber, "999", StringComparison.OrdinalIgnoreCase))
            {
                return dataSet;
            }

            var today = DateTime.UtcNow.Date;
            var seed = string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase)
                ? new[] { (2.25m, 1.25m, 0.50m), (2.00m, 1.50m, 0.50m), (1.75m, 1.25m, 0.25m), (1.50m, 1.00m, 0.25m) }
                : new[] { (6.00m, 3.00m, 1.50m), (5.75m, 2.75m, 1.50m), (4.50m, 2.50m, 1.25m), (5.00m, 2.00m, 1.00m) };

            for (var index = 0; index < weeksBack && index < seed.Length; index++)
            {
                var row = table.NewRow();
                row["StoreNumber"] = storeNumber;
                row["WeekEndingDate"] = today.AddDays(index * -7);
                row["DriverOvertimeHours"] = seed[index].Item1;
                row["KitchenOvertimeHours"] = seed[index].Item2;
                row["ShiftLeadOvertimeHours"] = seed[index].Item3;
                row["TotalOvertimeHours"] = seed[index].Item1 + seed[index].Item2 + seed[index].Item3;
                table.Rows.Add(row);
            }

            return dataSet;
        }

        private static DataSet BuildTurnoverSummary(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var summaryMonth = GetDateTime(call, "@SummaryMonth", new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1));
            var dataSet = CreateDataSet("TurnoverSummary");
            var table = dataSet.Tables[0];
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("SummaryMonth", typeof(DateTime));
            table.Columns.Add("BeginningHeadcount", typeof(int));
            table.Columns.Add("HireCount", typeof(int));
            table.Columns.Add("SeparationCount", typeof(int));
            table.Columns.Add("EndingHeadcount", typeof(int));
            table.Columns.Add("TurnoverRate", typeof(decimal));

            if (string.Equals(storeNumber, "999", StringComparison.OrdinalIgnoreCase))
            {
                return dataSet;
            }

            var row = table.NewRow();
            row["StoreNumber"] = storeNumber;
            row["SummaryMonth"] = new DateTime(summaryMonth.Year, summaryMonth.Month, 1);
            if (string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase))
            {
                row["BeginningHeadcount"] = 19;
                row["HireCount"] = 1;
                row["SeparationCount"] = 1;
                row["EndingHeadcount"] = 19;
                row["TurnoverRate"] = 5.26m;
            }
            else
            {
                row["BeginningHeadcount"] = 27;
                row["HireCount"] = 3;
                row["SeparationCount"] = 2;
                row["EndingHeadcount"] = 28;
                row["TurnoverRate"] = 7.41m;
            }

            table.Rows.Add(row);
            return dataSet;
        }

        private static DataSet BuildStaffingSummary(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var summaryDate = GetDateTime(call, "@SummaryDate", DateTime.UtcNow.Date);
            var dataSet = CreateDataSet("StaffingSummary");
            var table = dataSet.Tables[0];
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("SummaryDate", typeof(DateTime));
            table.Columns.Add("ScheduledDriverSlots", typeof(int));
            table.Columns.Add("FilledDriverSlots", typeof(int));
            table.Columns.Add("OpenDriverSlots", typeof(int));
            table.Columns.Add("CrossTrainedTeamMembers", typeof(int));
            table.Columns.Add("CalloutCount", typeof(int));
            table.Columns.Add("StaffingCoverageRate", typeof(decimal));

            if (string.Equals(storeNumber, "999", StringComparison.OrdinalIgnoreCase))
            {
                return dataSet;
            }

            var row = table.NewRow();
            row["StoreNumber"] = storeNumber;
            row["SummaryDate"] = summaryDate;
            if (string.Equals(storeNumber, "022", StringComparison.OrdinalIgnoreCase))
            {
                row["ScheduledDriverSlots"] = 12;
                row["FilledDriverSlots"] = 11;
                row["OpenDriverSlots"] = 1;
                row["CrossTrainedTeamMembers"] = 1;
                row["CalloutCount"] = 0;
                row["StaffingCoverageRate"] = 91.67m;
            }
            else
            {
                row["ScheduledDriverSlots"] = 18;
                row["FilledDriverSlots"] = 15;
                row["OpenDriverSlots"] = 3;
                row["CrossTrainedTeamMembers"] = 2;
                row["CalloutCount"] = 1;
                row["StaffingCoverageRate"] = 83.33m;
            }

            table.Rows.Add(row);
            return dataSet;
        }

        private static DataSet CreateDataSet(string tableName)
        {
            var dataSet = new DataSet("LegacyReporting");
            dataSet.Tables.Add(new DataTable(tableName));
            return dataSet;
        }

        private static GatewayParameter GetParameter(StoredProcedureCall call, string name)
        {
            return call.Parameters.FirstOrDefault(parameter => string.Equals(parameter.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetString(StoredProcedureCall call, string name, string defaultValue)
        {
            var parameter = GetParameter(call, name);
            return parameter == null || parameter.Value == null || parameter.Value == DBNull.Value
                ? defaultValue
                : Convert.ToString(parameter.Value);
        }

        private static DateTime GetDateTime(StoredProcedureCall call, string name, DateTime defaultValue)
        {
            var parameter = GetParameter(call, name);
            return parameter == null || parameter.Value == null || parameter.Value == DBNull.Value
                ? defaultValue
                : Convert.ToDateTime(parameter.Value);
        }

        private static int GetInt32(StoredProcedureCall call, string name, int defaultValue)
        {
            var parameter = GetParameter(call, name);
            return parameter == null || parameter.Value == null || parameter.Value == DBNull.Value
                ? defaultValue
                : Convert.ToInt32(parameter.Value);
        }

        private static bool GetBoolean(StoredProcedureCall call, string name, bool defaultValue)
        {
            var parameter = GetParameter(call, name);
            return parameter == null || parameter.Value == null || parameter.Value == DBNull.Value
                ? defaultValue
                : Convert.ToBoolean(parameter.Value);
        }

        private static DateTime GetMonthStart(DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        private static int ToDateKey(DateTime value)
        {
            return (value.Year * 10000) + (value.Month * 100) + value.Day;
        }
    }
}
