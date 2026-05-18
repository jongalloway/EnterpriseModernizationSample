using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Data.Gateways
{
    public class LegacyDbGateway
    {
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

        public LegacyDbGateway()
            : this(new LegacyConnectionCatalog())
        {
        }

        public LegacyDbGateway(LegacyConnectionCatalog connectionCatalog)
        {
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
        }

        public string GetConnectionName(string area)
        {
            return connectionCatalog.GetConnectionName(area);
        }

        public string GetConnectionName(LegacyDatabaseArea area)
        {
            return connectionCatalog.GetConnectionName(area);
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

        private static decimal GetDecimal(StoredProcedureCall call, string name, decimal defaultValue)
        {
            var parameter = GetParameter(call, name);
            return parameter == null || parameter.Value == null || parameter.Value == DBNull.Value
                ? defaultValue
                : Convert.ToDecimal(parameter.Value);
        }
    }
}
