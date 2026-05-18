using System;
using System.Data;

namespace Fabrikam.EnterprisePizza.Data.DataSets.Reporting
{
    public sealed class ReportingWarehouseStarSchemaDataSet : DataSet
    {
        public const string DeliveryStoreDriverSnapshotTableName = "DeliveryStoreDriverSnapshot";
        public const string OrderChannelMixSnapshotTableName = "OrderChannelMixSnapshot";
        public const string PartnerRevenueSettlementSnapshotTableName = "PartnerRevenueSettlementSnapshot";

        public ReportingWarehouseStarSchemaDataSet()
            : base("ReportingWarehouseStarSchema")
        {
            Tables.Add(CreateDeliveryStoreDriverSnapshotTable());
            Tables.Add(CreateOrderChannelMixSnapshotTable());
            Tables.Add(CreatePartnerRevenueSettlementSnapshotTable());
        }

        public DataTable DeliveryStoreDriverSnapshot
        {
            get { return Tables[DeliveryStoreDriverSnapshotTableName]; }
        }

        public DataTable OrderChannelMixSnapshot
        {
            get { return Tables[OrderChannelMixSnapshotTableName]; }
        }

        public DataTable PartnerRevenueSettlementSnapshot
        {
            get { return Tables[PartnerRevenueSettlementSnapshotTableName]; }
        }

        public static ReportingWarehouseStarSchemaDataSet FromTable(string tableName, DataTable source)
        {
            var dataSet = new ReportingWarehouseStarSchemaDataSet();
            dataSet.LoadTable(tableName, source);
            return dataSet;
        }

        public void LoadTable(string tableName, DataTable source)
        {
            if (!Tables.Contains(tableName))
            {
                throw new ArgumentOutOfRangeException(nameof(tableName), tableName, "Unknown star-schema reporting table.");
            }

            var target = Tables[tableName];
            target.Clear();
            if (source == null)
            {
                return;
            }

            foreach (DataRow row in source.Rows)
            {
                target.ImportRow(row);
            }
        }

        private static DataTable CreateDeliveryStoreDriverSnapshotTable()
        {
            var table = new DataTable(DeliveryStoreDriverSnapshotTableName);
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("StoreName", typeof(string));
            table.Columns.Add("CalendarDate", typeof(DateTime));
            table.Columns.Add("DriverCode", typeof(string));
            table.Columns.Add("DriverName", typeof(string));
            table.Columns.Add("CompletedRuns", typeof(int));
            table.Columns.Add("LateRuns", typeof(int));
            table.Columns.Add("DeliveryMinutes", typeof(decimal));
            table.Columns.Add("RouteMiles", typeof(decimal));
            table.Columns.Add("DeliveryRevenue", typeof(decimal));
            return table;
        }

        private static DataTable CreateOrderChannelMixSnapshotTable()
        {
            var table = new DataTable(OrderChannelMixSnapshotTableName);
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("StoreName", typeof(string));
            table.Columns.Add("CalendarDate", typeof(DateTime));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("OrderChannel", typeof(string));
            table.Columns.Add("OrderCount", typeof(int));
            table.Columns.Add("GrossSales", typeof(decimal));
            table.Columns.Add("DiscountAmount", typeof(decimal));
            table.Columns.Add("NetSales", typeof(decimal));
            return table;
        }

        private static DataTable CreatePartnerRevenueSettlementSnapshotTable()
        {
            var table = new DataTable(PartnerRevenueSettlementSnapshotTableName);
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("StoreName", typeof(string));
            table.Columns.Add("CalendarMonthLabel", typeof(string));
            table.Columns.Add("PartnerCode", typeof(string));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("DeliveredOrders", typeof(int));
            table.Columns.Add("PartnerSales", typeof(decimal));
            table.Columns.Add("CommissionRate", typeof(decimal));
            table.Columns.Add("SettlementAmount", typeof(decimal));
            table.Columns.Add("NetMargin", typeof(decimal));
            return table;
        }
    }
}
