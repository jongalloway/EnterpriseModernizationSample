using System;
using System.Data;

namespace Fabrikam.EnterprisePizza.Data.DataSets.Reporting
{
    public sealed class ReportingWarehouseDimensionsDataSet : DataSet
    {
        public const string DimStoreTableName = "DimStore";
        public const string DimDriverTableName = "DimDriver";
        public const string DimPartnerTableName = "DimPartner";
        public const string DimTimeTableName = "DimTime";

        public ReportingWarehouseDimensionsDataSet()
            : base("ReportingWarehouseDimensions")
        {
            Tables.Add(CreateDimStoreTable());
            Tables.Add(CreateDimDriverTable());
            Tables.Add(CreateDimPartnerTable());
            Tables.Add(CreateDimTimeTable());
        }

        public DataTable DimStore
        {
            get { return Tables[DimStoreTableName]; }
        }

        public DataTable DimDriver
        {
            get { return Tables[DimDriverTableName]; }
        }

        public DataTable DimPartner
        {
            get { return Tables[DimPartnerTableName]; }
        }

        public DataTable DimTime
        {
            get { return Tables[DimTimeTableName]; }
        }

        public static ReportingWarehouseDimensionsDataSet FromTable(string tableName, DataTable source)
        {
            var dataSet = new ReportingWarehouseDimensionsDataSet();
            dataSet.LoadTable(tableName, source);
            return dataSet;
        }

        public void LoadTable(string tableName, DataTable source)
        {
            if (!Tables.Contains(tableName))
            {
                throw new ArgumentOutOfRangeException(nameof(tableName), tableName, "Unknown reporting dimension table.");
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

        private static DataTable CreateDimStoreTable()
        {
            var table = new DataTable(DimStoreTableName);
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("StoreName", typeof(string));
            table.Columns.Add("RegionName", typeof(string));
            table.Columns.Add("MarketName", typeof(string));
            table.Columns.Add("OpenedDate", typeof(DateTime));
            table.Columns.Add("IsActive", typeof(bool));
            return table;
        }

        private static DataTable CreateDimDriverTable()
        {
            var table = new DataTable(DimDriverTableName);
            table.Columns.Add("DriverKey", typeof(int));
            table.Columns.Add("StoreKey", typeof(int));
            table.Columns.Add("StoreNumber", typeof(string));
            table.Columns.Add("DriverCode", typeof(string));
            table.Columns.Add("DriverName", typeof(string));
            table.Columns.Add("EmploymentStatus", typeof(string));
            table.Columns.Add("ShiftType", typeof(string));
            table.Columns.Add("HireDate", typeof(DateTime));
            table.Columns.Add("IsActive", typeof(bool));
            return table;
        }

        private static DataTable CreateDimPartnerTable()
        {
            var table = new DataTable(DimPartnerTableName);
            table.Columns.Add("PartnerKey", typeof(int));
            table.Columns.Add("PartnerCode", typeof(string));
            table.Columns.Add("PartnerName", typeof(string));
            table.Columns.Add("PartnerTier", typeof(string));
            table.Columns.Add("SettlementModel", typeof(string));
            table.Columns.Add("IsActive", typeof(bool));
            return table;
        }

        private static DataTable CreateDimTimeTable()
        {
            var table = new DataTable(DimTimeTableName);
            table.Columns.Add("DateKey", typeof(int));
            table.Columns.Add("CalendarDate", typeof(DateTime));
            table.Columns.Add("CalendarMonthLabel", typeof(string));
            table.Columns.Add("FiscalQuarter", typeof(string));
            table.Columns.Add("DayName", typeof(string));
            table.Columns.Add("DayPart", typeof(string));
            table.Columns.Add("IsWeekend", typeof(bool));
            return table;
        }
    }
}
