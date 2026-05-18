using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Fabrikam.EnterprisePizza.Data.Gateways
{
    public class LegacyDbGateway
    {
        private static readonly DateTime SampleTimestampUtc = new DateTime(2026, 5, 18, 8, 39, 47, 894, DateTimeKind.Utc);
        private static readonly DateTime CustomerHubSeedTimestamp = new DateTime(2026, 5, 18, 1, 39, 47, 894, DateTimeKind.Unspecified);
        private static readonly PartnerRecord[] PartnerStubRows =
        {
            new PartnerRecord { PartnerAccountId = 1, PartnerCode = "CORP-1002", PartnerName = "Contoso Office Parks", RelationshipTier = "Gold", PreferredStoreNumber = "014", StatusCode = "Active", LastContractRenewalDate = new DateTime(2025, 10, 20), CreditHold = false },
            new PartnerRecord { PartnerAccountId = 2, PartnerCode = "COMM-8821", PartnerName = "Northwind Youth Sports League", RelationshipTier = "Community", PreferredStoreNumber = "014", StatusCode = "Active", LastContractRenewalDate = new DateTime(2025, 12, 9), CreditHold = false },
            new PartnerRecord { PartnerAccountId = 3, PartnerCode = "EVT-4405", PartnerName = "Adventure Works Bike Expo", RelationshipTier = "Seasonal", PreferredStoreNumber = "022", StatusCode = "Active", LastContractRenewalDate = new DateTime(2026, 1, 18), CreditHold = false },
            new PartnerRecord { PartnerAccountId = 4, PartnerCode = "PCHAIN-1001", PartnerName = "Blue Yonder Pizza Express", RelationshipTier = "Platinum", PreferredStoreNumber = "031", StatusCode = "Active", LastContractRenewalDate = new DateTime(2026, 2, 14), CreditHold = false },
            new PartnerRecord { PartnerAccountId = 5, PartnerCode = "CATER-2400", PartnerName = "Fourth Coffee Catering Collective", RelationshipTier = "Gold", PreferredStoreNumber = "014", StatusCode = "Active", LastContractRenewalDate = new DateTime(2025, 11, 30), CreditHold = false },
            new PartnerRecord { PartnerAccountId = 6, PartnerCode = "CORP-3300", PartnerName = "Litware Corporate Dining", RelationshipTier = "Silver", PreferredStoreNumber = "022", StatusCode = "Active", LastContractRenewalDate = new DateTime(2025, 9, 15), CreditHold = false }
        };

        private static readonly AccountRecord[] AccountStubRows =
        {
            new AccountRecord { CorporateAccountId = 1, AccountCode = "CAT-0140", AccountName = "Fabrikam Regional Catering Desk", PreferredPartnerAccountId = 1, BillingFrequency = "Monthly", ActiveFlag = true, AccountTier = "Gold", ExternalAccountCode = "EXT-CAT-0140", AccountManagerName = "Kelly Vargas", StatusCode = "Active" },
            new AccountRecord { CorporateAccountId = 2, AccountCode = "LEAGUE-8821", AccountName = "Northwind League Concessions", PreferredPartnerAccountId = 2, BillingFrequency = "PerEvent", ActiveFlag = true, AccountTier = "Silver", ExternalAccountCode = "EXT-LEAGUE-8821", AccountManagerName = "Marcus Hale", StatusCode = "Active" },
            new AccountRecord { CorporateAccountId = 3, AccountCode = "CHAIN-1001", AccountName = "Blue Yonder Travel Plazas", PreferredPartnerAccountId = 4, BillingFrequency = "Weekly", ActiveFlag = true, AccountTier = "Platinum", ExternalAccountCode = "EXT-CHAIN-1001", AccountManagerName = "Daniela Cross", StatusCode = "Active" },
            new AccountRecord { CorporateAccountId = 4, AccountCode = "CAT-2400", AccountName = "Fourth Coffee Event Catering", PreferredPartnerAccountId = 5, BillingFrequency = "Monthly", ActiveFlag = true, AccountTier = "Gold", ExternalAccountCode = "EXT-CAT-2400", AccountManagerName = "Samira Khan", StatusCode = "Active" },
            new AccountRecord { CorporateAccountId = 5, AccountCode = "B2B-3300", AccountName = "Litware Campus Dining Program", PreferredPartnerAccountId = 6, BillingFrequency = "Quarterly", ActiveFlag = true, AccountTier = "Silver", ExternalAccountCode = "EXT-B2B-3300", AccountManagerName = "Malcolm Ives", StatusCode = "Active" }
        };

        private static readonly ContractRecord[] ContractStubRows =
        {
            new ContractRecord { PartnerContractId = 1, ContractCode = "CT-014-CAT", PartnerAccountId = 1, CorporateAccountId = 1, FranchiseLocationId = 1, ContractType = "CorporateCatering", PricingScheduleName = "Northwest Gold Catering", ReferralChannel = "FieldSales", EffectiveDate = new DateTime(2025, 10, 20), ExpirationDate = new DateTime(2026, 10, 20), MinimumOrderAmount = 150.00m, DiscountPercentage = 8.50m, CateringLeadHours = 6, StatusCode = "Active", LastReviewedUtc = new DateTime(2026, 4, 1, 9, 0, 0) },
            new ContractRecord { PartnerContractId = 2, ContractCode = "CT-014-LEAGUE", PartnerAccountId = 2, CorporateAccountId = 2, FranchiseLocationId = 1, ContractType = "CommunityProgram", PricingScheduleName = "League Concessions", ReferralChannel = "FranchiseReferral", EffectiveDate = new DateTime(2025, 12, 9), ExpirationDate = new DateTime(2026, 8, 21), MinimumOrderAmount = 75.00m, DiscountPercentage = 5.00m, CateringLeadHours = 12, StatusCode = "Active", LastReviewedUtc = new DateTime(2026, 4, 10, 14, 30, 0) },
            new ContractRecord { PartnerContractId = 3, ContractCode = "CT-022-EVT", PartnerAccountId = 3, CorporateAccountId = null, FranchiseLocationId = 2, ContractType = "EventSupport", PricingScheduleName = "Expo Seasonal Package", ReferralChannel = "EventBroker", EffectiveDate = new DateTime(2026, 1, 18), ExpirationDate = new DateTime(2026, 7, 2), MinimumOrderAmount = 300.00m, DiscountPercentage = 4.00m, CateringLeadHours = 24, StatusCode = "Active", LastReviewedUtc = new DateTime(2026, 5, 1, 11, 0, 0) },
            new ContractRecord { PartnerContractId = 4, ContractCode = "CT-1001-2026", PartnerAccountId = 4, CorporateAccountId = 3, FranchiseLocationId = 3, ContractType = "ChainSupply", PricingScheduleName = "Airport Plaza Master Pricing", ReferralChannel = "ChannelManager", EffectiveDate = new DateTime(2026, 1, 1), ExpirationDate = new DateTime(2026, 12, 31), MinimumOrderAmount = 425.00m, DiscountPercentage = 10.50m, CateringLeadHours = 4, StatusCode = "Active", LastReviewedUtc = new DateTime(2026, 5, 14, 9, 30, 0) },
            new ContractRecord { PartnerContractId = 5, ContractCode = "CT-2400-2026", PartnerAccountId = 5, CorporateAccountId = 4, FranchiseLocationId = 1, ContractType = "CateringProgram", PricingScheduleName = "Executive Catering Launch", ReferralChannel = "FieldSales", EffectiveDate = new DateTime(2026, 6, 1), ExpirationDate = new DateTime(2027, 5, 31), MinimumOrderAmount = 250.00m, DiscountPercentage = 7.75m, CateringLeadHours = 8, StatusCode = "PendingApproval", LastReviewedUtc = CustomerHubSeedTimestamp },
            new ContractRecord { PartnerContractId = 6, ContractCode = "CT-3300-2025", PartnerAccountId = 6, CorporateAccountId = 5, FranchiseLocationId = 2, ContractType = "CorporateDining", PricingScheduleName = "Campus Bronze Plan", ReferralChannel = "PartnerReferral", EffectiveDate = new DateTime(2025, 9, 1), ExpirationDate = new DateTime(2026, 8, 31), MinimumOrderAmount = 180.00m, DiscountPercentage = 4.25m, CateringLeadHours = 12, StatusCode = "Suspended", LastReviewedUtc = new DateTime(2026, 5, 3, 16, 15, 0) },
            new ContractRecord { PartnerContractId = 7, ContractCode = "CT-3300-2024", PartnerAccountId = 6, CorporateAccountId = 5, FranchiseLocationId = 2, ContractType = "CorporateDining", PricingScheduleName = "Campus Bronze Legacy", ReferralChannel = "PartnerReferral", EffectiveDate = new DateTime(2024, 9, 1), ExpirationDate = new DateTime(2025, 8, 31), MinimumOrderAmount = 165.00m, DiscountPercentage = 3.75m, CateringLeadHours = 12, StatusCode = "Expired", LastReviewedUtc = new DateTime(2025, 8, 31, 23, 59, 0) }
        };

        private static readonly ReferralRecord[] ReferralStubRows =
        {
            new ReferralRecord { PartnerReferralId = 1, ReferralCode = "REF-1001-ALPHA", PartnerAccountId = 4, CorporateAccountId = 3, ReferralChannel = "ChannelManager", ReferrerName = "Daniela Cross", AttributionCode = "ATR-ALPHA", ReferredOn = new DateTime(2026, 1, 12), StatusCode = "Converted", Notes = "Airport plaza launch lead converted to annual pricing." },
            new ReferralRecord { PartnerReferralId = 2, ReferralCode = "REF-2400-BETA", PartnerAccountId = 5, CorporateAccountId = 4, ReferralChannel = "FieldSales", ReferrerName = "Samira Khan", AttributionCode = "ATR-BETA", ReferredOn = new DateTime(2026, 4, 2), StatusCode = "PendingCommission", Notes = "Executive lunch program awaiting first quarterly commission run." },
            new ReferralRecord { PartnerReferralId = 3, ReferralCode = "REF-3300-GAMMA", PartnerAccountId = 6, CorporateAccountId = 5, ReferralChannel = "PartnerReferral", ReferrerName = "Malcolm Ives", AttributionCode = "ATR-GAMMA", ReferredOn = new DateTime(2025, 10, 10), StatusCode = "AtRisk", Notes = "Campus dining expansion is paused while the suspended contract is reviewed." }
        };

        private static readonly CommissionHistoryRecord[] CommissionStubRows =
        {
            new CommissionHistoryRecord { ReferralCommissionHistoryId = 1, PartnerReferralId = 1, CommissionPeriodStart = new DateTime(2026, 2, 1), CommissionPeriodEnd = new DateTime(2026, 2, 28), CommissionAmount = 1825.00m, CommissionStatus = "Paid", PaidUtc = new DateTime(2026, 3, 15, 10, 0, 0) },
            new CommissionHistoryRecord { ReferralCommissionHistoryId = 2, PartnerReferralId = 1, CommissionPeriodStart = new DateTime(2026, 3, 1), CommissionPeriodEnd = new DateTime(2026, 3, 31), CommissionAmount = 1995.00m, CommissionStatus = "Paid", PaidUtc = new DateTime(2026, 4, 15, 10, 0, 0) },
            new CommissionHistoryRecord { ReferralCommissionHistoryId = 3, PartnerReferralId = 2, CommissionPeriodStart = new DateTime(2026, 4, 1), CommissionPeriodEnd = new DateTime(2026, 4, 30), CommissionAmount = 640.00m, CommissionStatus = "Accrued", PaidUtc = null },
            new CommissionHistoryRecord { ReferralCommissionHistoryId = 4, PartnerReferralId = 3, CommissionPeriodStart = new DateTime(2026, 1, 1), CommissionPeriodEnd = new DateTime(2026, 1, 31), CommissionAmount = 275.00m, CommissionStatus = "OnHold", PaidUtc = null }
        };

        private readonly LegacyConnectionCatalog connectionCatalog;
        private readonly LegacyEnterpriseLibraryDatabaseFactory enterpriseLibraryDatabaseFactory;
        private readonly IDictionary<string, Database> configuredDatabases;
        private readonly LegacyDatabaseFactory databaseFactory;

        public LegacyDbGateway()
            : this(new LegacyConnectionCatalog(), new LegacyEnterpriseLibraryDatabaseFactory(), new LegacyDatabaseFactory())
        {
        }

        public LegacyDbGateway(LegacyConnectionCatalog connectionCatalog)
            : this(connectionCatalog, new LegacyEnterpriseLibraryDatabaseFactory(connectionCatalog, new DatabaseProviderFactory()), new LegacyDatabaseFactory(connectionCatalog))
        {
        }

        public LegacyDbGateway(LegacyConnectionCatalog connectionCatalog, LegacyEnterpriseLibraryDatabaseFactory enterpriseLibraryDatabaseFactory)
            : this(connectionCatalog, enterpriseLibraryDatabaseFactory, new LegacyDatabaseFactory(connectionCatalog))
        {
        }

        public LegacyDbGateway(LegacyConnectionCatalog connectionCatalog, LegacyDatabaseFactory databaseFactory)
            : this(connectionCatalog, new LegacyEnterpriseLibraryDatabaseFactory(connectionCatalog, new DatabaseProviderFactory()), databaseFactory)
        {
        }

        private LegacyDbGateway(LegacyConnectionCatalog connectionCatalog, LegacyEnterpriseLibraryDatabaseFactory enterpriseLibraryDatabaseFactory, LegacyDatabaseFactory databaseFactory)
        {
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
            this.enterpriseLibraryDatabaseFactory = enterpriseLibraryDatabaseFactory ?? throw new ArgumentNullException(nameof(enterpriseLibraryDatabaseFactory));
            this.databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
            configuredDatabases = new Dictionary<string, Database>(StringComparer.OrdinalIgnoreCase);
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
                configuredDatabases[connectionName] = enterpriseLibraryDatabaseFactory.Create(connectionName);
            }

            return configuredDatabases[connectionName];
        }

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
                case LegacyStoredProcedures.CustomerHub.GetPartners:
                    return BuildPartners(call);
                case LegacyStoredProcedures.CustomerHub.GetPartnerByCode:
                    return BuildPartnerByCode(call);
                case LegacyStoredProcedures.CustomerHub.SavePartner:
                    return BuildSavePartner(call);
                case LegacyStoredProcedures.CustomerHub.DeletePartner:
                    return BuildDeletePartner(call);
                case LegacyStoredProcedures.CustomerHub.GetContractByCode:
                    return BuildContractByCode(call);
                case LegacyStoredProcedures.CustomerHub.GetContractsByStatus:
                    return BuildContractsByStatus(call);
                case LegacyStoredProcedures.CustomerHub.SaveContract:
                    return BuildSaveContract(call);
                case LegacyStoredProcedures.CustomerHub.UpdateContractStatus:
                    return BuildUpdateContractStatus(call);
                case LegacyStoredProcedures.CustomerHub.GetAccountByCode:
                    return BuildAccountByCode(call);
                case LegacyStoredProcedures.CustomerHub.GetAccountsByTier:
                    return BuildAccountsByTier(call);
                case LegacyStoredProcedures.CustomerHub.SaveAccount:
                    return BuildSaveAccount(call);
                case LegacyStoredProcedures.CustomerHub.DeactivateAccount:
                    return BuildDeactivateAccount(call);
                case LegacyStoredProcedures.CustomerHub.GetReferralsByPartnerCode:
                    return BuildReferralsByPartnerCode(call);
                case LegacyStoredProcedures.CustomerHub.SaveReferral:
                    return BuildSaveReferral(call);
                case LegacyStoredProcedures.CustomerHub.GetReferralCommissionHistory:
                    return BuildReferralCommissionHistory(call);
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

        private static DataSet BuildPartners(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("Partners");
            var table = CreatePartnerTable("Partners");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);

            foreach (var partner in PartnerStubRows.Where(row => !string.Equals(row.StatusCode, "Inactive", StringComparison.OrdinalIgnoreCase)))
            {
                AddPartnerRow(table, partner);
            }

            return dataSet;
        }

        private static DataSet BuildPartnerByCode(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("Partner");
            var table = CreatePartnerTable("Partner");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);

            var partner = FindPartnerByCode(GetString(call, "@PartnerCode", null));
            if (partner != null)
            {
                AddPartnerRow(table, partner);
            }

            return dataSet;
        }

        private static DataSet BuildSavePartner(StoredProcedureCall call)
        {
            var existing = FindPartnerByCode(GetString(call, "@PartnerCode", null));
            var partner = new PartnerRecord
            {
                PartnerAccountId = GetInt32(call, "@PartnerAccountId", existing == null ? 900 : existing.PartnerAccountId),
                PartnerCode = GetString(call, "@PartnerCode", existing == null ? "PARTNER-NEW" : existing.PartnerCode),
                PartnerName = GetString(call, "@PartnerName", existing == null ? "New CustomerHub Partner" : existing.PartnerName),
                RelationshipTier = GetString(call, "@RelationshipTier", existing == null ? "Silver" : existing.RelationshipTier),
                PreferredStoreNumber = GetString(call, "@PreferredStoreNumber", existing == null ? "014" : existing.PreferredStoreNumber),
                StatusCode = GetString(call, "@StatusCode", existing == null ? "Active" : existing.StatusCode),
                LastContractRenewalDate = GetNullableDateTime(call, "@LastContractRenewalDate", existing == null ? (DateTime?)CustomerHubSeedTimestamp.Date : existing.LastContractRenewalDate),
                CreditHold = GetBoolean(call, "@CreditHold", existing != null && existing.CreditHold)
            };

            var dataSet = CreateDataSet("Partner");
            var table = CreatePartnerTable("Partner");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);
            AddPartnerRow(table, partner);
            return dataSet;
        }

        private static DataSet BuildDeletePartner(StoredProcedureCall call)
        {
            var deleted = FindPartnerByCode(GetString(call, "@PartnerCode", null)) != null;
            var dataSet = CreateDataSet("PartnerDeleteResult");
            var table = dataSet.Tables[0];
            table.Columns.Add("Deleted", typeof(bool));
            table.Rows.Add(deleted);
            return dataSet;
        }

        private static DataSet BuildContractByCode(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("Contract");
            var table = CreateContractTable("Contract");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);

            var contract = FindContractByCode(GetString(call, "@ContractCode", null));
            if (contract != null)
            {
                AddContractRow(table, contract);
            }

            return dataSet;
        }

        private static DataSet BuildContractsByStatus(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("Contracts");
            var table = CreateContractTable("Contracts");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);
            var statusCode = GetString(call, "@StatusCode", null);
            var rows = string.IsNullOrWhiteSpace(statusCode)
                ? ContractStubRows
                : ContractStubRows.Where(row => string.Equals(row.StatusCode, statusCode, StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (var contract in rows)
            {
                AddContractRow(table, contract);
            }

            return dataSet;
        }

        private static DataSet BuildSaveContract(StoredProcedureCall call)
        {
            var existing = FindContractByCode(GetString(call, "@ContractCode", null));
            var contract = new ContractRecord
            {
                PartnerContractId = GetInt32(call, "@PartnerContractId", existing == null ? 950 : existing.PartnerContractId),
                ContractCode = GetString(call, "@ContractCode", existing == null ? "CT-NEW-2026" : existing.ContractCode),
                PartnerAccountId = GetInt32(call, "@PartnerAccountId", existing == null ? 4 : existing.PartnerAccountId),
                CorporateAccountId = GetNullableInt32(call, "@CorporateAccountId", existing == null ? (int?)3 : existing.CorporateAccountId),
                FranchiseLocationId = GetNullableInt32(call, "@FranchiseLocationId", existing == null ? (int?)3 : existing.FranchiseLocationId),
                ContractType = GetString(call, "@ContractType", existing == null ? "CorporateDining" : existing.ContractType),
                PricingScheduleName = GetString(call, "@PricingScheduleName", existing == null ? "CustomerHub Managed Pricing" : existing.PricingScheduleName),
                ReferralChannel = GetString(call, "@ReferralChannel", existing == null ? "PartnerReferral" : existing.ReferralChannel),
                EffectiveDate = GetDateTime(call, "@EffectiveDate", existing == null ? CustomerHubSeedTimestamp.Date : existing.EffectiveDate),
                ExpirationDate = GetNullableDateTime(call, "@ExpirationDate", existing == null ? (DateTime?)CustomerHubSeedTimestamp.Date.AddYears(1) : existing.ExpirationDate),
                MinimumOrderAmount = GetDecimal(call, "@MinimumOrderAmount", existing == null ? 225.00m : existing.MinimumOrderAmount),
                DiscountPercentage = GetDecimal(call, "@DiscountPercentage", existing == null ? 5.50m : existing.DiscountPercentage),
                CateringLeadHours = GetInt16(call, "@CateringLeadHours", existing == null ? (short)8 : existing.CateringLeadHours),
                StatusCode = GetString(call, "@StatusCode", existing == null ? "Active" : existing.StatusCode),
                LastReviewedUtc = GetDateTime(call, "@LastReviewedUtc", CustomerHubSeedTimestamp)
            };

            var dataSet = CreateDataSet("Contract");
            var table = CreateContractTable("Contract");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);
            AddContractRow(table, contract);
            return dataSet;
        }

        private static DataSet BuildUpdateContractStatus(StoredProcedureCall call)
        {
            var existing = FindContractByCode(GetString(call, "@ContractCode", null));
            var contract = existing == null
                ? new ContractRecord { PartnerContractId = 950, ContractCode = GetString(call, "@ContractCode", "CT-NEW-2026"), PartnerAccountId = 4, CorporateAccountId = 3, FranchiseLocationId = 3, ContractType = "CorporateDining", PricingScheduleName = "CustomerHub Managed Pricing", ReferralChannel = "PartnerReferral", EffectiveDate = CustomerHubSeedTimestamp.Date, ExpirationDate = CustomerHubSeedTimestamp.Date.AddYears(1), MinimumOrderAmount = 225.00m, DiscountPercentage = 5.50m, CateringLeadHours = 8 }
                : new ContractRecord
                {
                    PartnerContractId = existing.PartnerContractId,
                    ContractCode = existing.ContractCode,
                    PartnerAccountId = existing.PartnerAccountId,
                    CorporateAccountId = existing.CorporateAccountId,
                    FranchiseLocationId = existing.FranchiseLocationId,
                    ContractType = existing.ContractType,
                    PricingScheduleName = existing.PricingScheduleName,
                    ReferralChannel = existing.ReferralChannel,
                    EffectiveDate = existing.EffectiveDate,
                    ExpirationDate = existing.ExpirationDate,
                    MinimumOrderAmount = existing.MinimumOrderAmount,
                    DiscountPercentage = existing.DiscountPercentage,
                    CateringLeadHours = existing.CateringLeadHours
                };

            contract.StatusCode = GetString(call, "@StatusCode", existing == null ? "Active" : existing.StatusCode);
            contract.LastReviewedUtc = CustomerHubSeedTimestamp;

            var dataSet = CreateDataSet("Contract");
            var table = CreateContractTable("Contract");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);
            AddContractRow(table, contract);
            return dataSet;
        }

        private static DataSet BuildAccountByCode(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("Account");
            var table = CreateAccountTable("Account");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);

            var account = FindAccountByCode(GetString(call, "@AccountCode", null));
            if (account != null)
            {
                AddAccountRow(table, account);
            }

            return dataSet;
        }

        private static DataSet BuildAccountsByTier(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("Accounts");
            var table = CreateAccountTable("Accounts");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);
            var accountTier = GetString(call, "@AccountTier", null);
            var rows = string.IsNullOrWhiteSpace(accountTier)
                ? AccountStubRows
                : AccountStubRows.Where(row => string.Equals(row.AccountTier, accountTier, StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (var account in rows)
            {
                AddAccountRow(table, account);
            }

            return dataSet;
        }

        private static DataSet BuildSaveAccount(StoredProcedureCall call)
        {
            var existing = FindAccountByCode(GetString(call, "@AccountCode", null));
            var account = new AccountRecord
            {
                CorporateAccountId = GetInt32(call, "@CorporateAccountId", existing == null ? 975 : existing.CorporateAccountId),
                AccountCode = GetString(call, "@AccountCode", existing == null ? "ACCOUNT-NEW" : existing.AccountCode),
                AccountName = GetString(call, "@AccountName", existing == null ? "New CustomerHub Account" : existing.AccountName),
                PreferredPartnerAccountId = GetNullableInt32(call, "@PreferredPartnerAccountId", existing == null ? (int?)4 : existing.PreferredPartnerAccountId),
                BillingFrequency = GetString(call, "@BillingFrequency", existing == null ? "Monthly" : existing.BillingFrequency),
                ActiveFlag = GetBoolean(call, "@ActiveFlag", existing == null || existing.ActiveFlag),
                AccountTier = GetString(call, "@AccountTier", existing == null ? "Silver" : existing.AccountTier),
                ExternalAccountCode = GetString(call, "@ExternalAccountCode", existing == null ? "EXT-ACCOUNT-NEW" : existing.ExternalAccountCode),
                AccountManagerName = GetString(call, "@AccountManagerName", existing == null ? "CustomerHub Desk" : existing.AccountManagerName),
                StatusCode = GetString(call, "@StatusCode", existing == null ? "Active" : existing.StatusCode)
            };

            var dataSet = CreateDataSet("Account");
            var table = CreateAccountTable("Account");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);
            AddAccountRow(table, account);
            return dataSet;
        }

        private static DataSet BuildDeactivateAccount(StoredProcedureCall call)
        {
            var existing = FindAccountByCode(GetString(call, "@AccountCode", null));
            var account = existing == null
                ? new AccountRecord { CorporateAccountId = 975, AccountCode = GetString(call, "@AccountCode", "ACCOUNT-NEW"), AccountName = "New CustomerHub Account", PreferredPartnerAccountId = 4, BillingFrequency = "Monthly", AccountTier = "Silver", ExternalAccountCode = "EXT-ACCOUNT-NEW", AccountManagerName = "CustomerHub Desk" }
                : new AccountRecord
                {
                    CorporateAccountId = existing.CorporateAccountId,
                    AccountCode = existing.AccountCode,
                    AccountName = existing.AccountName,
                    PreferredPartnerAccountId = existing.PreferredPartnerAccountId,
                    BillingFrequency = existing.BillingFrequency,
                    AccountTier = existing.AccountTier,
                    ExternalAccountCode = existing.ExternalAccountCode,
                    AccountManagerName = existing.AccountManagerName
                };

            account.ActiveFlag = false;
            account.StatusCode = "Inactive";

            var dataSet = CreateDataSet("Account");
            var table = CreateAccountTable("Account");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);
            AddAccountRow(table, account);
            return dataSet;
        }

        private static DataSet BuildReferralsByPartnerCode(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("Referrals");
            var table = CreateReferralTable("Referrals");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);

            foreach (var referral in FindReferralsByPartnerCode(GetString(call, "@PartnerCode", null)))
            {
                AddReferralRow(table, referral);
            }

            return dataSet;
        }

        private static DataSet BuildSaveReferral(StoredProcedureCall call)
        {
            var partnerReferralId = GetInt32(call, "@PartnerReferralId", 0);
            var existing = ReferralStubRows.FirstOrDefault(row => row.PartnerReferralId == partnerReferralId)
                ?? ReferralStubRows.FirstOrDefault(row => string.Equals(row.ReferralCode, GetString(call, "@ReferralCode", null), StringComparison.OrdinalIgnoreCase));
            var referral = new ReferralRecord
            {
                PartnerReferralId = partnerReferralId == 0 ? (existing == null ? 990 : existing.PartnerReferralId) : partnerReferralId,
                ReferralCode = GetString(call, "@ReferralCode", existing == null ? "REF-NEW-2026" : existing.ReferralCode),
                PartnerAccountId = GetInt32(call, "@PartnerAccountId", existing == null ? 4 : existing.PartnerAccountId),
                CorporateAccountId = GetNullableInt32(call, "@CorporateAccountId", existing == null ? (int?)3 : existing.CorporateAccountId),
                ReferralChannel = GetString(call, "@ReferralChannel", existing == null ? "PartnerReferral" : existing.ReferralChannel),
                ReferrerName = GetString(call, "@ReferrerName", existing == null ? "CustomerHub Referrer" : existing.ReferrerName),
                AttributionCode = GetString(call, "@AttributionCode", existing == null ? "ATR-NEW" : existing.AttributionCode),
                ReferredOn = GetDateTime(call, "@ReferredOn", existing == null ? CustomerHubSeedTimestamp.Date : existing.ReferredOn),
                StatusCode = GetString(call, "@StatusCode", existing == null ? "PendingCommission" : existing.StatusCode),
                Notes = GetString(call, "@Notes", existing == null ? "Seeded from CustomerHub DAL stub." : existing.Notes)
            };

            var dataSet = CreateDataSet("Referral");
            var table = CreateReferralTable("Referral");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);
            AddReferralRow(table, referral);
            return dataSet;
        }

        private static DataSet BuildReferralCommissionHistory(StoredProcedureCall call)
        {
            var dataSet = CreateDataSet("CommissionHistory");
            var table = CreateCommissionHistoryTable("CommissionHistory");
            dataSet.Tables.Clear();
            dataSet.Tables.Add(table);

            foreach (var commissionHistory in FindCommissionHistoryByPartnerCode(GetString(call, "@PartnerCode", null)))
            {
                AddCommissionHistoryRow(table, commissionHistory);
            }

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
            }

            return dataSet;
        }

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

            return dataSet;
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
                ? new[] { 2.25m, 1.25m, 0.50m, 2.00m, 1.50m, 0.50m, 1.75m, 1.25m, 0.25m, 1.50m, 1.00m, 0.25m }
                : new[] { 6.00m, 3.00m, 1.50m, 5.75m, 2.75m, 1.50m, 4.50m, 2.50m, 1.25m, 5.00m, 2.00m, 1.00m };

            for (var index = 0; index < weeksBack && index < 4; index++)
            {
                var row = table.NewRow();
                var offset = index * 3;
                row["StoreNumber"] = storeNumber;
                row["WeekEndingDate"] = today.AddDays(index * -7);
                row["DriverOvertimeHours"] = seed[offset];
                row["KitchenOvertimeHours"] = seed[offset + 1];
                row["ShiftLeadOvertimeHours"] = seed[offset + 2];
                row["TotalOvertimeHours"] = seed[offset] + seed[offset + 1] + seed[offset + 2];
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

        private static DataTable CreatePartnerTable(string tableName)
        {
            var table = new DataTable(tableName);
            table.Columns.Add("PartnerAccountId", typeof(int));
            table.Columns.Add("PartnerCode", typeof(string));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("RelationshipTier", typeof(string));
            table.Columns.Add("PreferredStoreNumber", typeof(string));
            table.Columns.Add("StatusCode", typeof(string));
            table.Columns.Add("LastContractRenewalDate", typeof(DateTime));
            table.Columns.Add("CreditHold", typeof(bool));
            return table;
        }

        private static DataTable CreateAccountTable(string tableName)
        {
            var table = new DataTable(tableName);
            table.Columns.Add("CorporateAccountId", typeof(int));
            table.Columns.Add("AccountCode", typeof(string));
            table.Columns.Add("AccountName", typeof(string));
            table.Columns.Add("PreferredPartnerAccountId", typeof(int));
            table.Columns.Add("BillingFrequency", typeof(string));
            table.Columns.Add("ActiveFlag", typeof(bool));
            table.Columns.Add("AccountTier", typeof(string));
            table.Columns.Add("ExternalAccountCode", typeof(string));
            table.Columns.Add("AccountManagerName", typeof(string));
            table.Columns.Add("StatusCode", typeof(string));
            return table;
        }

        private static DataTable CreateContractTable(string tableName)
        {
            var table = new DataTable(tableName);
            table.Columns.Add("PartnerContractId", typeof(int));
            table.Columns.Add("ContractCode", typeof(string));
            table.Columns.Add("PartnerAccountId", typeof(int));
            table.Columns.Add("CorporateAccountId", typeof(int));
            table.Columns.Add("FranchiseLocationId", typeof(int));
            table.Columns.Add("ContractType", typeof(string));
            table.Columns.Add("PricingScheduleName", typeof(string));
            table.Columns.Add("ReferralChannel", typeof(string));
            table.Columns.Add("EffectiveDate", typeof(DateTime));
            table.Columns.Add("ExpirationDate", typeof(DateTime));
            table.Columns.Add("MinimumOrderAmount", typeof(decimal));
            table.Columns.Add("DiscountPercentage", typeof(decimal));
            table.Columns.Add("CateringLeadHours", typeof(short));
            table.Columns.Add("StatusCode", typeof(string));
            table.Columns.Add("LastReviewedUtc", typeof(DateTime));
            return table;
        }

        private static DataTable CreateReferralTable(string tableName)
        {
            var table = new DataTable(tableName);
            table.Columns.Add("PartnerReferralId", typeof(int));
            table.Columns.Add("ReferralCode", typeof(string));
            table.Columns.Add("PartnerAccountId", typeof(int));
            table.Columns.Add("CorporateAccountId", typeof(int));
            table.Columns.Add("ReferralChannel", typeof(string));
            table.Columns.Add("ReferrerName", typeof(string));
            table.Columns.Add("AttributionCode", typeof(string));
            table.Columns.Add("ReferredOn", typeof(DateTime));
            table.Columns.Add("StatusCode", typeof(string));
            table.Columns.Add("Notes", typeof(string));
            return table;
        }

        private static DataTable CreateCommissionHistoryTable(string tableName)
        {
            var table = new DataTable(tableName);
            table.Columns.Add("ReferralCommissionHistoryId", typeof(int));
            table.Columns.Add("PartnerReferralId", typeof(int));
            table.Columns.Add("CommissionPeriodStart", typeof(DateTime));
            table.Columns.Add("CommissionPeriodEnd", typeof(DateTime));
            table.Columns.Add("CommissionAmount", typeof(decimal));
            table.Columns.Add("CommissionStatus", typeof(string));
            table.Columns.Add("PaidUtc", typeof(DateTime));
            return table;
        }

        private static void AddPartnerRow(DataTable table, PartnerRecord partner)
        {
            var row = table.NewRow();
            row["PartnerAccountId"] = partner.PartnerAccountId;
            row["PartnerCode"] = partner.PartnerCode;
            row["PartnerName"] = partner.PartnerName;
            row["RelationshipTier"] = partner.RelationshipTier;
            row["PreferredStoreNumber"] = (object)partner.PreferredStoreNumber ?? DBNull.Value;
            row["StatusCode"] = partner.StatusCode;
            row["LastContractRenewalDate"] = partner.LastContractRenewalDate.HasValue ? (object)partner.LastContractRenewalDate.Value : DBNull.Value;
            row["CreditHold"] = partner.CreditHold;
            table.Rows.Add(row);
        }

        private static void AddAccountRow(DataTable table, AccountRecord account)
        {
            var row = table.NewRow();
            row["CorporateAccountId"] = account.CorporateAccountId;
            row["AccountCode"] = account.AccountCode;
            row["AccountName"] = account.AccountName;
            row["PreferredPartnerAccountId"] = account.PreferredPartnerAccountId.HasValue ? (object)account.PreferredPartnerAccountId.Value : DBNull.Value;
            row["BillingFrequency"] = account.BillingFrequency;
            row["ActiveFlag"] = account.ActiveFlag;
            row["AccountTier"] = account.AccountTier;
            row["ExternalAccountCode"] = (object)account.ExternalAccountCode ?? DBNull.Value;
            row["AccountManagerName"] = (object)account.AccountManagerName ?? DBNull.Value;
            row["StatusCode"] = account.StatusCode;
            table.Rows.Add(row);
        }

        private static void AddContractRow(DataTable table, ContractRecord contract)
        {
            var row = table.NewRow();
            row["PartnerContractId"] = contract.PartnerContractId;
            row["ContractCode"] = contract.ContractCode;
            row["PartnerAccountId"] = contract.PartnerAccountId;
            row["CorporateAccountId"] = contract.CorporateAccountId.HasValue ? (object)contract.CorporateAccountId.Value : DBNull.Value;
            row["FranchiseLocationId"] = contract.FranchiseLocationId.HasValue ? (object)contract.FranchiseLocationId.Value : DBNull.Value;
            row["ContractType"] = contract.ContractType;
            row["PricingScheduleName"] = contract.PricingScheduleName;
            row["ReferralChannel"] = (object)contract.ReferralChannel ?? DBNull.Value;
            row["EffectiveDate"] = contract.EffectiveDate;
            row["ExpirationDate"] = contract.ExpirationDate.HasValue ? (object)contract.ExpirationDate.Value : DBNull.Value;
            row["MinimumOrderAmount"] = contract.MinimumOrderAmount;
            row["DiscountPercentage"] = contract.DiscountPercentage;
            row["CateringLeadHours"] = contract.CateringLeadHours;
            row["StatusCode"] = contract.StatusCode;
            row["LastReviewedUtc"] = contract.LastReviewedUtc;
            table.Rows.Add(row);
        }

        private static void AddReferralRow(DataTable table, ReferralRecord referral)
        {
            var row = table.NewRow();
            row["PartnerReferralId"] = referral.PartnerReferralId;
            row["ReferralCode"] = referral.ReferralCode;
            row["PartnerAccountId"] = referral.PartnerAccountId;
            row["CorporateAccountId"] = referral.CorporateAccountId.HasValue ? (object)referral.CorporateAccountId.Value : DBNull.Value;
            row["ReferralChannel"] = referral.ReferralChannel;
            row["ReferrerName"] = referral.ReferrerName;
            row["AttributionCode"] = referral.AttributionCode;
            row["ReferredOn"] = referral.ReferredOn;
            row["StatusCode"] = referral.StatusCode;
            row["Notes"] = (object)referral.Notes ?? DBNull.Value;
            table.Rows.Add(row);
        }

        private static void AddCommissionHistoryRow(DataTable table, CommissionHistoryRecord commissionHistory)
        {
            var row = table.NewRow();
            row["ReferralCommissionHistoryId"] = commissionHistory.ReferralCommissionHistoryId;
            row["PartnerReferralId"] = commissionHistory.PartnerReferralId;
            row["CommissionPeriodStart"] = commissionHistory.CommissionPeriodStart;
            row["CommissionPeriodEnd"] = commissionHistory.CommissionPeriodEnd;
            row["CommissionAmount"] = commissionHistory.CommissionAmount;
            row["CommissionStatus"] = commissionHistory.CommissionStatus;
            row["PaidUtc"] = commissionHistory.PaidUtc.HasValue ? (object)commissionHistory.PaidUtc.Value : DBNull.Value;
            table.Rows.Add(row);
        }

        private static PartnerRecord FindPartnerByCode(string partnerCode)
        {
            return PartnerStubRows.FirstOrDefault(row => string.Equals(row.PartnerCode, partnerCode, StringComparison.OrdinalIgnoreCase));
        }

        private static string FindPartnerCodeById(int partnerAccountId)
        {
            var partner = PartnerStubRows.FirstOrDefault(row => row.PartnerAccountId == partnerAccountId);
            return partner == null ? null : partner.PartnerCode;
        }

        private static AccountRecord FindAccountByCode(string accountCode)
        {
            return AccountStubRows.FirstOrDefault(row => string.Equals(row.AccountCode, accountCode, StringComparison.OrdinalIgnoreCase));
        }

        private static ContractRecord FindContractByCode(string contractCode)
        {
            return ContractStubRows.FirstOrDefault(row => string.Equals(row.ContractCode, contractCode, StringComparison.OrdinalIgnoreCase));
        }

        private static IList<ReferralRecord> FindReferralsByPartnerCode(string partnerCode)
        {
            if (string.IsNullOrWhiteSpace(partnerCode))
            {
                return ReferralStubRows.ToList();
            }

            return ReferralStubRows
                .Where(row => string.Equals(FindPartnerCodeById(row.PartnerAccountId), partnerCode, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private static IList<CommissionHistoryRecord> FindCommissionHistoryByPartnerCode(string partnerCode)
        {
            var referrals = FindReferralsByPartnerCode(partnerCode).Select(row => row.PartnerReferralId).ToArray();
            return CommissionStubRows.Where(row => referrals.Contains(row.PartnerReferralId)).ToList();
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

        private static DateTime? GetNullableDateTime(StoredProcedureCall call, string name, DateTime? defaultValue)
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

        private static int? GetNullableInt32(StoredProcedureCall call, string name, int? defaultValue)
        {
            var parameter = GetParameter(call, name);
            return parameter == null || parameter.Value == null || parameter.Value == DBNull.Value
                ? defaultValue
                : Convert.ToInt32(parameter.Value);
        }

        private static short GetInt16(StoredProcedureCall call, string name, short defaultValue)
        {
            var parameter = GetParameter(call, name);
            return parameter == null || parameter.Value == null || parameter.Value == DBNull.Value
                ? defaultValue
                : Convert.ToInt16(parameter.Value);
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
