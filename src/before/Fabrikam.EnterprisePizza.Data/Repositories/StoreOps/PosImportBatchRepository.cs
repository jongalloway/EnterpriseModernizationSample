using System;
using System.Data;
using Fabrikam.EnterprisePizza.Core.Domain;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.StoreOps
{
    public class PosImportBatchRepository : IPosImportBatchRepository
    {
        private readonly LegacyDbGateway gateway;
        private readonly LegacyConnectionCatalog connectionCatalog;

        public PosImportBatchRepository()
            : this(new LegacyDbGateway())
        {
        }

        public PosImportBatchRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public PosImportBatchRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
        }

        public PosImportBatchSnapshot GetLatestBatch(string storeNumber)
        {
            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.StoreOps),
                    LegacyStoredProcedures.StoreOps.GetLatestPosImportBatch,
                    new GatewayParameter("@StoreNumber", storeNumber)));

            var table = dataSet == null || dataSet.Tables.Count == 0 ? null : dataSet.Tables[0];
            if (table == null || table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];
            return new PosImportBatchSnapshot
            {
                PosOrderImportBatchId = GetInt32(row, "PosOrderImportBatchId"),
                StoreNumber = GetString(row, "StoreNumber"),
                SourceSystem = GetString(row, "SourceSystem"),
                BatchDate = GetDateTime(row, "BatchDate"),
                ImportedUtc = GetDateTime(row, "ImportedUtc"),
                BatchStatus = GetString(row, "BatchStatus"),
                ItemCount = GetInt32(row, "ItemCount")
            };
        }

        private static string GetString(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? null : Convert.ToString(row[columnName]);
        }

        private static DateTime GetDateTime(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? DateTime.MinValue : Convert.ToDateTime(row[columnName]);
        }

        private static int GetInt32(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? 0 : Convert.ToInt32(row[columnName]);
        }

        private static bool HasValue(DataRow row, string columnName)
        {
            return row != null
                && row.Table != null
                && row.Table.Columns.Contains(columnName)
                && row[columnName] != DBNull.Value;
        }
    }
}
