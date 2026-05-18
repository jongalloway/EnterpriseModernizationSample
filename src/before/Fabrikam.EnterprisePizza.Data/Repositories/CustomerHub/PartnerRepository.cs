using System;
using System.Collections.Generic;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class PartnerRepository
    {
        protected readonly LegacyDbGateway _dbGateway;
        protected readonly CustomerHubDatabaseFactory _databaseFactory;

        public PartnerRepository()
            : this(new LegacyDbGateway(), new CustomerHubDatabaseFactory())
        {
        }

        public PartnerRepository(LegacyDbGateway dbGateway)
            : this(dbGateway, new CustomerHubDatabaseFactory())
        {
        }

        public PartnerRepository(LegacyDbGateway dbGateway, CustomerHubDatabaseFactory databaseFactory)
        {
            _dbGateway = dbGateway ?? throw new ArgumentNullException(nameof(dbGateway));
            _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
        }

        public IList<string> GetPreferredPartners()
        {
            var call = CreateCustomerHubCall(LegacyStoredProcedures.CustomerHub.GetPreferredPartners);
            var dataSet = _dbGateway.ExecuteDataSet(call);
            var partners = new List<string>();
            var table = dataSet.Tables["PreferredPartners"];

            if (table == null)
            {
                return partners;
            }

            foreach (DataRow row in table.Rows)
            {
                partners.Add(CustomerHubDataRecordReader.GetString(row, "PartnerName"));
            }

            return partners;
        }

        public IList<PartnerRecord> GetActivePartners()
        {
            var call = CreateCustomerHubCall(LegacyStoredProcedures.CustomerHub.GetPartners);
            return ReadPartners(_dbGateway.ExecuteDataSet(call), "Partners");
        }

        public PartnerRecord GetByCode(string partnerCode)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.GetPartnerByCode,
                new GatewayParameter("@PartnerCode", partnerCode));

            return ReadSinglePartner(_dbGateway.ExecuteDataSet(call), "Partner");
        }

        public PartnerRecord Save(PartnerRecord partner)
        {
            if (partner == null)
            {
                throw new ArgumentNullException(nameof(partner));
            }

            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.SavePartner,
                new GatewayParameter("@PartnerAccountId", partner.PartnerAccountId),
                new GatewayParameter("@PartnerCode", partner.PartnerCode),
                new GatewayParameter("@PartnerName", partner.PartnerName),
                new GatewayParameter("@RelationshipTier", partner.RelationshipTier),
                new GatewayParameter("@PreferredStoreNumber", partner.PreferredStoreNumber),
                new GatewayParameter("@StatusCode", partner.StatusCode),
                new GatewayParameter("@LastContractRenewalDate", partner.LastContractRenewalDate),
                new GatewayParameter("@CreditHold", partner.CreditHold));

            return ReadSinglePartner(_dbGateway.ExecuteDataSet(call), "Partner");
        }

        public bool Delete(string partnerCode)
        {
            var call = CreateCustomerHubCall(
                LegacyStoredProcedures.CustomerHub.DeletePartner,
                new GatewayParameter("@PartnerCode", partnerCode));
            var dataSet = _dbGateway.ExecuteDataSet(call);
            var table = dataSet.Tables["PartnerDeleteResult"];

            return table != null
                && table.Rows.Count > 0
                && CustomerHubDataRecordReader.GetBoolean(table.Rows[0], "Deleted");
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

        private static IList<PartnerRecord> ReadPartners(DataSet dataSet, string tableName)
        {
            var partners = new List<PartnerRecord>();
            var table = dataSet.Tables[tableName];
            if (table == null)
            {
                return partners;
            }

            foreach (DataRow row in table.Rows)
            {
                partners.Add(MapPartner(row));
            }

            return partners;
        }

        private static PartnerRecord ReadSinglePartner(DataSet dataSet, string tableName)
        {
            var table = dataSet.Tables[tableName];
            return table == null || table.Rows.Count == 0
                ? null
                : MapPartner(table.Rows[0]);
        }

        private static PartnerRecord MapPartner(DataRow row)
        {
            return new PartnerRecord
            {
                PartnerAccountId = CustomerHubDataRecordReader.GetInt32(row, "PartnerAccountId"),
                PartnerCode = CustomerHubDataRecordReader.GetString(row, "PartnerCode"),
                PartnerName = CustomerHubDataRecordReader.GetString(row, "PartnerName"),
                RelationshipTier = CustomerHubDataRecordReader.GetString(row, "RelationshipTier"),
                PreferredStoreNumber = CustomerHubDataRecordReader.GetString(row, "PreferredStoreNumber"),
                StatusCode = CustomerHubDataRecordReader.GetString(row, "StatusCode"),
                LastContractRenewalDate = CustomerHubDataRecordReader.GetNullableDateTime(row, "LastContractRenewalDate"),
                CreditHold = CustomerHubDataRecordReader.GetBoolean(row, "CreditHold")
            };
        }
    }
}
