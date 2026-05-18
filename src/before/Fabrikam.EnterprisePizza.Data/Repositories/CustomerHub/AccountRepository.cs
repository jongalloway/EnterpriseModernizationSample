using System;
using System.Collections.Generic;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class AccountRepository
    {
        private readonly LegacyDbGateway _dbGateway;
        private readonly CustomerHubDatabaseFactory _databaseFactory;

        public AccountRepository()
            : this(new LegacyDbGateway(), new CustomerHubDatabaseFactory())
        {
        }

        public AccountRepository(LegacyDbGateway dbGateway)
            : this(dbGateway, new CustomerHubDatabaseFactory())
        {
        }

        public AccountRepository(LegacyDbGateway dbGateway, CustomerHubDatabaseFactory databaseFactory)
        {
            _dbGateway = dbGateway ?? throw new ArgumentNullException(nameof(dbGateway));
            _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
        }

        public IList<AccountRecord> GetByTier(string accountTier)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.GetAccountsByTier,
                new GatewayParameter("@AccountTier", accountTier));
            return ReadAccounts(_dbGateway.ExecuteDataSet(call), "Accounts");
        }

        public AccountRecord GetByCode(string accountCode)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.GetAccountByCode,
                new GatewayParameter("@AccountCode", accountCode));
            return ReadSingleAccount(_dbGateway.ExecuteDataSet(call), "Account");
        }

        public AccountRecord Save(AccountRecord account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.SaveAccount,
                new GatewayParameter("@CorporateAccountId", account.CorporateAccountId),
                new GatewayParameter("@AccountCode", account.AccountCode),
                new GatewayParameter("@AccountName", account.AccountName),
                new GatewayParameter("@PreferredPartnerAccountId", account.PreferredPartnerAccountId),
                new GatewayParameter("@BillingFrequency", account.BillingFrequency),
                new GatewayParameter("@ActiveFlag", account.ActiveFlag),
                new GatewayParameter("@AccountTier", account.AccountTier),
                new GatewayParameter("@ExternalAccountCode", account.ExternalAccountCode),
                new GatewayParameter("@AccountManagerName", account.AccountManagerName),
                new GatewayParameter("@StatusCode", account.StatusCode));

            return ReadSingleAccount(_dbGateway.ExecuteDataSet(call), "Account");
        }

        public AccountRecord Deactivate(string accountCode)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.DeactivateAccount,
                new GatewayParameter("@AccountCode", accountCode));
            return ReadSingleAccount(_dbGateway.ExecuteDataSet(call), "Account");
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

        private static IList<AccountRecord> ReadAccounts(DataSet dataSet, string tableName)
        {
            var accounts = new List<AccountRecord>();
            var table = dataSet.Tables[tableName];
            if (table == null)
            {
                return accounts;
            }

            foreach (DataRow row in table.Rows)
            {
                accounts.Add(MapAccount(row));
            }

            return accounts;
        }

        private static AccountRecord ReadSingleAccount(DataSet dataSet, string tableName)
        {
            var table = dataSet.Tables[tableName];
            return table == null || table.Rows.Count == 0
                ? null
                : MapAccount(table.Rows[0]);
        }

        private static AccountRecord MapAccount(DataRow row)
        {
            return new AccountRecord
            {
                CorporateAccountId = CustomerHubDataRecordReader.GetInt32(row, "CorporateAccountId"),
                AccountCode = CustomerHubDataRecordReader.GetString(row, "AccountCode"),
                AccountName = CustomerHubDataRecordReader.GetString(row, "AccountName"),
                PreferredPartnerAccountId = CustomerHubDataRecordReader.GetNullableInt32(row, "PreferredPartnerAccountId"),
                BillingFrequency = CustomerHubDataRecordReader.GetString(row, "BillingFrequency"),
                ActiveFlag = CustomerHubDataRecordReader.GetBoolean(row, "ActiveFlag"),
                AccountTier = CustomerHubDataRecordReader.GetString(row, "AccountTier"),
                ExternalAccountCode = CustomerHubDataRecordReader.GetString(row, "ExternalAccountCode"),
                AccountManagerName = CustomerHubDataRecordReader.GetString(row, "AccountManagerName"),
                StatusCode = CustomerHubDataRecordReader.GetString(row, "StatusCode")
            };
        }
    }
}
