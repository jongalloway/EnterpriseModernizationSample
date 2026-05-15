using System.Collections.Generic;

namespace Fabrikam.EnterprisePizza.Data.Gateways
{
    public class StoredProcedureCall
    {
        public StoredProcedureCall(string connectionName, string procedureName, params GatewayParameter[] parameters)
        {
            ConnectionName = connectionName;
            ProcedureName = procedureName;
            Parameters = parameters ?? new GatewayParameter[0];
        }

        public string ConnectionName { get; private set; }

        public string ProcedureName { get; private set; }

        public IEnumerable<GatewayParameter> Parameters { get; private set; }
    }
}
