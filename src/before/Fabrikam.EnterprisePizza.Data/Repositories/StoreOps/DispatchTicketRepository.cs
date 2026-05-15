using System.Collections.Generic;
using System.Data;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Data.Repositories.StoreOps
{
    public class DispatchTicketRepository : IDispatchTicketRepository
    {
        private readonly LegacyDbGateway _dbGateway;

        public DispatchTicketRepository()
            : this(new LegacyDbGateway())
        {
        }

        public DispatchTicketRepository(LegacyDbGateway dbGateway)
        {
            _dbGateway = dbGateway;
        }

        public IList<DispatchTicket> GetActiveTickets(string storeNumber)
        {
            var call = _dbGateway.CreateStoredProcedureCall(
                LegacyDatabaseArea.StoreOps,
                LegacyStoredProcedures.StoreOps.GetActiveDispatchTickets,
                new GatewayParameter("@StoreNumber", storeNumber));
            var dataSet = _dbGateway.ExecuteDataSet(call);
            var table = dataSet.Tables["DispatchTickets"];
            var tickets = new List<DispatchTicket>();

            if (table == null)
            {
                return tickets;
            }

            foreach (DataRow row in table.Rows)
            {
                tickets.Add(new DispatchTicket
                {
                    TicketId = (int)row["TicketId"],
                    StoreNumber = row["StoreNumber"].ToString(),
                    DriverCode = row["DriverCode"].ToString(),
                    RouteZone = row["RouteZone"].ToString()
                });
            }

            return tickets;
        }
    }
}
