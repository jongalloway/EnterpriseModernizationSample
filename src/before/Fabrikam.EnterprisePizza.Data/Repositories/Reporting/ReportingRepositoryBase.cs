using System;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public abstract class ReportingRepositoryBase
    {
        protected readonly LegacyDbGateway gateway;
        protected readonly LegacyConnectionCatalog connectionCatalog;

        protected ReportingRepositoryBase(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
        }

        protected DataSet ExecuteReportingDataSet(string procedureName, params GatewayParameter[] parameters)
        {
            return gateway.ExecuteDataSet(new StoredProcedureCall(connectionCatalog.GetConnectionName(LegacyDatabaseArea.Reporting), procedureName, parameters));
        }

        protected static DataTable GetFirstTable(DataSet dataSet)
        {
            return dataSet == null || dataSet.Tables.Count == 0 ? null : dataSet.Tables[0];
        }
    }
}
