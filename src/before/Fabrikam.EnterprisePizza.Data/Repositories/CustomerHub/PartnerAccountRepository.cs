using System.Collections.Generic;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class PartnerAccountRepository : IPartnerAccountRepository
    {
        private readonly LegacyDbGateway _dbGateway;

        public PartnerAccountRepository()
            : this(new LegacyDbGateway())
        {
        }

        public PartnerAccountRepository(LegacyDbGateway dbGateway)
        {
            _dbGateway = dbGateway ?? throw new System.ArgumentNullException(nameof(dbGateway));
        }

        public IList<string> GetPreferredPartners()
        {
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.CustomerHub,
                LegacyStoredProcedures.CustomerHub.GetPreferredPartners);
            var dataSet = _dbGateway.ExecuteDataSet(call);
            var partners = new List<string>();
            var table = dataSet == null ? null : dataSet.Tables["PreferredPartners"];

            if (table == null)
            {
                return partners;
            }

            foreach (DataRow row in table.Rows)
            {
                partners.Add(row["PartnerName"].ToString());
            }

            return partners;
        }
    }
}
