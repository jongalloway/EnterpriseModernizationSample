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
    VALUES (N'014', CONVERT(DATE, GETUTCDATE()), 2, 1, 164.00, 171.50, 7.50, 2448.00, 213.75, 96.00, 8200.00, GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.LaborOvertimeWeeklySummary)
BEGIN
    INSERT INTO dbo.LaborOvertimeWeeklySummary
    (
        StoreNumber,
        WeekEndingDate,
        DriverOvertimeHours,
        KitchenOvertimeHours,
        ShiftLeadOvertimeHours,
        LastLoadedUtc
    )
    VALUES
        (N'014', DATEADD(DAY, -21, CONVERT(DATE, GETUTCDATE())), 5.00, 2.00, 1.00, GETUTCDATE()),
        (N'014', DATEADD(DAY, -14, CONVERT(DATE, GETUTCDATE())), 4.50, 2.50, 1.25, GETUTCDATE()),
        (N'014', DATEADD(DAY, -7, CONVERT(DATE, GETUTCDATE())), 5.75, 2.75, 1.50, GETUTCDATE()),
        (N'014', CONVERT(DATE, GETUTCDATE()), 6.00, 3.00, 1.50, GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.WorkforceTurnoverMonthlySummary)
BEGIN
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
    VALUES (N'014', DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1), 27, 3, 2, 28, GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.StaffingDailySummary)
BEGIN
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
    VALUES (N'014', CONVERT(DATE, GETUTCDATE()), 18, 15, 3, 2, 1, GETUTCDATE());
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
