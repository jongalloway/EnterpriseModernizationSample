using System;
using System.Collections.Generic;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class ContractRepository
    {
        private readonly LegacyDbGateway _dbGateway;
        private readonly CustomerHubDatabaseFactory _databaseFactory;

        public ContractRepository()
            : this(new LegacyDbGateway(), new CustomerHubDatabaseFactory())
        {
        }

        public ContractRepository(LegacyDbGateway dbGateway)
            : this(dbGateway, new CustomerHubDatabaseFactory())
        {
        }

        public ContractRepository(LegacyDbGateway dbGateway, CustomerHubDatabaseFactory databaseFactory)
        {
            _dbGateway = dbGateway ?? throw new ArgumentNullException(nameof(dbGateway));
            _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
        }

        public IList<ContractRecord> GetByStatus(string statusCode)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.GetContractsByStatus,
                new GatewayParameter("@StatusCode", statusCode));
            return ReadContracts(_dbGateway.ExecuteDataSet(call), "Contracts");
        }

        public ContractRecord GetByCode(string contractCode)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.GetContractByCode,
                new GatewayParameter("@ContractCode", contractCode));
            return ReadSingleContract(_dbGateway.ExecuteDataSet(call), "Contract");
        }

        public ContractRecord Save(ContractRecord contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.SaveContract,
                new GatewayParameter("@PartnerContractId", contract.PartnerContractId),
                new GatewayParameter("@ContractCode", contract.ContractCode),
                new GatewayParameter("@PartnerAccountId", contract.PartnerAccountId),
                new GatewayParameter("@CorporateAccountId", contract.CorporateAccountId),
                new GatewayParameter("@FranchiseLocationId", contract.FranchiseLocationId),
                new GatewayParameter("@ContractType", contract.ContractType),
                new GatewayParameter("@PricingScheduleName", contract.PricingScheduleName),
                new GatewayParameter("@ReferralChannel", contract.ReferralChannel),
                new GatewayParameter("@EffectiveDate", contract.EffectiveDate),
                new GatewayParameter("@ExpirationDate", contract.ExpirationDate),
                new GatewayParameter("@MinimumOrderAmount", contract.MinimumOrderAmount),
                new GatewayParameter("@DiscountPercentage", contract.DiscountPercentage),
                new GatewayParameter("@CateringLeadHours", contract.CateringLeadHours),
                new GatewayParameter("@StatusCode", contract.StatusCode),
                new GatewayParameter("@LastReviewedUtc", contract.LastReviewedUtc));

            return ReadSingleContract(_dbGateway.ExecuteDataSet(call), "Contract");
        }

        public ContractRecord UpdateStatus(string contractCode, string statusCode)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.UpdateContractStatus,
                new GatewayParameter("@ContractCode", contractCode),
                new GatewayParameter("@StatusCode", statusCode));
            return ReadSingleContract(_dbGateway.ExecuteDataSet(call), "Contract");
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

        private static IList<ContractRecord> ReadContracts(DataSet dataSet, string tableName)
        {
            var contracts = new List<ContractRecord>();
            var table = dataSet.Tables[tableName];
            if (table == null)
            {
                return contracts;
            }

            foreach (DataRow row in table.Rows)
            {
                contracts.Add(MapContract(row));
            }

            return contracts;
        }

        private static ContractRecord ReadSingleContract(DataSet dataSet, string tableName)
        {
            var table = dataSet.Tables[tableName];
            return table == null || table.Rows.Count == 0
                ? null
                : MapContract(table.Rows[0]);
        }

        private static ContractRecord MapContract(DataRow row)
        {
            return new ContractRecord
            {
                PartnerContractId = CustomerHubDataRecordReader.GetInt32(row, "PartnerContractId"),
                ContractCode = CustomerHubDataRecordReader.GetString(row, "ContractCode"),
                PartnerAccountId = CustomerHubDataRecordReader.GetInt32(row, "PartnerAccountId"),
                CorporateAccountId = CustomerHubDataRecordReader.GetNullableInt32(row, "CorporateAccountId"),
                FranchiseLocationId = CustomerHubDataRecordReader.GetNullableInt32(row, "FranchiseLocationId"),
                ContractType = CustomerHubDataRecordReader.GetString(row, "ContractType"),
                PricingScheduleName = CustomerHubDataRecordReader.GetString(row, "PricingScheduleName"),
                ReferralChannel = CustomerHubDataRecordReader.GetString(row, "ReferralChannel"),
                EffectiveDate = CustomerHubDataRecordReader.GetDateTime(row, "EffectiveDate"),
                ExpirationDate = CustomerHubDataRecordReader.GetNullableDateTime(row, "ExpirationDate"),
                MinimumOrderAmount = CustomerHubDataRecordReader.GetDecimal(row, "MinimumOrderAmount"),
                DiscountPercentage = CustomerHubDataRecordReader.GetDecimal(row, "DiscountPercentage"),
                CateringLeadHours = CustomerHubDataRecordReader.GetInt16(row, "CateringLeadHours"),
                StatusCode = CustomerHubDataRecordReader.GetString(row, "StatusCode"),
                LastReviewedUtc = CustomerHubDataRecordReader.GetDateTime(row, "LastReviewedUtc")
            };
        }
    }
}
