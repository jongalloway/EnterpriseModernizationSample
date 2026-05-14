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
    GROUP BY s.StoreNumber;

    DELETE FROM dbo.LaborDailySummary
    WHERE SummaryDate = @SummaryDate;

    INSERT INTO dbo.LaborDailySummary
    (
        StoreNumber,
        SummaryDate,
        DriverCount,
        ExceptionCount,
        OvertimeHours,
        LastLoadedUtc
    )
    SELECT
        s.StoreNumber,
        @SummaryDate,
        ISNULL(driverSummary.DriverCount, 0),
        ISNULL(driverSummary.ExceptionCount, 0),
        CAST(ISNULL(ticketSummary.TicketCount, 0) * 0.25 AS DECIMAL(9,2)),
        GETUTCDATE()
    FROM [$(StoreOpsDatabase)].dbo.Store s
    LEFT JOIN
    (
        SELECT
            d.StoreId,
            COUNT(*) AS DriverCount,
            SUM(CASE WHEN d.EligibilityExpirationDate < DATEADD(DAY, 30, GETUTCDATE()) THEN 1 ELSE 0 END) AS ExceptionCount
        FROM [$(StoreOpsDatabase)].dbo.Driver d
        GROUP BY d.StoreId
    ) driverSummary
        ON driverSummary.StoreId = s.StoreId
    LEFT JOIN
    (
        SELECT
            dt.StoreId,
            COUNT(*) AS TicketCount
        FROM [$(StoreOpsDatabase)].dbo.DispatchTicket dt
        GROUP BY dt.StoreId
    ) ticketSummary
        ON ticketSummary.StoreId = s.StoreId
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
        RunStatus = N'Complete'
    WHERE ReportingBatchRunId = @ReportingBatchRunId;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\05-migration-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\05-migration-procedures.sql');
END
GO
