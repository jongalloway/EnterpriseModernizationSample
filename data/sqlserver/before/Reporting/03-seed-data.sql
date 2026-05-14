USE [$(ReportingDatabase)];
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ReportingBatchRun)
BEGIN
    INSERT INTO dbo.ReportingBatchRun (SyncBatchId, SummaryDate, StartedUtc, CompletedUtc, RunStatus, Notes)
    VALUES (NULL, CONVERT(DATE, GETUTCDATE()), GETUTCDATE(), GETUTCDATE(), N'Seeded', N'Initial seeded reporting snapshot for demos.');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DeliveryDailySummary)
BEGIN
    INSERT INTO dbo.DeliveryDailySummary (StoreNumber, SummaryDate, CompletedRuns, LateRuns, SalesAmount, LastLoadedUtc)
    VALUES (N'014', CONVERT(DATE, GETUTCDATE()), 42, 4, 381.85, GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.LaborDailySummary)
BEGIN
    INSERT INTO dbo.LaborDailySummary (StoreNumber, SummaryDate, DriverCount, ExceptionCount, OvertimeHours, LastLoadedUtc)
    VALUES (N'014', CONVERT(DATE, GETUTCDATE()), 2, 1, 1.75, GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerProfitabilitySummary)
BEGIN
    INSERT INTO dbo.PartnerProfitabilitySummary (PartnerCode, SummaryDate, DeliveredOrders, GrossSales, FeePercentage, LastLoadedUtc)
    VALUES (N'CORP-1002', CONVERT(DATE, GETUTCDATE()), 12, 624.00, 8.50, GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\03-seed-data.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\03-seed-data.sql');
END
GO
