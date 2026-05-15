using System.Collections.Generic;

using System;
using System.Collections.Generic;

namespace Fabrikam.EnterprisePizza.Data.Gateways
{
    public class StoredProcedureCall
    {
        public StoredProcedureCall(string connectionName, string procedureName, params GatewayParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ArgumentException("Connection name is required.", nameof(connectionName));
            }

            if (string.IsNullOrWhiteSpace(procedureName))
            {
                throw new ArgumentException("Stored procedure name is required.", nameof(procedureName));
            }

            ConnectionName = connectionName;
            ProcedureName = procedureName;
            Parameters = parameters == null
                ? new List<GatewayParameter>()
                : new List<GatewayParameter>(parameters);
        }

        public string ConnectionName { get; private set; }

        public string ProcedureName { get; private set; }

        public string StoredProcedureName
        {
            get { return ProcedureName; }
        }

        public IList<GatewayParameter> Parameters { get; private set; }
    }
}
