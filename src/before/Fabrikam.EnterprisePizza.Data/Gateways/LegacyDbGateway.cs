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
        private static readonly DateTime SampleTimestampUtc = new DateTime(2026, 5, 18, 8, 39, 47, 894, DateTimeKind.Utc);
        private readonly LegacyConnectionCatalog connectionCatalog;
        private readonly LegacyEnterpriseLibraryDatabaseFactory databaseFactory;
        private readonly IDictionary<string, Database> configuredDatabases;
        private readonly LegacyDatabaseFactory databaseFactory;

        public LegacyDbGateway()
            : this(new LegacyConnectionCatalog(), new LegacyDatabaseFactory())
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
            : this(connectionCatalog, new LegacyDatabaseFactory())
        {
        }

        public LegacyDbGateway(LegacyConnectionCatalog connectionCatalog, LegacyDatabaseFactory databaseFactory)
        {
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
            this.databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
        }

        public virtual string GetConnectionName(string area)
        {
            return connectionCatalog.GetConnectionName(area);
        }

        public virtual string GetConnectionName(LegacyDatabaseArea area)
        {
            return databaseFactory.CreateDatabase(area).ConnectionName;
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
        public virtual StoredProcedureCall CreateStoredProcedureCall(LegacyDatabaseArea area, string procedureName, params GatewayParameter[] parameters)
        {
            return new StoredProcedureCall(GetConnectionName(area), procedureName, parameters);
        }

        public virtual DataSet ExecuteDataSet(StoredProcedureCall call)
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
                case LegacyStoredProcedures.StoreOps.GetOrder:
                    return BuildOrder(call);
                case LegacyStoredProcedures.StoreOps.SearchOrders:
                    return BuildOrderSearchResults(call);
                case LegacyStoredProcedures.StoreOps.PlaceOrder:
                    return BuildPlacedOrder(call);
                case LegacyStoredProcedures.StoreOps.UpdateOrderStatus:
                    return BuildUpdatedOrder(call);
                case LegacyStoredProcedures.StoreOps.GetOrderHistory:
                    return BuildOrderHistory(call);
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
                case LegacyStoredProcedures.CustomerHub.GetPartnerProfile:
                    return BuildPartnerProfile(call);
                case LegacyStoredProcedures.CustomerHub.RegisterPartner:
                    return BuildRegisteredPartner(call);
                case LegacyStoredProcedures.CustomerHub.UpdatePartnerContract:
                    return BuildPartnerContract(call);
                case LegacyStoredProcedures.CustomerHub.GetPartnerReferrals:
                    return BuildPartnerReferrals(call);
                case LegacyStoredProcedures.CustomerHub.SubmitPartnerReferral:
                    return BuildSubmittedReferral(call);
                case LegacyStoredProcedures.CustomerHub.ProcessPartnerCommission:
                    return BuildCommissionProcessing(call);
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
            table.Columns.Add("CustomerName", typeof(string));
            table.Columns.Add("DeliveryAddress", typeof(string));
            table.Columns.Add("ReadyAtLocal", typeof(DateTime));
            table.Columns.Add("PromiseTimeLocal", typeof(DateTime));
            table.Columns.Add("RouteDistanceMiles", typeof(decimal));
            table.Columns.Add("EstimatedTravelMinutes", typeof(int));
            table.Columns.Add("RequiresPairing", typeof(bool));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("PriorityScore", typeof(int));

            var routeDeskStart = DateTime.Today.AddHours(17);
            table.Rows.Add(4105, storeNumber, "DRV-17", "Northwest Corporate Corridor", "Contoso Office Park", "8100 148th Ave NE", routeDeskStart.AddMinutes(12), routeDeskStart.AddMinutes(32), 6.8m, 18, false, "ReadyForDispatch", 92);
            table.Rows.Add(4106, storeNumber, "DRV-03", "Mall Annex", "Mall Annex Leasing Office", "245 Center Mall Plaza", routeDeskStart.AddMinutes(16), routeDeskStart.AddMinutes(36), 4.2m, 14, true, "Assigned", 84);

            return dataSet;
        }

        private static DataSet BuildPreferredPartners(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("PreferredPartners");
            var table = dataSet.Tables[0];
            table.Columns.Add("PartnerCode", typeof(string));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("RelationshipTier", typeof(string));
            table.Columns.Add("PreferredStoreNumber", typeof(string));
            table.Columns.Add("AccountCode", typeof(string));
            table.Columns.Add("AccountName", typeof(string));

            table.Rows.Add("CORP-1002", "Contoso Office Parks", "Gold", "014", "CAT-0140", "Fabrikam Regional Catering Desk");
            table.Rows.Add("COMM-8821", "Northwind Youth Sports League", "Community", "014", "LEAGUE-8821", "Northwind League Concessions");
            table.Rows.Add("EVT-4405", "Adventure Works Bike Expo", "Seasonal", "022", "EVENT-4405", "Adventure Works Expo Events");

            return dataSet;
        }

        private static DataSet BuildPartnerProfile(StoredProcedureCall call)
        {
            var partnerId = GetString(call, "@PartnerId", "PARTNER-1002");
            var dataSet = CreateDataSet("PartnerProfile");
            var table = dataSet.Tables[0];
            table.Columns.Add("PartnerId", typeof(string));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("AccountCode", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("RelationshipTier", typeof(string));
            table.Columns.Add("ContractCode", typeof(string));
            table.Columns.Add("PrimaryContact", typeof(string));
            table.Columns.Add("ActiveSinceUtc", typeof(DateTime));
            table.Columns.Add("IsPreferred", typeof(bool));
            table.Columns.Add("StatusUpdatedAtUtc", typeof(DateTime));

            var row = table.NewRow();
            row["PartnerId"] = partnerId;
            if (string.Equals(partnerId, "PARTNER-8821", StringComparison.OrdinalIgnoreCase))
            {
                row["PartnerName"] = "Northwind Youth Sports League";
                row["AccountCode"] = "COMM-8821";
                row["Status"] = "SeasonApproved";
                row["RelationshipTier"] = "Community";
                row["ContractCode"] = "COMM-SPRING-26";
                row["PrimaryContact"] = "League Events Desk";
                row["ActiveSinceUtc"] = SampleTimestampUtc.AddDays(-75);
                row["IsPreferred"] = false;
                row["StatusUpdatedAtUtc"] = SampleTimestampUtc.AddDays(-6);
            }
            else if (string.Equals(partnerId, "PARTNER-4405", StringComparison.OrdinalIgnoreCase))
            {
                row["PartnerName"] = "Adventure Works Bike Expo";
                row["AccountCode"] = "EVT-4405";
                row["Status"] = "PendingRenewal";
                row["RelationshipTier"] = "Seasonal";
                row["ContractCode"] = "EXPO-SUMMER-26";
                row["PrimaryContact"] = "Expo Hospitality Desk";
                row["ActiveSinceUtc"] = SampleTimestampUtc.AddDays(-210);
                row["IsPreferred"] = false;
                row["StatusUpdatedAtUtc"] = SampleTimestampUtc.AddDays(-2);
            }
            else
            {
                row["PartnerName"] = "Contoso Office Parks";
                row["AccountCode"] = "CORP-1002";
                row["Status"] = "Active";
                row["RelationshipTier"] = "Gold";
                row["ContractCode"] = "CORP-FY26";
                row["PrimaryContact"] = "Contoso Facilities Team";
                row["ActiveSinceUtc"] = SampleTimestampUtc.AddDays(-420);
                row["IsPreferred"] = true;
                row["StatusUpdatedAtUtc"] = SampleTimestampUtc.AddDays(-1);
            }

            table.Rows.Add(row);
            return dataSet;
        }

        private static DataSet BuildRegisteredPartner(StoredProcedureCall call)
        {
            var partnerName = GetString(call, "@PartnerName", "Wingtip Business Catering");
            var accountCode = GetString(call, "@AccountCode", "CORP-9100");
            var relationshipTier = GetString(call, "@RelationshipTier", "Preferred");
            var primaryContact = GetString(call, "@PrimaryContact", "Partner Operations Desk");
            var dataSet = CreateDataSet("PartnerProfile");
            var table = dataSet.Tables[0];
            table.Columns.Add("PartnerId", typeof(string));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("AccountCode", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("RelationshipTier", typeof(string));
            table.Columns.Add("ContractCode", typeof(string));
            table.Columns.Add("PrimaryContact", typeof(string));
            table.Columns.Add("ActiveSinceUtc", typeof(DateTime));
            table.Columns.Add("IsPreferred", typeof(bool));
            table.Columns.Add("StatusUpdatedAtUtc", typeof(DateTime));
            table.Rows.Add(BuildPartnerId(accountCode, "PARTNER-9100"), partnerName, accountCode, "PendingApproval", relationshipTier, "PENDING-REVIEW", primaryContact, SampleTimestampUtc, true, SampleTimestampUtc);
            return dataSet;
        }

        private static DataSet BuildPartnerContract(StoredProcedureCall call)
        {
            var effectiveDate = GetDateTime(call, "@EffectiveDateUtc", SampleTimestampUtc.Date);
            var expirationDate = GetDateTime(call, "@ExpirationDateUtc", SampleTimestampUtc.Date.AddYears(1));
            var dataSet = CreateDataSet("PartnerContract");
            var table = dataSet.Tables[0];
            table.Columns.Add("PartnerId", typeof(string));
            table.Columns.Add("ContractCode", typeof(string));
            table.Columns.Add("PricingPlanCode", typeof(string));
            table.Columns.Add("EffectiveDateUtc", typeof(DateTime));
            table.Columns.Add("ExpirationDateUtc", typeof(DateTime));
            table.Columns.Add("IsAutoRenew", typeof(bool));
            table.Rows.Add(
                GetString(call, "@PartnerId", "PARTNER-1002"),
                GetString(call, "@ContractCode", "CORP-FY26"),
                GetString(call, "@PricingPlanCode", "B2B-CATERING"),
                effectiveDate,
                expirationDate,
                GetBoolean(call, "@IsAutoRenew", true));
            return dataSet;
        }

        private static DataSet BuildPartnerReferrals(StoredProcedureCall call)
        {
            var partnerId = GetString(call, "@PartnerId", "PARTNER-1002");
            var dataSet = CreateDataSet("PartnerReferrals");
            var table = dataSet.Tables[0];
            table.Columns.Add("ReferralId", typeof(string));
            table.Columns.Add("PartnerId", typeof(string));
            table.Columns.Add("ReferredAccountName", typeof(string));
            table.Columns.Add("ReferralChannel", typeof(string));
            table.Columns.Add("ReferralStatus", typeof(string));
            table.Columns.Add("SubmittedBy", typeof(string));
            table.Columns.Add("SubmittedAtUtc", typeof(DateTime));
            table.Rows.Add(partnerId + "-REF-001", partnerId, "Fourth Coffee Campus Events", "CATERING", "Qualified", "legacy-sync@fabrikam.com", SampleTimestampUtc.AddDays(-14));
            table.Rows.Add(partnerId + "-REF-002", partnerId, "Graphic Design Institute", "FRANCHISE", "PendingReview", "legacy-sync@fabrikam.com", SampleTimestampUtc.AddDays(-3));
            return dataSet;
        }

        private static DataSet BuildSubmittedReferral(StoredProcedureCall call)
        {
            var partnerId = GetString(call, "@PartnerId", "PARTNER-1002");
            var accountName = GetString(call, "@ReferredAccountName", "New Corporate Lunch Program");
            var dataSet = CreateDataSet("PartnerReferrals");
            var table = dataSet.Tables[0];
            table.Columns.Add("ReferralId", typeof(string));
            table.Columns.Add("PartnerId", typeof(string));
            table.Columns.Add("ReferredAccountName", typeof(string));
            table.Columns.Add("ReferralChannel", typeof(string));
            table.Columns.Add("ReferralStatus", typeof(string));
            table.Columns.Add("SubmittedBy", typeof(string));
            table.Columns.Add("SubmittedAtUtc", typeof(DateTime));
            table.Rows.Add(partnerId + "-REF-NEW", partnerId, accountName, GetString(call, "@ChannelCode", "LEGACY-SOAP"), "QueuedForReview", GetString(call, "@SubmittedBy", "legacy-portal"), SampleTimestampUtc);
            return dataSet;
        }

        private static DataSet BuildCommissionProcessing(StoredProcedureCall call)
        {
            var grossSales = GetDecimal(call, "@GrossSalesAmount", 12450.00m);
            var commissionRatePercent = GetDecimal(call, "@CommissionRatePercent", 8.5m);
            var dataSet = CreateDataSet("CommissionProcessing");
            var table = dataSet.Tables[0];
            table.Columns.Add("PartnerId", typeof(string));
            table.Columns.Add("SettlementBatchId", typeof(string));
            table.Columns.Add("CommissionAmount", typeof(decimal));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("ProcessedAtUtc", typeof(DateTime));
            table.Columns.Add("ProcessedBy", typeof(string));
            table.Rows.Add(
                GetString(call, "@PartnerId", "PARTNER-1002"),
                GetString(call, "@SettlementBatchId", "SETTLE-2026-05-B"),
                decimal.Round(grossSales * (commissionRatePercent / 100m), 2, MidpointRounding.AwayFromZero),
                "PostedToSettlementBatch",
                SampleTimestampUtc,
                GetString(call, "@RequestedBy", "PartnerBilling.Job"));
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
        private static DataSet BuildOrder(StoredProcedureCall call)
        {
            var orderNumber = GetInt32(call, "@OrderNumber", 74105);
            var dataSet = CreateDataSet("OrderDetails");
            var table = dataSet.Tables[0];
            AddOrderColumns(table);

            var profile = CreateOrderProfile(orderNumber, null);
            if (profile != null)
            {
                AddOrderRow(table, profile);
            }

            return dataSet;
        }

        private static DataSet BuildOrderSearchResults(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var searchText = GetString(call, "@SearchText", string.Empty);
            var statusCode = GetString(call, "@StatusCode", string.Empty);
            var fromSubmittedUtc = GetDateTime(call, "@FromSubmittedUtc", DateTime.MinValue);
            var toSubmittedUtc = GetDateTime(call, "@ToSubmittedUtc", DateTime.MinValue);
            var includeClosed = GetBoolean(call, "@IncludeClosed", false);
            var dataSet = CreateDataSet("OrderSearch");
            var table = dataSet.Tables[0];
            table.Columns.Add("OrderNumber", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("CustomerName", typeof(string));
            table.Columns.Add("Channel", typeof(string));
            table.Columns.Add("ServiceMode", typeof(string));
            table.Columns.Add("PromiseWindow", typeof(string));
            table.Columns.Add("TicketTotal", typeof(decimal));
            table.Columns.Add("KitchenStatus", typeof(string));
            table.Columns.Add("DispatchStatus", typeof(string));
            table.Columns.Add("PaymentStatus", typeof(string));

            foreach (var profile in BuildSearchCandidates(storeNumber)
                .Where(candidate => includeClosed || !IsClosed(candidate))
                .Where(candidate => MatchesSearch(candidate, searchText))
                .Where(candidate => MatchesStatus(candidate, statusCode))
                .Where(candidate => fromSubmittedUtc == DateTime.MinValue || candidate.SubmittedAtUtc >= fromSubmittedUtc.ToUniversalTime())
                .Where(candidate => toSubmittedUtc == DateTime.MinValue || candidate.SubmittedAtUtc <= toSubmittedUtc.ToUniversalTime())
                .OrderBy(candidate => candidate.QuotedReadyTimeUtc)
                .ThenBy(candidate => candidate.OrderNumber))
            {
                table.Rows.Add(
                    profile.OrderNumber,
                    profile.StoreNumber,
                    profile.CustomerName,
                    profile.Channel,
                    profile.ServiceMode,
                    profile.QuotedReadyTimeUtc.ToLocalTime().ToString("h:mm tt"),
                    profile.TicketTotal,
                    profile.KitchenStatus,
                    profile.DispatchStatus,
                    profile.PaymentStatus);
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
        private static DataSet BuildPlacedOrder(StoredProcedureCall call)
        {
            var storeNumber = GetString(call, "@StoreNumber", "014");
            var submittedAtUtc = GetDateTime(call, "@SubmittedAtUtc", DateTime.UtcNow);
            var neededByUtc = GetDateTime(call, "@NeededByUtc", submittedAtUtc.AddMinutes(35));
            var quotedReadyTimeUtc = GetDateTime(call, "@QuotedReadyTimeUtc", neededByUtc);
            var profile = new OrderStubProfile
            {
                OrderNumber = 79000 + ResolveStoreSeed(storeNumber),
                StoreNumber = storeNumber,
                CustomerName = GetString(call, "@CustomerName", "Walk-Up Guest"),
                Channel = GetString(call, "@Channel", "Web"),
                ServiceMode = GetString(call, "@ServiceMode", "Delivery"),
                OrderStatus = "Placed",
                KitchenStatus = "Queued",
                DispatchStatus = string.Equals(GetString(call, "@ServiceMode", "Delivery"), "Carryout", StringComparison.OrdinalIgnoreCase) ? "Counter Hold" : "Dispatch Review",
                PaymentStatus = "Card Hold",
                TicketTotal = GetDecimal(call, "@TicketTotal", 0m),
                SubmittedAtUtc = submittedAtUtc.ToUniversalTime(),
                NeededByUtc = neededByUtc.ToUniversalTime(),
                QuotedReadyTimeUtc = quotedReadyTimeUtc.ToUniversalTime(),
                FulfillmentLane = GetString(call, "@FulfillmentLane", "StandardMakeLine"),
                DeliveryAddress = GetString(call, "@DeliveryAddress", "Front Desk Pickup"),
                SpecialInstructions = GetString(call, "@SpecialInstructions", string.Empty),
                IsCorporateAccount = GetBoolean(call, "@IsCorporateAccount", false),
                DeliveryMileage = GetDecimal(call, "@DeliveryMileage", 0m),
                CurrentDriverCode = string.Empty
            };

            var dataSet = CreateDataSet("OrderDetails");
            var table = dataSet.Tables[0];
            AddOrderColumns(table);
            AddOrderRow(table, profile);
            return dataSet;
        }

        private static DataSet BuildUpdatedOrder(StoredProcedureCall call)
        {
            var orderNumber = GetInt32(call, "@OrderNumber", 74105);
            var storeNumber = GetString(call, "@StoreNumber", null);
            var updatedStatus = GetString(call, "@StatusCode", "In Progress");
            var updatedAtUtc = GetDateTime(call, "@UpdatedAtUtc", DateTime.UtcNow);
            var note = GetString(call, "@StatusNote", string.Empty);
            var profile = CreateOrderProfile(orderNumber, storeNumber);
            if (profile != null)
            {
                ApplyStatusOverride(profile, updatedStatus, note, updatedAtUtc.ToUniversalTime());
            }

            var dataSet = CreateDataSet("OrderDetails");
            var table = dataSet.Tables[0];
            AddOrderColumns(table);
            if (profile != null)
            {
                AddOrderRow(table, profile);
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

        private static DataSet BuildOrderHistory(StoredProcedureCall call)
        {
            var orderNumber = GetInt32(call, "@OrderNumber", 74105);
            var profile = CreateOrderProfile(orderNumber, null);
            var dataSet = CreateDataSet("OrderHistory");
            var table = dataSet.Tables[0];
            table.Columns.Add("LoggedAtUtc", typeof(DateTime));
            table.Columns.Add("StatusCode", typeof(string));
            table.Columns.Add("Note", typeof(string));
            table.Columns.Add("UpdatedBy", typeof(string));
            table.Columns.Add("SourceSystem", typeof(string));

            if (profile == null)
            {
                return dataSet;
            }

            table.Rows.Add(profile.SubmittedAtUtc, "Submitted", "Order accepted from " + profile.Channel + ".", "StoreOps WCF", "StoreOps.OrderService");
            table.Rows.Add(profile.SubmittedAtUtc.AddMinutes(4), "In Kitchen", "Make line acknowledged the ticket.", "Kitchen KDS", "StoreOps.Kitchen");
            table.Rows.Add(profile.SubmittedAtUtc.AddMinutes(13), profile.ServiceMode == "Carryout" ? "Carryout Hold" : "Dispatch Queue", profile.ServiceMode == "Carryout" ? "Counter hold slip printed for pickup." : "Dispatch console staged the order for routing.", "Dispatch Console", "StoreOps.Dispatch");
            table.Rows.Add(profile.SubmittedAtUtc.AddMinutes(21), profile.OrderStatus, ResolveHistoryNote(profile), "StoreOps Service", "StoreOps.OrderService");

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

        private static string BuildPartnerId(string seedValue, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(seedValue))
            {
                return defaultValue;
            }

            var sanitized = new string(seedValue.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                return defaultValue;
            }

            if (sanitized.StartsWith("PARTNER", StringComparison.OrdinalIgnoreCase))
            {
                return sanitized.Length > 16
                    ? sanitized.Substring(0, 16)
                    : sanitized;
            }

            return "PARTNER-" + (sanitized.Length > 10
                ? sanitized.Substring(sanitized.Length - 10)
                : sanitized);
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

        private static decimal GetDecimal(StoredProcedureCall call, string name, decimal defaultValue)
        {
            var parameter = GetParameter(call, name);
            return parameter == null || parameter.Value == null || parameter.Value == DBNull.Value
                ? defaultValue
                : Convert.ToDecimal(parameter.Value);
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
        private static void AddOrderColumns(DataTable table)
        {
            table.Columns.Add("OrderNumber", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("CustomerName", typeof(string));
            table.Columns.Add("Channel", typeof(string));
            table.Columns.Add("ServiceMode", typeof(string));
            table.Columns.Add("OrderStatus", typeof(string));
            table.Columns.Add("KitchenStatus", typeof(string));
            table.Columns.Add("DispatchStatus", typeof(string));
            table.Columns.Add("PaymentStatus", typeof(string));
            table.Columns.Add("TicketTotal", typeof(decimal));
            table.Columns.Add("SubmittedAtUtc", typeof(DateTime));
            table.Columns.Add("NeededByUtc", typeof(DateTime));
            table.Columns.Add("QuotedReadyTimeUtc", typeof(DateTime));
            table.Columns.Add("FulfillmentLane", typeof(string));
            table.Columns.Add("DeliveryAddress", typeof(string));
            table.Columns.Add("SpecialInstructions", typeof(string));
            table.Columns.Add("IsCorporateAccount", typeof(bool));
            table.Columns.Add("DeliveryMileage", typeof(decimal));
            table.Columns.Add("CurrentDriverCode", typeof(string));
        }

        private static void AddOrderRow(DataTable table, OrderStubProfile profile)
        {
            table.Rows.Add(
                profile.OrderNumber,
                profile.StoreNumber,
                profile.CustomerName,
                profile.Channel,
                profile.ServiceMode,
                profile.OrderStatus,
                profile.KitchenStatus,
                profile.DispatchStatus,
                profile.PaymentStatus,
                profile.TicketTotal,
                profile.SubmittedAtUtc,
                profile.NeededByUtc,
                profile.QuotedReadyTimeUtc,
                profile.FulfillmentLane,
                profile.DeliveryAddress,
                profile.SpecialInstructions,
                profile.IsCorporateAccount,
                profile.DeliveryMileage,
                profile.CurrentDriverCode);
        }

        private static IEnumerable<OrderStubProfile> BuildSearchCandidates(string storeNumber)
        {
            var normalizedStoreNumber = string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
            if (string.Equals(normalizedStoreNumber, "014", StringComparison.OrdinalIgnoreCase))
            {
                return new[]
                {
                    CreateOrderProfile(74105, normalizedStoreNumber),
                    CreateOrderProfile(74106, normalizedStoreNumber),
                    CreateOrderProfile(71510, normalizedStoreNumber),
                    CreateOrderProfile(71518, normalizedStoreNumber)
                };
            }

            var storeSeed = ResolveStoreSeed(normalizedStoreNumber);
            return new[]
            {
                CreateOrderProfile(74000 + storeSeed, normalizedStoreNumber),
                CreateOrderProfile(74010 + storeSeed, normalizedStoreNumber),
                CreateOrderProfile(74020 + storeSeed, normalizedStoreNumber)
            };
        }

        private static OrderStubProfile CreateOrderProfile(int orderNumber, string requestedStoreNumber)
        {
            if (orderNumber <= 0)
            {
                return null;
            }

            var storeNumber = string.IsNullOrWhiteSpace(requestedStoreNumber)
                ? ResolveStoreNumber(orderNumber)
                : requestedStoreNumber.Trim().ToUpperInvariant();
            var submittedAtUtc = DateTime.UtcNow.Date.AddHours(16).AddMinutes(orderNumber % 23);

            switch (orderNumber)
            {
                case 74105:
                    return new OrderStubProfile
                    {
                        OrderNumber = orderNumber,
                        StoreNumber = storeNumber,
                        CustomerName = "North Corridor Office",
                        Channel = "Web",
                        ServiceMode = "Delivery",
                        OrderStatus = "In Kitchen",
                        KitchenStatus = "Make Line",
                        DispatchStatus = "Staged",
                        PaymentStatus = "Card Hold",
                        TicketTotal = 27.50m,
                        SubmittedAtUtc = submittedAtUtc,
                        NeededByUtc = submittedAtUtc.AddMinutes(24),
                        QuotedReadyTimeUtc = submittedAtUtc.AddMinutes(22),
                        FulfillmentLane = "StandardMakeLine",
                        DeliveryAddress = "450 108th Ave NE, Suite 620",
                        SpecialInstructions = "Leave at reception if the conference room is still locked.",
                        IsCorporateAccount = false,
                        DeliveryMileage = 4.2m,
                        CurrentDriverCode = string.Empty
                    };
                case 74106:
                    return new OrderStubProfile
                    {
                        OrderNumber = orderNumber,
                        StoreNumber = storeNumber,
                        CustomerName = "Corporate Lunch Desk",
                        Channel = "Call Center",
                        ServiceMode = "Delivery",
                        OrderStatus = "Ready",
                        KitchenStatus = "Ready",
                        DispatchStatus = "Dispatch Queue",
                        PaymentStatus = "Settled",
                        TicketTotal = 39.75m,
                        SubmittedAtUtc = submittedAtUtc.AddMinutes(-8),
                        NeededByUtc = submittedAtUtc.AddMinutes(28),
                        QuotedReadyTimeUtc = submittedAtUtc.AddMinutes(18),
                        FulfillmentLane = "CorporateAccountDesk",
                        DeliveryAddress = "1 Microsoft Way, Building 34 Lobby",
                        SpecialInstructions = "Corporate invoice already approved by campus desk.",
                        IsCorporateAccount = true,
                        DeliveryMileage = 6.8m,
                        CurrentDriverCode = "DRV-17"
                    };
                case 71510:
                    return new OrderStubProfile
                    {
                        OrderNumber = orderNumber,
                        StoreNumber = storeNumber,
                        CustomerName = "Lobby Pickup - Harris",
                        Channel = "Phone",
                        ServiceMode = "Carryout",
                        OrderStatus = "Completed",
                        KitchenStatus = "Closed",
                        DispatchStatus = "Picked Up",
                        PaymentStatus = "Settled",
                        TicketTotal = 18.75m,
                        SubmittedAtUtc = submittedAtUtc.AddMinutes(-20),
                        NeededByUtc = submittedAtUtc.AddMinutes(10),
                        QuotedReadyTimeUtc = submittedAtUtc.AddMinutes(12),
                        FulfillmentLane = "StandardMakeLine",
                        DeliveryAddress = "Front counter pickup",
                        SpecialInstructions = "Customer asked for extra napkins in the carryout bag.",
                        IsCorporateAccount = false,
                        DeliveryMileage = 0m,
                        CurrentDriverCode = string.Empty
                    };
                case 71518:
                    return new OrderStubProfile
                    {
                        OrderNumber = orderNumber,
                        StoreNumber = storeNumber,
                        CustomerName = "School Night Bundle",
                        Channel = "POS",
                        ServiceMode = "Carryout",
                        OrderStatus = "Exception",
                        KitchenStatus = "Exception",
                        DispatchStatus = "Carryout Hold",
                        PaymentStatus = "Cash Pending",
                        TicketTotal = 32.00m,
                        SubmittedAtUtc = submittedAtUtc.AddMinutes(-14),
                        NeededByUtc = submittedAtUtc.AddMinutes(16),
                        QuotedReadyTimeUtc = submittedAtUtc.AddMinutes(15),
                        FulfillmentLane = "StandardMakeLine",
                        DeliveryAddress = "Front counter pickup",
                        SpecialInstructions = "Verify cash receipt before handing over two-liter add-on.",
                        IsCorporateAccount = false,
                        DeliveryMileage = 0m,
                        CurrentDriverCode = string.Empty
                    };
                default:
                    var corporateAccount = orderNumber % 2 == 0;
                    var deliveryOrder = orderNumber % 3 != 0;
                    var deliveryMileage = deliveryOrder ? 3.5m + (orderNumber % 4) : 0m;
                    return new OrderStubProfile
                    {
                        OrderNumber = orderNumber,
                        StoreNumber = storeNumber,
                        CustomerName = corporateAccount ? "Campus Account Desk" : "Walk-Up Guest",
                        Channel = corporateAccount ? "Call Center" : "Web",
                        ServiceMode = deliveryOrder ? "Delivery" : "Carryout",
                        OrderStatus = corporateAccount ? "Ready" : "In Kitchen",
                        KitchenStatus = corporateAccount ? "Ready" : "Make Line",
                        DispatchStatus = deliveryOrder ? (corporateAccount ? "Dispatch Queue" : "Staged") : "Carryout Hold",
                        PaymentStatus = corporateAccount ? "Settled" : "Card Hold",
                        TicketTotal = 24.00m + (orderNumber % 5) * 4.50m,
                        SubmittedAtUtc = submittedAtUtc,
                        NeededByUtc = submittedAtUtc.AddMinutes(deliveryOrder ? 26 : 18),
                        QuotedReadyTimeUtc = submittedAtUtc.AddMinutes(deliveryOrder ? 20 : 16),
                        FulfillmentLane = corporateAccount ? "CorporateAccountDesk" : (deliveryMileage >= 7.5m ? "ExtendedRadiusDispatch" : "StandardMakeLine"),
                        DeliveryAddress = deliveryOrder ? storeNumber + " service corridor" : "Front counter pickup",
                        SpecialInstructions = corporateAccount ? "Route through the account desk before handoff." : "Legacy handheld terminal may reprint the ticket.",
                        IsCorporateAccount = corporateAccount,
                        DeliveryMileage = deliveryMileage,
                        CurrentDriverCode = deliveryOrder && corporateAccount ? "DRV-11" : string.Empty
                    };
            }
        }

        private static void ApplyStatusOverride(OrderStubProfile profile, string updatedStatus, string note, DateTime updatedAtUtc)
        {
            if (profile == null)
            {
                return;
            }

            var normalizedStatus = string.IsNullOrWhiteSpace(updatedStatus)
                ? string.Empty
                : updatedStatus.Trim().Replace(" ", string.Empty).ToUpperInvariant();

            switch (normalizedStatus)
            {
                case "READY":
                case "READYFORDISPATCH":
                    profile.OrderStatus = "Ready";
                    profile.KitchenStatus = "Ready";
                    profile.DispatchStatus = string.Equals(profile.ServiceMode, "Carryout", StringComparison.OrdinalIgnoreCase) ? "Carryout Hold" : "Dispatch Queue";
                    break;
                case "OUTFORDELIVERY":
                case "DISPATCHED":
                    profile.OrderStatus = "Out for Delivery";
                    profile.KitchenStatus = "Boxed";
                    profile.DispatchStatus = "Driver En Route";
                    profile.CurrentDriverCode = string.IsNullOrWhiteSpace(profile.CurrentDriverCode) ? "DRV-11" : profile.CurrentDriverCode;
                    break;
                case "COMPLETED":
                case "DELIVERED":
                    profile.OrderStatus = "Completed";
                    profile.KitchenStatus = "Closed";
                    profile.DispatchStatus = string.Equals(profile.ServiceMode, "Carryout", StringComparison.OrdinalIgnoreCase) ? "Picked Up" : "Delivered";
                    profile.PaymentStatus = "Settled";
                    break;
                case "CANCELLED":
                    profile.OrderStatus = "Cancelled";
                    profile.KitchenStatus = "Stopped";
                    profile.DispatchStatus = "Cancelled";
                    break;
                default:
                    profile.OrderStatus = string.IsNullOrWhiteSpace(updatedStatus) ? profile.OrderStatus : updatedStatus.Trim();
                    break;
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                profile.SpecialInstructions = note.Trim();
            }

            profile.QuotedReadyTimeUtc = updatedAtUtc > profile.QuotedReadyTimeUtc ? updatedAtUtc : profile.QuotedReadyTimeUtc;
        }

        private static string ResolveHistoryNote(OrderStubProfile profile)
        {
            if (profile == null)
            {
                return string.Empty;
            }

            switch (profile.OrderStatus)
            {
                case "Completed":
                    return "Order closed and handed off with payment settled.";
                case "Cancelled":
                    return "Order was cancelled before service handoff.";
                case "Out for Delivery":
                    return "Driver is carrying the order to the destination now.";
                case "Ready":
                    return "Order is ready and waiting on the service seam.";
                default:
                    return "Order remains active in the current StoreOps lane.";
            }
        }

        private static bool MatchesSearch(OrderStubProfile profile, string searchText)
        {
            if (profile == null || string.IsNullOrWhiteSpace(searchText))
            {
                return true;
            }

            var normalizedSearch = searchText.Trim();
            return profile.OrderNumber.ToString().Contains(normalizedSearch)
                || profile.CustomerName.IndexOf(normalizedSearch, StringComparison.OrdinalIgnoreCase) >= 0
                || profile.Channel.IndexOf(normalizedSearch, StringComparison.OrdinalIgnoreCase) >= 0
                || profile.ServiceMode.IndexOf(normalizedSearch, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool MatchesStatus(OrderStubProfile profile, string statusCode)
        {
            if (profile == null || string.IsNullOrWhiteSpace(statusCode))
            {
                return true;
            }

            return profile.OrderStatus.IndexOf(statusCode, StringComparison.OrdinalIgnoreCase) >= 0
                || profile.KitchenStatus.IndexOf(statusCode, StringComparison.OrdinalIgnoreCase) >= 0
                || profile.DispatchStatus.IndexOf(statusCode, StringComparison.OrdinalIgnoreCase) >= 0
                || profile.PaymentStatus.IndexOf(statusCode, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsClosed(OrderStubProfile profile)
        {
            return profile != null && (string.Equals(profile.OrderStatus, "Completed", StringComparison.OrdinalIgnoreCase)
                || string.Equals(profile.OrderStatus, "Cancelled", StringComparison.OrdinalIgnoreCase)
                || string.Equals(profile.DispatchStatus, "Delivered", StringComparison.OrdinalIgnoreCase)
                || string.Equals(profile.DispatchStatus, "Picked Up", StringComparison.OrdinalIgnoreCase));
        }

        private static string ResolveStoreNumber(int orderNumber)
        {
            if (orderNumber == 74105 || orderNumber == 74106 || orderNumber == 71510 || orderNumber == 71518)
            {
                return "014";
            }

            return ((orderNumber % 80) + 10).ToString("000");
        }

        private static int ResolveStoreSeed(string storeNumber)
        {
            if (string.IsNullOrWhiteSpace(storeNumber))
            {
                return 14;
            }

            int parsedStoreNumber;
            return int.TryParse(storeNumber, out parsedStoreNumber)
                ? parsedStoreNumber
                : 14;
        }

        private sealed class OrderStubProfile
        {
            public int OrderNumber { get; set; }

            public string StoreNumber { get; set; }

            public string CustomerName { get; set; }

            public string Channel { get; set; }

            public string ServiceMode { get; set; }

            public string OrderStatus { get; set; }

            public string KitchenStatus { get; set; }

            public string DispatchStatus { get; set; }

            public string PaymentStatus { get; set; }

            public decimal TicketTotal { get; set; }

            public DateTime SubmittedAtUtc { get; set; }

            public DateTime NeededByUtc { get; set; }

            public DateTime QuotedReadyTimeUtc { get; set; }

            public string FulfillmentLane { get; set; }

            public string DeliveryAddress { get; set; }

            public string SpecialInstructions { get; set; }

            public bool IsCorporateAccount { get; set; }

            public decimal DeliveryMileage { get; set; }

            public string CurrentDriverCode { get; set; }
        }
    }
}
