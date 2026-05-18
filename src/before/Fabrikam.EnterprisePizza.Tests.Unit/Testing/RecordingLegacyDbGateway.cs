using System;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Testing
{
    internal class RecordingLegacyDbGateway : LegacyDbGateway
    {
        public RecordingLegacyDbGateway()
            : base(new LegacyConnectionCatalog())
        {
        }

        public LegacyDatabaseArea? LastArea { get; private set; }

        public string LastProcedureName { get; private set; }

        public GatewayParameter[] LastCreateParameters { get; private set; }

        public StoredProcedureCall LastExecutedCall { get; private set; }

        public DataSet DataSetToReturn { get; set; }

        public Exception ExceptionToThrow { get; set; }

        public override StoredProcedureCall CreateStoredProcedureCall(LegacyDatabaseArea area, string procedureName, params GatewayParameter[] parameters)
        {
            LastArea = area;
            LastProcedureName = procedureName;
            LastCreateParameters = parameters ?? new GatewayParameter[0];
            return base.CreateStoredProcedureCall(area, procedureName, parameters);
        }

        public override DataSet ExecuteDataSet(StoredProcedureCall call)
        {
            LastExecutedCall = call;
            if (ExceptionToThrow != null)
            {
                throw ExceptionToThrow;
            }

            return DataSetToReturn;
        }
    }
}
