using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fabrikam.EnterprisePizza.Core.Domain.Reporting;
using Fabrikam.EnterprisePizza.Data.Configuration;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.StoredProcedures;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public class WorkforceReportRepository : IWorkforceReportRepository
    {
        private readonly LegacyDbGateway gateway;
        private readonly LegacyConnectionCatalog connectionCatalog;

        public WorkforceReportRepository(LegacyDbGateway gateway)
            : this(gateway, new LegacyConnectionCatalog())
        {
        }

        public WorkforceReportRepository(LegacyDbGateway gateway, LegacyConnectionCatalog connectionCatalog)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this.connectionCatalog = connectionCatalog ?? throw new ArgumentNullException(nameof(connectionCatalog));
        }

        public LaborCostSummary GetLaborCostSummary(string storeNumber, DateTime summaryDate)
        {
            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.Reporting),
                    LegacyStoredProcedures.Reporting.GetLaborCostSummary,
                    new GatewayParameter("@StoreNumber", storeNumber),
                    new GatewayParameter("@SummaryDate", summaryDate)));

            var row = GetFirstRow(dataSet);
            return new LaborCostSummary
            {
                StoreNumber = GetString(row, "StoreNumber"),
                SummaryDate = GetDateTime(row, "SummaryDate"),
                ScheduledHours = GetDecimal(row, "ScheduledHours"),
                WorkedHours = GetDecimal(row, "WorkedHours"),
                OvertimeHours = GetDecimal(row, "OvertimeHours"),
                RegularLaborCost = GetDecimal(row, "RegularLaborCost"),
                OvertimeLaborCost = GetDecimal(row, "OvertimeLaborCost"),
                AgencyLaborCost = GetDecimal(row, "AgencyLaborCost"),
                NetSales = GetDecimal(row, "NetSales"),
                LaborCostPercentageOfSales = GetDecimal(row, "LaborCostPercentageOfSales")
            };
        }

        public IList<OvertimeTrendPoint> GetOvertimeTrend(string storeNumber, int weeksBack)
        {
            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.Reporting),
                    LegacyStoredProcedures.Reporting.GetOvertimeTrend,
                    new GatewayParameter("@StoreNumber", storeNumber),
                    new GatewayParameter("@WeeksBack", weeksBack)));

            var items = new List<OvertimeTrendPoint>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                items.Add(new OvertimeTrendPoint
                {
                    StoreNumber = GetString(row, "StoreNumber"),
                    WeekEndingDate = GetDateTime(row, "WeekEndingDate"),
                    DriverOvertimeHours = GetDecimal(row, "DriverOvertimeHours"),
                    KitchenOvertimeHours = GetDecimal(row, "KitchenOvertimeHours"),
                    ShiftLeadOvertimeHours = GetDecimal(row, "ShiftLeadOvertimeHours"),
                    TotalOvertimeHours = GetDecimal(row, "TotalOvertimeHours")
                });
            }

            return items;
        }

        public TurnoverSummary GetTurnoverSummary(string storeNumber, DateTime summaryMonth)
        {
            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.Reporting),
                    LegacyStoredProcedures.Reporting.GetTurnoverSummary,
                    new GatewayParameter("@StoreNumber", storeNumber),
                    new GatewayParameter("@SummaryMonth", summaryMonth)));

            var row = GetFirstRow(dataSet);
            return new TurnoverSummary
            {
                StoreNumber = GetString(row, "StoreNumber"),
                SummaryMonth = GetDateTime(row, "SummaryMonth"),
                BeginningHeadcount = GetInt32(row, "BeginningHeadcount"),
                HireCount = GetInt32(row, "HireCount"),
                SeparationCount = GetInt32(row, "SeparationCount"),
                EndingHeadcount = GetInt32(row, "EndingHeadcount"),
                TurnoverRate = GetDecimal(row, "TurnoverRate")
            };
        }

        public StaffingSummary GetStaffingSummary(string storeNumber, DateTime summaryDate)
        {
            var dataSet = gateway.ExecuteDataSet(
                new StoredProcedureCall(
                    connectionCatalog.GetConnectionName(LegacyDatabaseArea.Reporting),
                    LegacyStoredProcedures.Reporting.GetStaffingSummary,
                    new GatewayParameter("@StoreNumber", storeNumber),
                    new GatewayParameter("@SummaryDate", summaryDate)));

            var row = GetFirstRow(dataSet);
            return new StaffingSummary
            {
                StoreNumber = GetString(row, "StoreNumber"),
                SummaryDate = GetDateTime(row, "SummaryDate"),
                ScheduledDriverSlots = GetInt32(row, "ScheduledDriverSlots"),
                FilledDriverSlots = GetInt32(row, "FilledDriverSlots"),
                OpenDriverSlots = GetInt32(row, "OpenDriverSlots"),
                CrossTrainedTeamMembers = GetInt32(row, "CrossTrainedTeamMembers"),
                CalloutCount = GetInt32(row, "CalloutCount"),
                StaffingCoverageRate = GetDecimal(row, "StaffingCoverageRate")
            };
        }

        private static DataRow GetFirstRow(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                throw new InvalidOperationException("The legacy reporting gateway returned no rows.");
            }

            return dataSet.Tables[0].Rows[0];
        }

        private static string GetString(DataRow row, string columnName)
        {
            return Convert.ToString(row[columnName]);
        }

        private static DateTime GetDateTime(DataRow row, string columnName)
        {
            return Convert.ToDateTime(row[columnName]);
        }

        private static decimal GetDecimal(DataRow row, string columnName)
        {
            return Convert.ToDecimal(row[columnName]);
        }

        private static int GetInt32(DataRow row, string columnName)
        {
            return Convert.ToInt32(row[columnName]);
        }
    }
}
