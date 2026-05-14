using Fabrikam.EnterprisePizza.Data.Gateways;

namespace Fabrikam.EnterprisePizza.Integrations.PosSync.Jobs
{
    public class NightlyPosImportJob
    {
        private readonly LegacyDbGateway _dbGateway = new LegacyDbGateway();

        public string GetTargetDatabase()
        {
            return _dbGateway.GetConnectionName("StoreOps");
        }
    }
}
