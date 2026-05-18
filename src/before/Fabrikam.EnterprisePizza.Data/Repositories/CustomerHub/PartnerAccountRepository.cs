using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class PartnerAccountRepository : PartnerRepository, IPartnerAccountRepository
    {
        public PartnerAccountRepository()
        {
        }

        public PartnerAccountRepository(LegacyDbGateway dbGateway)
            : base(dbGateway)
        {
        }

        public PartnerAccountRepository(LegacyDbGateway dbGateway, CustomerHubDatabaseFactory databaseFactory)
            : base(dbGateway, databaseFactory)
        {
        }
    }
}
