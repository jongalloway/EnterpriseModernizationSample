using System;
using System.Data;

namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    internal static class CustomerHubDataRecordReader
    {
        public static string GetString(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? null : Convert.ToString(row[columnName]);
        }

        public static int GetInt32(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? 0 : Convert.ToInt32(row[columnName]);
        }

        public static int? GetNullableInt32(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? (int?)null : Convert.ToInt32(row[columnName]);
        }

        public static short GetInt16(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? (short)0 : Convert.ToInt16(row[columnName]);
        }

        public static bool GetBoolean(DataRow row, string columnName)
        {
            return HasValue(row, columnName) && Convert.ToBoolean(row[columnName]);
        }

        public static decimal GetDecimal(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? 0m : Convert.ToDecimal(row[columnName]);
        }

        public static DateTime GetDateTime(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? DateTime.MinValue : Convert.ToDateTime(row[columnName]);
        }

        public static DateTime? GetNullableDateTime(DataRow row, string columnName)
        {
            return !HasValue(row, columnName) ? (DateTime?)null : Convert.ToDateTime(row[columnName]);
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
