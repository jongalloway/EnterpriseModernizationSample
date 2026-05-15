USE [$(ReportingDatabase)];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.ReportingBatchRun', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReportingBatchRun
    (
        ReportingBatchRunId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SyncBatchId UNIQUEIDENTIFIER NULL,
        SummaryDate DATE NOT NULL,
        StartedUtc DATETIME NOT NULL,
        CompletedUtc DATETIME NULL,
        RunStatus NVARCHAR(20) NOT NULL,
        Notes NVARCHAR(200) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.DeliveryDailySummary', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DeliveryDailySummary
    (
        DeliveryDailySummaryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreNumber NVARCHAR(10) NOT NULL,
        SummaryDate DATE NOT NULL,
        CompletedRuns INT NOT NULL,
        LateRuns INT NOT NULL,
        SalesAmount MONEY NOT NULL,
        LastLoadedUtc DATETIME NOT NULL CONSTRAINT DF_DeliveryDailySummary_LastLoadedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_DeliveryDailySummary_Store_Date ON dbo.DeliveryDailySummary (StoreNumber, SummaryDate);
END
GO

IF OBJECT_ID(N'dbo.LaborDailySummary', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LaborDailySummary
    (
        LaborDailySummaryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreNumber NVARCHAR(10) NOT NULL,
        SummaryDate DATE NOT NULL,
        DriverCount INT NOT NULL,
        ExceptionCount INT NOT NULL,
        ScheduledHours DECIMAL(9,2) NOT NULL,
        WorkedHours DECIMAL(9,2) NOT NULL,
        OvertimeHours DECIMAL(9,2) NOT NULL,
        RegularLaborCost MONEY NOT NULL,
        OvertimeLaborCost MONEY NOT NULL,
        AgencyLaborCost MONEY NOT NULL,
        NetSales MONEY NOT NULL,
        LastLoadedUtc DATETIME NOT NULL CONSTRAINT DF_LaborDailySummary_LastLoadedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_LaborDailySummary_Store_Date ON dbo.LaborDailySummary (StoreNumber, SummaryDate);
END
GO

IF OBJECT_ID(N'dbo.LaborOvertimeWeeklySummary', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LaborOvertimeWeeklySummary
    (
        LaborOvertimeWeeklySummaryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreNumber NVARCHAR(10) NOT NULL,
        WeekEndingDate DATE NOT NULL,
        DriverOvertimeHours DECIMAL(9,2) NOT NULL,
        KitchenOvertimeHours DECIMAL(9,2) NOT NULL,
        ShiftLeadOvertimeHours DECIMAL(9,2) NOT NULL,
        LastLoadedUtc DATETIME NOT NULL CONSTRAINT DF_LaborOvertimeWeeklySummary_LastLoadedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_LaborOvertimeWeeklySummary_Store_WeekEndingDate ON dbo.LaborOvertimeWeeklySummary (StoreNumber, WeekEndingDate);
END
GO

IF OBJECT_ID(N'dbo.WorkforceTurnoverMonthlySummary', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WorkforceTurnoverMonthlySummary
    (
        WorkforceTurnoverMonthlySummaryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreNumber NVARCHAR(10) NOT NULL,
        SummaryMonth DATE NOT NULL,
        BeginningHeadcount INT NOT NULL,
        HireCount INT NOT NULL,
        SeparationCount INT NOT NULL,
        EndingHeadcount INT NOT NULL,
        LastLoadedUtc DATETIME NOT NULL CONSTRAINT DF_WorkforceTurnoverMonthlySummary_LastLoadedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_WorkforceTurnoverMonthlySummary_Store_Month ON dbo.WorkforceTurnoverMonthlySummary (StoreNumber, SummaryMonth);
END
GO

IF OBJECT_ID(N'dbo.StaffingDailySummary', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StaffingDailySummary
    (
        StaffingDailySummaryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreNumber NVARCHAR(10) NOT NULL,
        SummaryDate DATE NOT NULL,
        ScheduledDriverSlots INT NOT NULL,
        FilledDriverSlots INT NOT NULL,
        OpenDriverSlots INT NOT NULL,
        CrossTrainedTeamMembers INT NOT NULL,
        CalloutCount INT NOT NULL,
        LastLoadedUtc DATETIME NOT NULL CONSTRAINT DF_StaffingDailySummary_LastLoadedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_StaffingDailySummary_Store_Date ON dbo.StaffingDailySummary (StoreNumber, SummaryDate);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\02-schema.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\02-schema.sql');
END
GO
