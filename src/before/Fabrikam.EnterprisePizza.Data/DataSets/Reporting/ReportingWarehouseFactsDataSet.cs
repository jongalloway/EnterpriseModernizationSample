using System;
using System.Data;

namespace Fabrikam.EnterprisePizza.Data.DataSets.Reporting
{
    public sealed class ReportingWarehouseFactsDataSet : DataSet
    {
        public const string FactDeliveryTableName = "FactDelivery";
        public const string FactOrderTableName = "FactOrder";
        public const string FactPartnerRevenueTableName = "FactPartnerRevenue";

        public ReportingWarehouseFactsDataSet()
            : base("ReportingWarehouseFacts")
        {
            Tables.Add(CreateFactDeliveryTable());
            Tables.Add(CreateFactOrderTable());
            Tables.Add(CreateFactPartnerRevenueTable());
        }

        public DataTable FactDelivery
        {
            get { return Tables[FactDeliveryTableName]; }
        }

        public DataTable FactOrder
        {
            get { return Tables[FactOrderTableName]; }
        }

        public DataTable FactPartnerRevenue
        {
            get { return Tables[FactPartnerRevenueTableName]; }
        }

        public static ReportingWarehouseFactsDataSet FromTable(string tableName, DataTable source)
        {
            var dataSet = new ReportingWarehouseFactsDataSet();
            dataSet.LoadTable(tableName, source);
            return dataSet;
        }

        public void LoadTable(string tableName, DataTable source)
        {
            if (!Tables.Contains(tableName))
            {
                throw new ArgumentOutOfRangeException(nameof(tableName), tableName, "Unknown reporting fact table.");
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

        private static DataTable CreateFactDeliveryTable()
        {
            var table = new DataTable(FactDeliveryTableName);
            table.Columns.Add("DeliveryFactKey", typeof(int));
            table.Columns.Add("DateKey", typeof(int));
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("DriverKey", typeof(int));
            table.Columns.Add("OrderNumber", typeof(string));
            table.Columns.Add("CompletedRuns", typeof(int));
            table.Columns.Add("LateRuns", typeof(int));
            table.Columns.Add("DeliveryMinutes", typeof(decimal));
            table.Columns.Add("RouteMiles", typeof(decimal));
            table.Columns.Add("DeliveryRevenue", typeof(decimal));
            return table;
        }

        private static DataTable CreateFactOrderTable()
        {
            var table = new DataTable(FactOrderTableName);
            table.Columns.Add("OrderFactKey", typeof(int));
            table.Columns.Add("DateKey", typeof(int));
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("PartnerKey", typeof(int));
            table.Columns.Add("OrderChannel", typeof(string));
            table.Columns.Add("OrderCount", typeof(int));
            table.Columns.Add("AverageTicket", typeof(decimal));
            table.Columns.Add("GrossSales", typeof(decimal));
            table.Columns.Add("DiscountAmount", typeof(decimal));
            table.Columns.Add("NetSales", typeof(decimal));
            return table;
        }

        private static DataTable CreateFactPartnerRevenueTable()
        {
            var table = new DataTable(FactPartnerRevenueTableName);
            table.Columns.Add("PartnerRevenueFactKey", typeof(int));
            table.Columns.Add("DateKey", typeof(int));
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("PartnerKey", typeof(int));
            table.Columns.Add("DeliveredOrders", typeof(int));
            table.Columns.Add("PartnerSales", typeof(decimal));
            table.Columns.Add("CommissionRate", typeof(decimal));
            table.Columns.Add("SettlementAmount", typeof(decimal));
            table.Columns.Add("NetMargin", typeof(decimal));
            return table;
        }
    }
}
