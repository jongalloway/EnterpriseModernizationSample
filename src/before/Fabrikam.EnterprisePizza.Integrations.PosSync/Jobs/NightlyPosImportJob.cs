using System;
using Fabrikam.EnterprisePizza.Core.Domain;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.Repositories.StoreOps;

namespace Fabrikam.EnterprisePizza.Integrations.PosSync.Jobs
{
    public class NightlyPosImportJob
    {
        private readonly LegacyDbGateway gateway;
        private readonly IPosImportBatchRepository posImportBatchRepository;

        public NightlyPosImportJob()
            : this(new LegacyDbGateway())
        {
        }

        public NightlyPosImportJob(LegacyDbGateway gateway)
            : this(gateway, new PosImportBatchRepository(gateway))
        {
        }

        public NightlyPosImportJob(LegacyDbGateway gateway, IPosImportBatchRepository posImportBatchRepository)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this.posImportBatchRepository = posImportBatchRepository ?? throw new ArgumentNullException(nameof(posImportBatchRepository));
        }

        public string GetTargetDatabase()
        {
            return gateway.GetConnectionName("StoreOps");
        }

        public PosImportBatchSnapshot GetLatestImportedBatch(string storeNumber)
        {
            return posImportBatchRepository.GetLatestBatch(storeNumber);
        }
    }
}
