using System;
using System.Collections.Generic;
using System.Data;
using Fabrikam.EnterprisePizza.Core.Domain;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Data.Repositories.StoreOps
{
    public class OrderRepository : IOrderRepository
    {
        private readonly LegacyDbGateway gateway;
        private readonly LegacyConnectionCatalog connectionCatalog;

        public OrderRepository()
            : this(new LegacyDbGateway(), new LegacyConnectionCatalog())
        {
        }

        public OrderRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public OrderRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
        }

        public OrderRecord GetOrder(int orderNumber)
        {
            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.StoreOps),
                    LegacyStoredProcedures.StoreOps.GetOrder,
                    new GatewayParameter("@OrderNumber", orderNumber)));

            return MapOrderRecord(GetFirstRow(dataSet));
        }

        public IList<OrderLookupRecord> SearchOrders(OrderSearchRequest request)
        {
            var searchRequest = request ?? new OrderSearchRequest();
            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.StoreOps),
                    LegacyStoredProcedures.StoreOps.SearchOrders,
                    new GatewayParameter("@StoreNumber", searchRequest.StoreNumber),
                    new GatewayParameter("@SearchText", searchRequest.SearchText),
                    new GatewayParameter("@StatusCode", searchRequest.StatusCode),
                    new GatewayParameter("@FromSubmittedUtc", searchRequest.FromSubmittedUtc == DateTime.MinValue ? (object)DBNull.Value : searchRequest.FromSubmittedUtc),
                    new GatewayParameter("@ToSubmittedUtc", searchRequest.ToSubmittedUtc == DateTime.MinValue ? (object)DBNull.Value : searchRequest.ToSubmittedUtc),
                    new GatewayParameter("@IncludeClosed", searchRequest.IncludeClosed)));

            var orders = new List<OrderLookupRecord>();
            var table = GetFirstTable(dataSet);
            if (table == null)
            {
                return orders;
            }

            foreach (DataRow row in table.Rows)
            {
                orders.Add(new OrderLookupRecord
                {
                    OrderNumber = GetInt32(row, "OrderNumber"),
                    StoreNumber = GetString(row, "StoreNumber"),
                    CustomerName = GetString(row, "CustomerName"),
                    Channel = GetString(row, "Channel"),
                    ServiceMode = GetString(row, "ServiceMode"),
                    PromiseWindow = GetString(row, "PromiseWindow"),
                    TicketTotal = GetDecimal(row, "TicketTotal"),
                    KitchenStatus = GetString(row, "KitchenStatus"),
                    DispatchStatus = GetString(row, "DispatchStatus"),
                    PaymentStatus = GetString(row, "PaymentStatus")
                });
            }

            return orders;
        }

        public OrderRecord PlaceOrder(OrderPlacementRequest request, DeliveryPromise promise, DateTime submittedAtUtc)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (promise == null)
            {
                throw new ArgumentNullException(nameof(promise));
            }

            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.StoreOps),
                    LegacyStoredProcedures.StoreOps.PlaceOrder,
                    new GatewayParameter("@StoreNumber", request.StoreNumber),
                    new GatewayParameter("@CustomerName", request.CustomerName),
                    new GatewayParameter("@Channel", request.Channel),
                    new GatewayParameter("@ServiceMode", request.ServiceMode),
                    new GatewayParameter("@TicketTotal", request.TicketTotal),
                    new GatewayParameter("@IsCorporateAccount", request.IsCorporateAccount),
                    new GatewayParameter("@DeliveryMileage", request.DeliveryMileage),
                    new GatewayParameter("@NeededByUtc", request.NeededByUtc),
                    new GatewayParameter("@DeliveryAddress", request.DeliveryAddress),
                    new GatewayParameter("@SpecialInstructions", request.SpecialInstructions),
                    new GatewayParameter("@FulfillmentLane", promise.Lane.ToString()),
                    new GatewayParameter("@QuotedReadyTimeUtc", promise.QuotedReadyTime),
                    new GatewayParameter("@SubmittedAtUtc", submittedAtUtc)));

            return MapOrderRecord(GetFirstRow(dataSet));
        }

        public OrderRecord UpdateOrderStatus(OrderStatusUpdateRequest request, DateTime updatedAtUtc)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.StoreOps),
                    LegacyStoredProcedures.StoreOps.UpdateOrderStatus,
                    new GatewayParameter("@OrderNumber", request.OrderNumber),
                    new GatewayParameter("@StoreNumber", request.StoreNumber),
                    new GatewayParameter("@StatusCode", request.StatusCode),
                    new GatewayParameter("@UpdatedBy", request.UpdatedBy),
                    new GatewayParameter("@StatusNote", request.StatusNote),
                    new GatewayParameter("@UpdatedAtUtc", updatedAtUtc)));

            return MapOrderRecord(GetFirstRow(dataSet));
        }

        public IList<OrderHistoryRecord> GetOrderHistory(int orderNumber)
        {
            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.StoreOps),
                    LegacyStoredProcedures.StoreOps.GetOrderHistory,
                    new GatewayParameter("@OrderNumber", orderNumber)));

            var items = new List<OrderHistoryRecord>();
            var table = GetFirstTable(dataSet);
            if (table == null)
            {
                return items;
            }

            foreach (DataRow row in table.Rows)
            {
                items.Add(new OrderHistoryRecord
                {
                    LoggedAtUtc = GetDateTime(row, "LoggedAtUtc"),
                    StatusCode = GetString(row, "StatusCode"),
                    Note = GetString(row, "Note"),
                    UpdatedBy = GetString(row, "UpdatedBy"),
                    SourceSystem = GetString(row, "SourceSystem")
                });
            }

            return items;
        }

        private static OrderRecord MapOrderRecord(DataRow row)
        {
            if (row == null)
            {
                return null;
            }

            return new OrderRecord
            {
                OrderNumber = GetInt32(row, "OrderNumber"),
                StoreNumber = GetString(row, "StoreNumber"),
                CustomerName = GetString(row, "CustomerName"),
                Channel = GetString(row, "Channel"),
                ServiceMode = GetString(row, "ServiceMode"),
                OrderStatus = GetString(row, "OrderStatus"),
                KitchenStatus = GetString(row, "KitchenStatus"),
                DispatchStatus = GetString(row, "DispatchStatus"),
                PaymentStatus = GetString(row, "PaymentStatus"),
                TicketTotal = GetDecimal(row, "TicketTotal"),
                SubmittedAtUtc = GetDateTime(row, "SubmittedAtUtc"),
                NeededByUtc = GetDateTime(row, "NeededByUtc"),
                QuotedReadyTimeUtc = GetDateTime(row, "QuotedReadyTimeUtc"),
                FulfillmentLane = GetString(row, "FulfillmentLane"),
                DeliveryAddress = GetString(row, "DeliveryAddress"),
                SpecialInstructions = GetString(row, "SpecialInstructions"),
                IsCorporateAccount = GetBoolean(row, "IsCorporateAccount"),
                DeliveryMileage = GetDecimal(row, "DeliveryMileage"),
                CurrentDriverCode = GetString(row, "CurrentDriverCode")
            };
        }

        private static DataRow GetFirstRow(DataSet dataSet)
        {
            var table = GetFirstTable(dataSet);
            return table == null || table.Rows.Count == 0 ? null : table.Rows[0];
        }

        private static DataTable GetFirstTable(DataSet dataSet)
        {
            return dataSet == null || dataSet.Tables.Count == 0 ? null : dataSet.Tables[0];
        }

        private static string GetString(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? null : Convert.ToString(row[columnName]);
        }

        private static DateTime GetDateTime(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? DateTime.MinValue : Convert.ToDateTime(row[columnName]);
        }

        private static decimal GetDecimal(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? 0m : Convert.ToDecimal(row[columnName]);
        }

        private static int GetInt32(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? 0 : Convert.ToInt32(row[columnName]);
        }

        private static bool GetBoolean(DataRow row, string columnName)
        {
            return HasValue(row, columnName) && Convert.ToBoolean(row[columnName]);
        }

        private static bool HasValue(DataRow row, string columnName)
        {
            return row != null
                && row.Table != null
                && row.Table.Columns.Contains(columnName)
                && row[columnName] != DBNull.Value;
        }
    }
}
