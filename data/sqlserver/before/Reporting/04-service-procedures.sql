USE [$(ReportingDatabase)];
GO

IF OBJECT_ID(N'dbo.usp_DeliveryDashboard_GetDailySummary', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_DeliveryDashboard_GetDailySummary;
GO

CREATE PROCEDURE dbo.usp_DeliveryDashboard_GetDailySummary
    @StoreNumber NVARCHAR(10),
    @SummaryDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    SELECT
        dds.StoreNumber,
        dds.SummaryDate,
        dds.CompletedRuns,
        dds.LateRuns,
        lds.OvertimeHours AS LaborVariance,
        dds.SalesAmount
    FROM dbo.DeliveryDailySummary dds
    LEFT JOIN dbo.LaborDailySummary lds
        ON lds.StoreNumber = dds.StoreNumber
       AND lds.SummaryDate = dds.SummaryDate
    WHERE dds.StoreNumber = @StoreNumber
      AND dds.SummaryDate = @SummaryDate;
END
GO

IF OBJECT_ID(N'dbo.usp_WorkforceReports_GetLaborCostSummary', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_WorkforceReports_GetLaborCostSummary;
GO

CREATE PROCEDURE dbo.usp_WorkforceReports_GetLaborCostSummary
    @StoreNumber NVARCHAR(10),
    @SummaryDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    SELECT
        lds.StoreNumber,
        lds.SummaryDate,
        lds.ScheduledHours,
        lds.WorkedHours,
        lds.OvertimeHours,
        lds.RegularLaborCost,
        lds.OvertimeLaborCost,
        lds.AgencyLaborCost,
        lds.NetSales,
        CASE
            WHEN lds.NetSales = 0 THEN 0
            ELSE CAST(((lds.RegularLaborCost + lds.OvertimeLaborCost + lds.AgencyLaborCost) / lds.NetSales) * 100.0 AS DECIMAL(9,2))
        END AS LaborCostPercentageOfSales
    FROM dbo.LaborDailySummary lds
    WHERE lds.StoreNumber = @StoreNumber
      AND lds.SummaryDate = @SummaryDate;
END
GO

IF OBJECT_ID(N'dbo.usp_WorkforceReports_GetOvertimeTrend', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_WorkforceReports_GetOvertimeTrend;
GO

CREATE PROCEDURE dbo.usp_WorkforceReports_GetOvertimeTrend
    @StoreNumber NVARCHAR(10),
    @WeeksBack INT = 4
AS
BEGIN
    SET NOCOUNT ON;

    IF @WeeksBack IS NULL OR @WeeksBack < 1
    BEGIN
        SET @WeeksBack = 4;
    END

    SELECT TOP (@WeeksBack)
        lows.StoreNumber,
        lows.WeekEndingDate,
        lows.DriverOvertimeHours,
        lows.KitchenOvertimeHours,
        lows.ShiftLeadOvertimeHours,
        lows.DriverOvertimeHours + lows.KitchenOvertimeHours + lows.ShiftLeadOvertimeHours AS TotalOvertimeHours
    FROM dbo.LaborOvertimeWeeklySummary lows
    WHERE lows.StoreNumber = @StoreNumber
    ORDER BY lows.WeekEndingDate DESC;
END
GO

IF OBJECT_ID(N'dbo.usp_WorkforceReports_GetTurnoverSummary', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_WorkforceReports_GetTurnoverSummary;
GO

CREATE PROCEDURE dbo.usp_WorkforceReports_GetTurnoverSummary
    @StoreNumber NVARCHAR(10),
    @SummaryMonth DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryMonth IS NULL
    BEGIN
        SET @SummaryMonth = DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1);
    END

    SELECT
        wtms.StoreNumber,
        wtms.SummaryMonth,
        wtms.BeginningHeadcount,
        wtms.HireCount,
        wtms.SeparationCount,
        wtms.EndingHeadcount,
        CASE
            WHEN wtms.BeginningHeadcount = 0 THEN 0
            ELSE CAST((wtms.SeparationCount * 100.0) / wtms.BeginningHeadcount AS DECIMAL(9,2))
        END AS TurnoverRate
    FROM dbo.WorkforceTurnoverMonthlySummary wtms
    WHERE wtms.StoreNumber = @StoreNumber
      AND wtms.SummaryMonth = @SummaryMonth;
END
GO

IF OBJECT_ID(N'dbo.usp_WorkforceReports_GetStaffingSummary', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_WorkforceReports_GetStaffingSummary;
GO

CREATE PROCEDURE dbo.usp_WorkforceReports_GetStaffingSummary
    @StoreNumber NVARCHAR(10),
    @SummaryDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    SELECT
        sds.StoreNumber,
        sds.SummaryDate,
        sds.ScheduledDriverSlots,
        sds.FilledDriverSlots,
        sds.OpenDriverSlots,
        sds.CrossTrainedTeamMembers,
        sds.CalloutCount,
        CASE
            WHEN sds.ScheduledDriverSlots = 0 THEN 0
            ELSE CAST((sds.FilledDriverSlots * 100.0) / sds.ScheduledDriverSlots AS DECIMAL(9,2))
        END AS StaffingCoverageRate
    FROM dbo.StaffingDailySummary sds
    WHERE sds.StoreNumber = @StoreNumber
      AND sds.SummaryDate = @SummaryDate;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\04-service-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\04-service-procedures.sql');
END
GO
