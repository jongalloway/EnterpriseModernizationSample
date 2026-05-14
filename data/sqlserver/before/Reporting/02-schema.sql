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
        OvertimeHours DECIMAL(9,2) NOT NULL,
        LastLoadedUtc DATETIME NOT NULL CONSTRAINT DF_LaborDailySummary_LastLoadedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_LaborDailySummary_Store_Date ON dbo.LaborDailySummary (StoreNumber, SummaryDate);
END
GO

IF OBJECT_ID(N'dbo.PartnerProfitabilitySummary', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PartnerProfitabilitySummary
    (
        PartnerProfitabilitySummaryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        PartnerCode NVARCHAR(20) NOT NULL,
        SummaryDate DATE NOT NULL,
        DeliveredOrders INT NOT NULL,
        GrossSales MONEY NOT NULL,
        FeePercentage DECIMAL(5,2) NOT NULL,
        LastLoadedUtc DATETIME NOT NULL CONSTRAINT DF_PartnerProfitabilitySummary_LastLoadedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_PartnerProfitabilitySummary_Code_Date ON dbo.PartnerProfitabilitySummary (PartnerCode, SummaryDate);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\02-schema.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\02-schema.sql');
END
GO
