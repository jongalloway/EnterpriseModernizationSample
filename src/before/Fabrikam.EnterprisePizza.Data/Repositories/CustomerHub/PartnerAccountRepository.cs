using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fabrikam.EnterprisePizza.Core.Domain.CustomerHub;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Fabrikam.EnterprisePizza.Shared.Contracts.PartnerSync;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class PartnerAccountRepository : IPartnerAccountRepository
    {
        private readonly LegacyDbGateway _dbGateway;

        public PartnerAccountRepository()
            : this(new LegacyDbGateway())
        {
        }

        public PartnerAccountRepository(LegacyDbGateway dbGateway)
        {
            _dbGateway = dbGateway ?? throw new System.ArgumentNullException(nameof(dbGateway));
        }

        public IList<string> GetPreferredPartners()
        {
            return GetPreferredPartnerSnapshots().Select(snapshot => snapshot.PartnerName).ToList();
        }

        public IList<PartnerAccountSnapshot> GetPreferredPartnerSnapshots()
        {
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.CustomerHub,
                LegacyStoredProcedures.CustomerHub.GetPreferredPartners);
            var dataSet = _dbGateway.ExecuteDataSet(call);
            var partners = new List<string>();
            var table = dataSet == null ? null : dataSet.Tables["PreferredPartners"];
            var partners = new List<PartnerAccountSnapshot>();
            var table = dataSet.Tables["PreferredPartners"];

            if (table == null)
            {
                return partners;
            }

            foreach (DataRow row in table.Rows)
            {
                partners.Add(new PartnerAccountSnapshot
                {
                    PartnerCode = row["PartnerCode"].ToString(),
                    PartnerName = row["PartnerName"].ToString(),
                    RelationshipTier = row["RelationshipTier"].ToString(),
                    PreferredStoreNumber = row["PreferredStoreNumber"].ToString(),
                    AccountCode = row["AccountCode"].ToString(),
                    AccountName = row["AccountName"].ToString()
                });
                partners.Add(Convert.ToString(row["PartnerName"]));
            }

            return partners;
        }

        public PartnerProfile GetPartner(string partnerId)
        {
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.CustomerHub,
                LegacyStoredProcedures.CustomerHub.GetPartnerProfile,
                new GatewayParameter("@PartnerId", partnerId));
            var dataSet = _dbGateway.ExecuteDataSet(call);
            return MapPartnerProfile(dataSet.Tables["PartnerProfile"]);
        }

        public PartnerProfile RegisterPartner(PartnerRegistrationRequest request)
        {
            var registration = request ?? new PartnerRegistrationRequest();
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.CustomerHub,
                LegacyStoredProcedures.CustomerHub.RegisterPartner,
                new GatewayParameter("@PartnerName", registration.PartnerName),
                new GatewayParameter("@AccountCode", registration.AccountCode),
                new GatewayParameter("@RelationshipTier", registration.RelationshipTier),
                new GatewayParameter("@PrimaryContact", registration.PrimaryContact),
                new GatewayParameter("@RequestedBy", registration.RequestedBy));
            var dataSet = _dbGateway.ExecuteDataSet(call);
            return MapPartnerProfile(dataSet.Tables["PartnerProfile"]);
        }

        public PartnerContractRecord UpdateContract(PartnerContractUpdateRequest request)
        {
            var update = request ?? new PartnerContractUpdateRequest();
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.CustomerHub,
                LegacyStoredProcedures.CustomerHub.UpdatePartnerContract,
                new GatewayParameter("@PartnerId", update.PartnerId),
                new GatewayParameter("@ContractCode", update.ContractCode),
                new GatewayParameter("@PricingPlanCode", update.PricingPlanCode),
                new GatewayParameter("@EffectiveDateUtc", update.EffectiveDateUtc),
                new GatewayParameter("@ExpirationDateUtc", update.ExpirationDateUtc),
                new GatewayParameter("@IsAutoRenew", update.IsAutoRenew));
            var dataSet = _dbGateway.ExecuteDataSet(call);
            return MapContractRecord(dataSet.Tables["PartnerContract"]);
        }

        public PartnerReferralRecord[] GetReferrals(string partnerId)
        {
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.CustomerHub,
                LegacyStoredProcedures.CustomerHub.GetPartnerReferrals,
                new GatewayParameter("@PartnerId", partnerId));
            var dataSet = _dbGateway.ExecuteDataSet(call);
            return MapReferralRecords(dataSet.Tables["PartnerReferrals"]);
        }

        public CommissionProcessingResult ProcessCommission(CommissionProcessingRequest request)
        {
            var commissionRequest = request ?? new CommissionProcessingRequest();
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.CustomerHub,
                LegacyStoredProcedures.CustomerHub.ProcessPartnerCommission,
                new GatewayParameter("@PartnerId", commissionRequest.PartnerId),
                new GatewayParameter("@SettlementBatchId", commissionRequest.SettlementBatchId),
                new GatewayParameter("@GrossSalesAmount", commissionRequest.GrossSalesAmount),
                new GatewayParameter("@CommissionRatePercent", commissionRequest.CommissionRatePercent),
                new GatewayParameter("@RequestedBy", commissionRequest.RequestedBy));
            var dataSet = _dbGateway.ExecuteDataSet(call);
            return MapCommissionResult(dataSet.Tables["CommissionProcessing"]);
        }

        public PartnerReferralRecord SubmitReferral(PartnerReferralSubmission request)
        {
            var submission = request ?? new PartnerReferralSubmission();
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.CustomerHub,
                LegacyStoredProcedures.CustomerHub.SubmitPartnerReferral,
                new GatewayParameter("@PartnerId", submission.PartnerId),
                new GatewayParameter("@ReferredAccountName", submission.ReferredAccountName),
                new GatewayParameter("@ChannelCode", submission.ChannelCode),
                new GatewayParameter("@SubmittedBy", submission.SubmittedBy));
            var dataSet = _dbGateway.ExecuteDataSet(call);
            return MapReferralRecord(dataSet.Tables["PartnerReferrals"]);
        }

        private static PartnerProfile MapPartnerProfile(DataTable table)
        {
            var row = table == null ? null : table.Rows.Cast<DataRow>().FirstOrDefault();
            if (row == null)
            {
                return new PartnerProfile();
            }

            return new PartnerProfile
            {
                PartnerId = ReadString(row, "PartnerId"),
                PartnerName = ReadString(row, "PartnerName"),
                AccountCode = ReadString(row, "AccountCode"),
                Status = ReadString(row, "Status"),
                RelationshipTier = ReadString(row, "RelationshipTier"),
                ContractCode = ReadString(row, "ContractCode"),
                PrimaryContact = ReadString(row, "PrimaryContact"),
                ActiveSinceUtc = ReadDateTime(row, "ActiveSinceUtc"),
                IsPreferred = ReadBoolean(row, "IsPreferred"),
                StatusUpdatedAtUtc = ReadDateTime(row, "StatusUpdatedAtUtc")
            };
        }

        private static PartnerContractRecord MapContractRecord(DataTable table)
        {
            var row = table == null ? null : table.Rows.Cast<DataRow>().FirstOrDefault();
            if (row == null)
            {
                return new PartnerContractRecord();
            }

            return new PartnerContractRecord
            {
                PartnerId = ReadString(row, "PartnerId"),
                ContractCode = ReadString(row, "ContractCode"),
                PricingPlanCode = ReadString(row, "PricingPlanCode"),
                EffectiveDateUtc = ReadDateTime(row, "EffectiveDateUtc"),
                ExpirationDateUtc = ReadDateTime(row, "ExpirationDateUtc"),
                IsAutoRenew = ReadBoolean(row, "IsAutoRenew")
            };
        }

        private static PartnerReferralRecord[] MapReferralRecords(DataTable table)
        {
            if (table == null)
            {
                return Array.Empty<PartnerReferralRecord>();
            }

            return table.Rows
                .Cast<DataRow>()
                .Select(row => new PartnerReferralRecord
                {
                    ReferralId = ReadString(row, "ReferralId"),
                    PartnerId = ReadString(row, "PartnerId"),
                    ReferredAccountName = ReadString(row, "ReferredAccountName"),
                    ReferralChannel = ReadString(row, "ReferralChannel"),
                    ReferralStatus = ReadString(row, "ReferralStatus"),
                    SubmittedBy = ReadString(row, "SubmittedBy"),
                    SubmittedAtUtc = ReadDateTime(row, "SubmittedAtUtc")
                })
                .ToArray();
        }

        private static PartnerReferralRecord MapReferralRecord(DataTable table)
        {
            return MapReferralRecords(table).FirstOrDefault() ?? new PartnerReferralRecord();
        }

        private static CommissionProcessingResult MapCommissionResult(DataTable table)
        {
            var row = table == null ? null : table.Rows.Cast<DataRow>().FirstOrDefault();
            if (row == null)
            {
                return new CommissionProcessingResult();
            }

            return new CommissionProcessingResult
            {
                PartnerId = ReadString(row, "PartnerId"),
                SettlementBatchId = ReadString(row, "SettlementBatchId"),
                CommissionAmount = ReadDecimal(row, "CommissionAmount"),
                Status = ReadString(row, "Status"),
                ProcessedAtUtc = ReadDateTime(row, "ProcessedAtUtc"),
                ProcessedBy = ReadString(row, "ProcessedBy")
            };
        }

        private static string ReadString(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToString(row[columnName])
                : string.Empty;
        }

        private static DateTime ReadDateTime(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToDateTime(row[columnName])
                : DateTime.MinValue;
        }

        private static bool ReadBoolean(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value && Convert.ToBoolean(row[columnName]);
        }

        private static decimal ReadDecimal(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToDecimal(row[columnName])
                : 0m;
        }
    }
}
