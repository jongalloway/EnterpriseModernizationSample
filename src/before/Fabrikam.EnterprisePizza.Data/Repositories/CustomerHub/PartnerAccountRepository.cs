using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fabrikam.EnterprisePizza.Core.Domain.CustomerHub;
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
            _dbGateway = dbGateway;
        }

        public IList<string> GetPreferredPartners()
        {
            return GetPreferredPartnerSnapshots().Select(snapshot => snapshot.PartnerName).ToList();
        }

        public IList<PartnerAccountSnapshot> GetPreferredPartnerSnapshots()
        {
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.CustomerHub,
                LegacyStoredProcedures.CustomerHub.GetPreferredPartners);
            var dataSet = _dbGateway.ExecuteDataSet(call);
            var partners = new List<PartnerAccountSnapshot>();
            var table = dataSet.Tables["PreferredPartners"];

            if (table == null)
            {
                return partners;
            }

            foreach (DataRow row in table.Rows)
            {
                partners.Add(new PartnerAccountSnapshot
                {
                    PartnerCode = row["PartnerCode"].ToString(),
                    PartnerName = row["PartnerName"].ToString(),
                    RelationshipTier = row["RelationshipTier"].ToString(),
                    PreferredStoreNumber = row["PreferredStoreNumber"].ToString(),
                    AccountCode = row["AccountCode"].ToString(),
                    AccountName = row["AccountName"].ToString()
                });
            }

            return partners;
        }
    }
}
