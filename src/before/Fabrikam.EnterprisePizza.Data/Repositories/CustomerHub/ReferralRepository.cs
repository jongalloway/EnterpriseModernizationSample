using System;
using System.Collections.Generic;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class ReferralRepository
    {
        private readonly LegacyDbGateway _dbGateway;
        private readonly CustomerHubDatabaseFactory _databaseFactory;

        public ReferralRepository()
            : this(new LegacyDbGateway(), new CustomerHubDatabaseFactory())
        {
        }

        public ReferralRepository(LegacyDbGateway dbGateway)
            : this(dbGateway, new CustomerHubDatabaseFactory())
        {
        }

        public ReferralRepository(LegacyDbGateway dbGateway, CustomerHubDatabaseFactory databaseFactory)
        {
            _dbGateway = dbGateway ?? throw new ArgumentNullException(nameof(dbGateway));
            _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
        }

        public IList<ReferralRecord> GetByPartnerCode(string partnerCode)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.GetReferralsByPartnerCode,
                new GatewayParameter("@PartnerCode", partnerCode));
            return ReadReferrals(_dbGateway.ExecuteDataSet(call), "Referrals");
        }

        public ReferralRecord Save(ReferralRecord referral)
        {
            if (referral == null)
            {
                throw new ArgumentNullException(nameof(referral));
            }

            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.SaveReferral,
                new GatewayParameter("@PartnerReferralId", referral.PartnerReferralId),
                new GatewayParameter("@ReferralCode", referral.ReferralCode),
                new GatewayParameter("@PartnerAccountId", referral.PartnerAccountId),
                new GatewayParameter("@CorporateAccountId", referral.CorporateAccountId),
                new GatewayParameter("@ReferralChannel", referral.ReferralChannel),
                new GatewayParameter("@ReferrerName", referral.ReferrerName),
                new GatewayParameter("@AttributionCode", referral.AttributionCode),
                new GatewayParameter("@ReferredOn", referral.ReferredOn),
                new GatewayParameter("@StatusCode", referral.StatusCode),
                new GatewayParameter("@Notes", referral.Notes));

            return ReadSingleReferral(_dbGateway.ExecuteDataSet(call), "Referral");
        }

        public IList<CommissionHistoryRecord> GetCommissionHistory(string partnerCode)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.GetReferralCommissionHistory,
                new GatewayParameter("@PartnerCode", partnerCode));
            return ReadCommissionHistory(_dbGateway.ExecuteDataSet(call), "CommissionHistory");
        }

        private StoredProcedureCall CreateCustomerHubCall(string procedureName, params GatewayParameter[] parameters)
        {
            var call = _dbGateway.CreateStoredProcedureCall(LegacyDatabaseArea.CustomerHub, procedureName, parameters);
            if (!string.Equals(call.ConnectionName, _databaseFactory.ConnectionName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("CustomerHub repository expected the FabrikamPizza_CustomerHub connection.");
            }

            return call;
        }

        private static IList<ReferralRecord> ReadReferrals(DataSet dataSet, string tableName)
        {
            var referrals = new List<ReferralRecord>();
            var table = dataSet.Tables[tableName];
            if (table == null)
            {
                return referrals;
            }

            foreach (DataRow row in table.Rows)
            {
                referrals.Add(MapReferral(row));
            }

            return referrals;
        }

        private static ReferralRecord ReadSingleReferral(DataSet dataSet, string tableName)
        {
            var table = dataSet.Tables[tableName];
            return table == null || table.Rows.Count == 0
                ? null
                : MapReferral(table.Rows[0]);
        }

        private static IList<CommissionHistoryRecord> ReadCommissionHistory(DataSet dataSet, string tableName)
        {
            var commissionHistory = new List<CommissionHistoryRecord>();
            var table = dataSet.Tables[tableName];
            if (table == null)
            {
                return commissionHistory;
            }

            foreach (DataRow row in table.Rows)
            {
                commissionHistory.Add(new CommissionHistoryRecord
                {
                    ReferralCommissionHistoryId = CustomerHubDataRecordReader.GetInt32(row, "ReferralCommissionHistoryId"),
                    PartnerReferralId = CustomerHubDataRecordReader.GetInt32(row, "PartnerReferralId"),
                    CommissionPeriodStart = CustomerHubDataRecordReader.GetDateTime(row, "CommissionPeriodStart"),
                    CommissionPeriodEnd = CustomerHubDataRecordReader.GetDateTime(row, "CommissionPeriodEnd"),
                    CommissionAmount = CustomerHubDataRecordReader.GetDecimal(row, "CommissionAmount"),
                    CommissionStatus = CustomerHubDataRecordReader.GetString(row, "CommissionStatus"),
                    PaidUtc = CustomerHubDataRecordReader.GetNullableDateTime(row, "PaidUtc")
                });
            }

            return commissionHistory;
        }

        private static ReferralRecord MapReferral(DataRow row)
        {
            return new ReferralRecord
            {
                PartnerReferralId = CustomerHubDataRecordReader.GetInt32(row, "PartnerReferralId"),
                ReferralCode = CustomerHubDataRecordReader.GetString(row, "ReferralCode"),
                PartnerAccountId = CustomerHubDataRecordReader.GetInt32(row, "PartnerAccountId"),
                CorporateAccountId = CustomerHubDataRecordReader.GetNullableInt32(row, "CorporateAccountId"),
                ReferralChannel = CustomerHubDataRecordReader.GetString(row, "ReferralChannel"),
                ReferrerName = CustomerHubDataRecordReader.GetString(row, "ReferrerName"),
                AttributionCode = CustomerHubDataRecordReader.GetString(row, "AttributionCode"),
                ReferredOn = CustomerHubDataRecordReader.GetDateTime(row, "ReferredOn"),
                StatusCode = CustomerHubDataRecordReader.GetString(row, "StatusCode"),
                Notes = CustomerHubDataRecordReader.GetString(row, "Notes")
            };
        }
    }
}
