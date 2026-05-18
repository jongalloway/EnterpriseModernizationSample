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

IF OBJECT_ID(N'dbo.usp_DeliveryPerformanceReport_GetSummary', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_DeliveryPerformanceReport_GetSummary;
GO

CREATE PROCEDURE dbo.usp_DeliveryPerformanceReport_GetSummary
    @StoreNumber NVARCHAR(10),
    @SummaryDate DATE = NULL,
    @MinimumCompletedRuns INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    IF @MinimumCompletedRuns IS NULL OR @MinimumCompletedRuns < 0
    BEGIN
        SET @MinimumCompletedRuns = 0;
    END

    SELECT
        dds.StoreNumber,
        dds.SummaryDate,
        dds.CompletedRuns,
        dds.LateRuns,
        CASE
            WHEN dds.CompletedRuns = 0 THEN 0
            ELSE CAST(((dds.CompletedRuns - dds.LateRuns) * 100.0) / dds.CompletedRuns AS DECIMAL(9,2))
        END AS OnTimePercentage,
        CASE
            WHEN ISNULL(lds.WorkedHours, 0) = 0 THEN 0
            ELSE CAST(dds.CompletedRuns / lds.WorkedHours AS DECIMAL(9,2))
        END AS RouteEfficiencyScore,
        ISNULL(lds.DriverCount, 0) AS DriverCount,
        ISNULL(lds.WorkedHours, 0) AS WorkedHours,
        dds.SalesAmount
    FROM dbo.DeliveryDailySummary dds
    LEFT JOIN dbo.LaborDailySummary lds
        ON lds.StoreNumber = dds.StoreNumber
       AND lds.SummaryDate = dds.SummaryDate
    WHERE dds.StoreNumber = @StoreNumber
      AND dds.SummaryDate = @SummaryDate
      AND dds.CompletedRuns >= @MinimumCompletedRuns;
END
GO

IF OBJECT_ID(N'dbo.usp_StoreOperationsSummaryReport_GetRollup', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_StoreOperationsSummaryReport_GetRollup;
GO

CREATE PROCEDURE dbo.usp_StoreOperationsSummaryReport_GetRollup
    @StoreNumber NVARCHAR(10),
    @StartDate DATE = NULL,
    @EndDate DATE = NULL,
    @RollupMode NVARCHAR(10) = N'Daily'
AS
BEGIN
    SET NOCOUNT ON;

    IF @EndDate IS NULL
    BEGIN
        SET @EndDate = CONVERT(DATE, GETUTCDATE());
    END

    IF @StartDate IS NULL
    BEGIN
        SET @StartDate = DATEADD(DAY, -6, @EndDate);
    END

    IF @StartDate > @EndDate
    BEGIN
        DECLARE @SwapDate DATE = @StartDate;
        SET @StartDate = @EndDate;
        SET @EndDate = @SwapDate;
    END

    IF @RollupMode IS NULL OR UPPER(@RollupMode) NOT IN (N'DAILY', N'WEEKLY')
    BEGIN
        SET @RollupMode = N'Daily';
    END

    ;WITH SummarySource AS
    (
        SELECT
            dds.StoreNumber,
            CASE
                WHEN UPPER(@RollupMode) = N'WEEKLY' THEN DATEADD(DAY, 1 - DATEPART(WEEKDAY, dds.SummaryDate), dds.SummaryDate)
                ELSE dds.SummaryDate
            END AS PeriodStart,
            dds.CompletedRuns,
            dds.LateRuns,
            ISNULL(lds.DriverCount, 0) AS DriverCount,
            ISNULL(lds.ExceptionCount, 0) AS ExceptionCount,
            ISNULL(lds.WorkedHours, 0) AS WorkedHours,
            ISNULL(lds.NetSales, dds.SalesAmount) AS NetSales,
            CASE
                WHEN ISNULL(sds.ScheduledDriverSlots, 0) = 0 THEN 0
                ELSE CAST((sds.FilledDriverSlots * 100.0) / sds.ScheduledDriverSlots AS DECIMAL(9,2))
            END AS StaffingCoveragePercentage
        FROM dbo.DeliveryDailySummary dds
        LEFT JOIN dbo.LaborDailySummary lds
            ON lds.StoreNumber = dds.StoreNumber
           AND lds.SummaryDate = dds.SummaryDate
        LEFT JOIN dbo.StaffingDailySummary sds
            ON sds.StoreNumber = dds.StoreNumber
           AND sds.SummaryDate = dds.SummaryDate
        WHERE dds.StoreNumber = @StoreNumber
          AND dds.SummaryDate BETWEEN @StartDate AND @EndDate
    )
    SELECT
        StoreNumber,
        PeriodStart,
        SUM(CompletedRuns) AS CompletedRuns,
        SUM(LateRuns) AS LateRuns,
        SUM(DriverCount) AS DriverCount,
        SUM(ExceptionCount) AS ExceptionCount,
        SUM(WorkedHours) AS WorkedHours,
        SUM(NetSales) AS NetSales,
        CAST(AVG(StaffingCoveragePercentage) AS DECIMAL(9,2)) AS StaffingCoveragePercentage
    FROM SummarySource
    GROUP BY StoreNumber, PeriodStart
    ORDER BY PeriodStart DESC;
END
GO

IF OBJECT_ID(N'dbo.usp_PartnerProfitabilityReport_GetSummary', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_PartnerProfitabilityReport_GetSummary;
GO

CREATE PROCEDURE dbo.usp_PartnerProfitabilityReport_GetSummary
    @SummaryDate DATE = NULL,
    @PartnerCode NVARCHAR(25) = N'ALL',
    @MinimumGrossSales MONEY = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    IF @PartnerCode IS NULL OR LTRIM(RTRIM(@PartnerCode)) = N''
    BEGIN
        SET @PartnerCode = N'ALL';
    END

    IF @MinimumGrossSales IS NULL OR @MinimumGrossSales < 0
    BEGIN
        SET @MinimumGrossSales = 0;
    END

    SELECT
        pps.PartnerCode,
        pps.SummaryDate,
        pps.DeliveredOrders,
        pps.GrossSales,
        pps.FeePercentage,
        CAST((pps.GrossSales * pps.FeePercentage) / 100.0 AS MONEY) AS CommissionPayout,
        CAST(pps.GrossSales - ((pps.GrossSales * pps.FeePercentage) / 100.0) AS MONEY) AS NetRevenue
    FROM dbo.PartnerProfitabilitySummary pps
    WHERE pps.SummaryDate = @SummaryDate
      AND (UPPER(@PartnerCode) IN (N'ALL', N'*') OR pps.PartnerCode = @PartnerCode)
      AND pps.GrossSales >= @MinimumGrossSales
    ORDER BY pps.GrossSales DESC, pps.PartnerCode;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\04-service-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\04-service-procedures.sql');
END
GO
