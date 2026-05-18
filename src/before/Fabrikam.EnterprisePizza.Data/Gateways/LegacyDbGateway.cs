using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Data.Gateways
{
    public class LegacyDbGateway
    {
        private static readonly DateTime SampleTimestampUtc = new DateTime(2026, 5, 18, 8, 39, 47, 894, DateTimeKind.Utc);
        private readonly LegacyConnectionCatalog connectionCatalog;
        private readonly LegacyDatabaseFactory databaseFactory;

        public LegacyDbGateway()
            : this(new LegacyConnectionCatalog(), new LegacyDatabaseFactory())
        {
        }

        public LegacyDbGateway(LegacyConnectionCatalog connectionCatalog)
            : this(connectionCatalog, new LegacyDatabaseFactory())
        {
        }

        public LegacyDbGateway(LegacyConnectionCatalog connectionCatalog, LegacyDatabaseFactory databaseFactory)
        {
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
            this.databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
        }

        public string GetConnectionName(string area)
        {
            return connectionCatalog.GetConnectionName(area);
        }

        public string GetConnectionName(LegacyDatabaseArea area)
        {
            return databaseFactory.CreateDatabase(area).ConnectionName;
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

            switch (call.ProcedureName)
            {
                case LegacyStoredProcedures.StoreOps.GetActiveDispatchTickets:
                    return BuildDispatchTickets(call);
                case LegacyStoredProcedures.StoreOps.GetLatestPosImportBatch:
                    return BuildLatestPosImportBatch(call);
                case LegacyStoredProcedures.CustomerHub.GetPreferredPartners:
                    return BuildPreferredPartners(call);
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
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("AccountCode", typeof(string));
            table.Columns.Add("RelationshipTier", typeof(string));

            table.Rows.Add("Contoso Office Parks", "CORP-1002", "Gold");
            table.Rows.Add("Northwind Youth Sports League", "COMM-8821", "Community");
            table.Rows.Add("Adventure Works Bike Expo", "EVT-4405", "Seasonal");

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
    }
}
