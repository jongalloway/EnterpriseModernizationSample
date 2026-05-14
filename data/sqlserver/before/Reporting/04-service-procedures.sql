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

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\04-service-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\04-service-procedures.sql');
END
GO
