USE [$(ReportingDatabase)];
GO

IF OBJECT_ID(N'dbo.usp_Reporting_LoadStoreOpsDailySummary', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Reporting_LoadStoreOpsDailySummary;
GO

CREATE PROCEDURE dbo.usp_Reporting_LoadStoreOpsDailySummary
    @SummaryDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    DELETE FROM dbo.DeliveryDailySummary
    WHERE SummaryDate = @SummaryDate;

    INSERT INTO dbo.DeliveryDailySummary
    (
        StoreNumber,
        SummaryDate,
        CompletedRuns,
        LateRuns,
        SalesAmount,
        LastLoadedUtc
    )
    SELECT
        s.StoreNumber,
        @SummaryDate,
        SUM(CASE WHEN dt.StatusCode IN (N'Dispatched', N'CompletedLate') THEN 1 ELSE 0 END),
        SUM(CASE WHEN dt.StatusCode = N'CompletedLate' THEN 1 ELSE 0 END),
        SUM(dt.TotalAmount),
        GETUTCDATE()
    FROM [$(StoreOpsDatabase)].dbo.DispatchTicket dt
    INNER JOIN [$(StoreOpsDatabase)].dbo.Store s
        ON s.StoreId = dt.StoreId
    WHERE CONVERT(DATE, dt.PromiseUtc) = @SummaryDate
    GROUP BY s.StoreNumber;

    DELETE FROM dbo.LaborDailySummary
    WHERE SummaryDate = @SummaryDate;

    INSERT INTO dbo.LaborDailySummary
    (
        StoreNumber,
        SummaryDate,
        DriverCount,
        ExceptionCount,
        ScheduledHours,
        WorkedHours,
        OvertimeHours,
        RegularLaborCost,
        OvertimeLaborCost,
        AgencyLaborCost,
        NetSales,
        LastLoadedUtc
    )
    SELECT
        s.StoreNumber,
        @SummaryDate,
        ISNULL(driverSummary.DriverCount, 0),
        ISNULL(driverSummary.ExceptionCount, 0),
        pdi.ScheduledHours,
        pdi.WorkedHours,
        pdi.OvertimeHours,
        pdi.RegularLaborCost,
        pdi.OvertimeLaborCost,
        pdi.AgencyLaborCost,
        pdi.NetSales,
        GETUTCDATE()
    FROM [$(StoreOpsDatabase)].dbo.Store s
    INNER JOIN [$(StoreOpsDatabase)].dbo.PayrollDailyImport pdi
        ON pdi.StoreId = s.StoreId
       AND pdi.WorkDate = @SummaryDate
    LEFT JOIN
    (
        SELECT
            d.StoreId,
            COUNT(*) AS DriverCount,
            SUM(CASE WHEN d.EligibilityExpirationDate < DATEADD(DAY, 30, GETUTCDATE()) THEN 1 ELSE 0 END) AS ExceptionCount
        FROM [$(StoreOpsDatabase)].dbo.Driver d
        GROUP BY d.StoreId
    ) driverSummary
        ON driverSummary.StoreId = s.StoreId;

    DELETE FROM dbo.StaffingDailySummary
    WHERE SummaryDate = @SummaryDate;

    INSERT INTO dbo.StaffingDailySummary
    (
        StoreNumber,
        SummaryDate,
        ScheduledDriverSlots,
        FilledDriverSlots,
        OpenDriverSlots,
        CrossTrainedTeamMembers,
        CalloutCount,
        LastLoadedUtc
    )
    SELECT
        s.StoreNumber,
        @SummaryDate,
        pdi.ScheduledDriverSlots,
        pdi.FilledDriverSlots,
        pdi.OpenDriverSlots,
        pdi.CrossTrainedTeamMembers,
        pdi.CalloutCount,
        GETUTCDATE()
    FROM [$(StoreOpsDatabase)].dbo.Store s
    INNER JOIN [$(StoreOpsDatabase)].dbo.PayrollDailyImport pdi
        ON pdi.StoreId = s.StoreId
       AND pdi.WorkDate = @SummaryDate;

    DELETE target
    FROM dbo.LaborOvertimeWeeklySummary target
    INNER JOIN [$(StoreOpsDatabase)].dbo.PayrollOvertimeWeeklyImport source
        ON source.WeekEndingDate = target.WeekEndingDate
    INNER JOIN [$(StoreOpsDatabase)].dbo.Store storeMap
        ON storeMap.StoreId = source.StoreId
       AND storeMap.StoreNumber = target.StoreNumber;

    INSERT INTO dbo.LaborOvertimeWeeklySummary
    (
        StoreNumber,
        WeekEndingDate,
        DriverOvertimeHours,
        KitchenOvertimeHours,
        ShiftLeadOvertimeHours,
        LastLoadedUtc
    )
    SELECT
        s.StoreNumber,
        powi.WeekEndingDate,
        powi.DriverOvertimeHours,
        powi.KitchenOvertimeHours,
        powi.ShiftLeadOvertimeHours,
        GETUTCDATE()
    FROM [$(StoreOpsDatabase)].dbo.PayrollOvertimeWeeklyImport powi
    INNER JOIN [$(StoreOpsDatabase)].dbo.Store s
        ON s.StoreId = powi.StoreId;

    DELETE target
    FROM dbo.WorkforceTurnoverMonthlySummary target
    INNER JOIN [$(StoreOpsDatabase)].dbo.PayrollTurnoverMonthlyImport source
        ON source.SummaryMonth = target.SummaryMonth
    INNER JOIN [$(StoreOpsDatabase)].dbo.Store storeMap
        ON storeMap.StoreId = source.StoreId
       AND storeMap.StoreNumber = target.StoreNumber;

    INSERT INTO dbo.WorkforceTurnoverMonthlySummary
    (
        StoreNumber,
        SummaryMonth,
        BeginningHeadcount,
        HireCount,
        SeparationCount,
        EndingHeadcount,
        LastLoadedUtc
    )
    SELECT
        s.StoreNumber,
        ptmi.SummaryMonth,
        ptmi.BeginningHeadcount,
        ptmi.HireCount,
        ptmi.SeparationCount,
        ptmi.EndingHeadcount,
        GETUTCDATE()
    FROM [$(StoreOpsDatabase)].dbo.PayrollTurnoverMonthlyImport ptmi
    INNER JOIN [$(StoreOpsDatabase)].dbo.Store s
        ON s.StoreId = ptmi.StoreId;
END
GO

IF OBJECT_ID(N'dbo.usp_Reporting_LoadCustomerHubPartnerSummary', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Reporting_LoadCustomerHubPartnerSummary;
GO

CREATE PROCEDURE dbo.usp_Reporting_LoadCustomerHubPartnerSummary
    @SummaryDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    DELETE FROM dbo.PartnerProfitabilitySummary
    WHERE SummaryDate = @SummaryDate;

    INSERT INTO dbo.PartnerProfitabilitySummary
    (
        PartnerCode,
        SummaryDate,
        DeliveredOrders,
        GrossSales,
        FeePercentage,
        LastLoadedUtc
    )
    SELECT
        pa.PartnerCode,
        @SummaryDate,
        CASE pa.RelationshipTier
            WHEN N'Gold' THEN 12
            WHEN N'Community' THEN 7
            ELSE 4
        END,
        CASE pa.RelationshipTier
            WHEN N'Gold' THEN 624.00
            WHEN N'Community' THEN 287.00
            ELSE 198.00
        END,
        CASE pa.RelationshipTier
            WHEN N'Gold' THEN 8.50
            WHEN N'Community' THEN 5.00
            ELSE 11.00
        END,
        GETUTCDATE()
    FROM [$(CustomerHubDatabase)].dbo.PartnerAccount pa
    WHERE pa.StatusCode = N'Active';
END
GO

IF OBJECT_ID(N'dbo.usp_Reporting_NightlyRebuild', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Reporting_NightlyRebuild;
GO

CREATE PROCEDURE dbo.usp_Reporting_NightlyRebuild
    @SummaryDate DATE = NULL,
    @SyncBatchId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    INSERT INTO dbo.ReportingBatchRun (SyncBatchId, SummaryDate, StartedUtc, RunStatus, Notes)
    VALUES (@SyncBatchId, @SummaryDate, GETUTCDATE(), N'Running', N'Nightly rebuild started by deployment orchestration.');

    DECLARE @ReportingBatchRunId INT;
    SET @ReportingBatchRunId = SCOPE_IDENTITY();

    EXEC dbo.usp_Reporting_LoadStoreOpsDailySummary @SummaryDate = @SummaryDate;
    EXEC dbo.usp_Reporting_LoadCustomerHubPartnerSummary @SummaryDate = @SummaryDate;

    UPDATE dbo.ReportingBatchRun
    SET CompletedUtc = GETUTCDATE(),
        RunStatus = N'Complete',
        Notes = N'Nightly rebuild refreshed delivery, labor, overtime, turnover, staffing, and partner summaries.'
    WHERE ReportingBatchRunId = @ReportingBatchRunId;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\05-migration-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\05-migration-procedures.sql');
END
GO
