using System;
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
                    RouteZone = row["RouteZone"].ToString(),
                    CustomerName = ReadString(row, "CustomerName", "Delivery Customer"),
                    DeliveryAddress = ReadString(row, "DeliveryAddress", string.Empty),
                    ReadyAtLocal = ReadDateTime(row, "ReadyAtLocal", DateTime.Today.AddHours(17).AddMinutes(15)),
                    PromiseTimeLocal = ReadDateTime(row, "PromiseTimeLocal", DateTime.Today.AddHours(17).AddMinutes(35)),
                    RouteDistanceMiles = ReadDecimal(row, "RouteDistanceMiles", 5.0m),
                    EstimatedTravelMinutes = ReadInt(row, "EstimatedTravelMinutes", 18),
                    RequiresPairing = ReadBool(row, "RequiresPairing", false),
                    Status = ReadStatus(row, "Status", DeliveryStatus.ReadyForDispatch),
                    PriorityScore = ReadInt(row, "PriorityScore", 50)
                });
            }

            return tickets;
        }

        private static string ReadString(DataRow row, string columnName, string fallback)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? row[columnName].ToString()
                : fallback;
        }

        private static DateTime ReadDateTime(DataRow row, string columnName, DateTime fallback)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToDateTime(row[columnName])
                : fallback;
        }

        private static decimal ReadDecimal(DataRow row, string columnName, decimal fallback)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToDecimal(row[columnName])
                : fallback;
        }

        private static int ReadInt(DataRow row, string columnName, int fallback)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToInt32(row[columnName])
                : fallback;
        }

        private static bool ReadBool(DataRow row, string columnName, bool fallback)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToBoolean(row[columnName])
                : fallback;
        }

        private static DeliveryStatus ReadStatus(DataRow row, string columnName, DeliveryStatus fallback)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return fallback;
            }

            DeliveryStatus parsedStatus;
            return Enum.TryParse(row[columnName].ToString(), true, out parsedStatus)
                ? parsedStatus
                : fallback;
        }
    }
}
