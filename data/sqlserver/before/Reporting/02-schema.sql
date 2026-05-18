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

IF OBJECT_ID(N'dbo.DimStore', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DimStore
    (
        StoreKey INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SourceStoreId INT NULL,
        StoreNumber NVARCHAR(10) NOT NULL,
        StoreName NVARCHAR(120) NOT NULL,
        RegionCode NVARCHAR(20) NOT NULL,
        DistrictName NVARCHAR(60) NULL,
        MarketName NVARCHAR(80) NULL,
        ActiveFlag BIT NOT NULL,
        LastRefreshedUtc DATETIME NOT NULL
    );

    CREATE UNIQUE INDEX UX_DimStore_StoreNumber ON dbo.DimStore (StoreNumber);
END
GO

IF OBJECT_ID(N'dbo.DimDriver', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DimDriver
    (
        DriverKey INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SourceDriverId INT NULL,
        DriverCode NVARCHAR(20) NOT NULL,
        DriverName NVARCHAR(100) NOT NULL,
        EmploymentStatus NVARCHAR(20) NOT NULL,
        HomeZoneCode NVARCHAR(20) NULL,
        HomeStoreNumber NVARCHAR(10) NOT NULL,
        EligibilityExpirationDate DATETIME NULL,
        ActiveFlag BIT NOT NULL,
        LastRefreshedUtc DATETIME NOT NULL
    );

    CREATE UNIQUE INDEX UX_DimDriver_DriverCode ON dbo.DimDriver (DriverCode);
END
GO

IF OBJECT_ID(N'dbo.DimPartner', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DimPartner
    (
        PartnerKey INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SourcePartnerAccountId INT NULL,
        SourcePartnerContractId INT NULL,
        PartnerCode NVARCHAR(20) NOT NULL,
        PartnerName NVARCHAR(120) NOT NULL,
        RelationshipTier NVARCHAR(20) NOT NULL,
        PreferredStoreNumber NVARCHAR(10) NULL,
        ContractCode NVARCHAR(30) NULL,
        PricingScheduleName NVARCHAR(80) NULL,
        ReferralChannel NVARCHAR(40) NULL,
        DiscountPercentage DECIMAL(5,2) NOT NULL,
        StatusCode NVARCHAR(20) NOT NULL,
        CreditHoldFlag BIT NOT NULL,
        LastRefreshedUtc DATETIME NOT NULL
    );

    CREATE UNIQUE INDEX UX_DimPartner_PartnerCode ON dbo.DimPartner (PartnerCode);
END
GO

IF OBJECT_ID(N'dbo.DimTime', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DimTime
    (
        TimeKey INT NOT NULL PRIMARY KEY,
        FullDate DATE NOT NULL,
        DayName NVARCHAR(20) NOT NULL,
        DayOfWeekNumber TINYINT NOT NULL,
        WeekEndingDate DATE NOT NULL,
        MonthStartDate DATE NOT NULL,
        MonthName NVARCHAR(20) NOT NULL,
        MonthNumber TINYINT NOT NULL,
        QuarterNumber TINYINT NOT NULL,
        CalendarYear SMALLINT NOT NULL,
        IsWeekend BIT NOT NULL
    );

    CREATE UNIQUE INDEX UX_DimTime_FullDate ON dbo.DimTime (FullDate);
END
GO

IF OBJECT_ID(N'dbo.DimProduct', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DimProduct
    (
        ProductKey INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ProductCode NVARCHAR(30) NOT NULL,
        ProductName NVARCHAR(120) NOT NULL,
        ProductCategory NVARCHAR(40) NOT NULL,
        SizeName NVARCHAR(20) NOT NULL,
        CrustStyle NVARCHAR(40) NOT NULL,
        BasePrice MONEY NOT NULL,
        ActiveFlag BIT NOT NULL,
        LimitedTimeFlag BIT NOT NULL,
        LastRefreshedUtc DATETIME NOT NULL
    );

    CREATE UNIQUE INDEX UX_DimProduct_ProductCode ON dbo.DimProduct (ProductCode);
END
GO

IF OBJECT_ID(N'dbo.FactOrder', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FactOrder
    (
        FactOrderId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TimeKey INT NOT NULL,
        StoreKey INT NOT NULL,
        ProductKey INT NOT NULL,
        PartnerKey INT NULL,
        OrderNumber INT NOT NULL,
        SalesChannel NVARCHAR(20) NOT NULL,
        ServiceMode NVARCHAR(20) NOT NULL,
        OrderCount INT NOT NULL,
        ItemQuantity INT NOT NULL,
        GrossSalesAmount MONEY NOT NULL,
        DiscountAmount MONEY NOT NULL,
        NetSalesAmount MONEY NOT NULL,
        LoadedUtc DATETIME NOT NULL
    );

    CREATE UNIQUE INDEX UX_FactOrder_OrderNumber_ProductKey ON dbo.FactOrder (OrderNumber, ProductKey);
END
GO

IF OBJECT_ID(N'dbo.FactDelivery', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FactDelivery
    (
        FactDeliveryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TimeKey INT NOT NULL,
        StoreKey INT NOT NULL,
        DriverKey INT NULL,
        PartnerKey INT NULL,
        OrderNumber INT NULL,
        TicketNumber INT NOT NULL,
        DeliveryStatusCode NVARCHAR(20) NOT NULL,
        DeliveryCount INT NOT NULL,
        DeliveryMinutes SMALLINT NOT NULL,
        PromiseWindowMinutes SMALLINT NOT NULL,
        LateDeliveryCount INT NOT NULL,
        NetSalesAmount MONEY NOT NULL,
        LoadedUtc DATETIME NOT NULL
    );

    CREATE UNIQUE INDEX UX_FactDelivery_TicketNumber ON dbo.FactDelivery (TicketNumber);
END
GO

IF OBJECT_ID(N'dbo.FactPartnerRevenue', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FactPartnerRevenue
    (
        FactPartnerRevenueId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TimeKey INT NOT NULL,
        StoreKey INT NOT NULL,
        PartnerKey INT NOT NULL,
        DeliveredOrders INT NOT NULL,
        GrossSalesAmount MONEY NOT NULL,
        DiscountAmount MONEY NOT NULL,
        NetSalesAmount MONEY NOT NULL,
        PartnerFeePercentage DECIMAL(5,2) NOT NULL,
        PartnerFeeAmount MONEY NOT NULL,
        SettlementAmount MONEY NOT NULL,
        LoadedUtc DATETIME NOT NULL
    );

    CREATE UNIQUE INDEX UX_FactPartnerRevenue_Time_Store_Partner ON dbo.FactPartnerRevenue (TimeKey, StoreKey, PartnerKey);
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
        LastLoadedUtc DATETIME NOT NULL
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
        LastLoadedUtc DATETIME NOT NULL
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
        LastLoadedUtc DATETIME NOT NULL
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
        LastLoadedUtc DATETIME NOT NULL
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
        LastLoadedUtc DATETIME NOT NULL
    );

    CREATE UNIQUE INDEX UX_StaffingDailySummary_Store_Date ON dbo.StaffingDailySummary (StoreNumber, SummaryDate);
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
        LastLoadedUtc DATETIME NOT NULL
        PartnerCode NVARCHAR(25) NOT NULL,
        SummaryDate DATE NOT NULL,
        DeliveredOrders INT NOT NULL,
        GrossSales MONEY NOT NULL,
        FeePercentage DECIMAL(9,2) NOT NULL,
        LastLoadedUtc DATETIME NOT NULL CONSTRAINT DF_PartnerProfitabilitySummary_LastLoadedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_PartnerProfitabilitySummary_Partner_Date ON dbo.PartnerProfitabilitySummary (PartnerCode, SummaryDate);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactOrder_DimTime')
BEGIN
    ALTER TABLE dbo.FactOrder
        ADD CONSTRAINT FK_FactOrder_DimTime
        FOREIGN KEY (TimeKey)
        REFERENCES dbo.DimTime (TimeKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactOrder_DimStore')
BEGIN
    ALTER TABLE dbo.FactOrder
        ADD CONSTRAINT FK_FactOrder_DimStore
        FOREIGN KEY (StoreKey)
        REFERENCES dbo.DimStore (StoreKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactOrder_DimProduct')
BEGIN
    ALTER TABLE dbo.FactOrder
        ADD CONSTRAINT FK_FactOrder_DimProduct
        FOREIGN KEY (ProductKey)
        REFERENCES dbo.DimProduct (ProductKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactOrder_DimPartner')
BEGIN
    ALTER TABLE dbo.FactOrder
        ADD CONSTRAINT FK_FactOrder_DimPartner
        FOREIGN KEY (PartnerKey)
        REFERENCES dbo.DimPartner (PartnerKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactDelivery_DimTime')
BEGIN
    ALTER TABLE dbo.FactDelivery
        ADD CONSTRAINT FK_FactDelivery_DimTime
        FOREIGN KEY (TimeKey)
        REFERENCES dbo.DimTime (TimeKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactDelivery_DimStore')
BEGIN
    ALTER TABLE dbo.FactDelivery
        ADD CONSTRAINT FK_FactDelivery_DimStore
        FOREIGN KEY (StoreKey)
        REFERENCES dbo.DimStore (StoreKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactDelivery_DimDriver')
BEGIN
    ALTER TABLE dbo.FactDelivery
        ADD CONSTRAINT FK_FactDelivery_DimDriver
        FOREIGN KEY (DriverKey)
        REFERENCES dbo.DimDriver (DriverKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactDelivery_DimPartner')
BEGIN
    ALTER TABLE dbo.FactDelivery
        ADD CONSTRAINT FK_FactDelivery_DimPartner
        FOREIGN KEY (PartnerKey)
        REFERENCES dbo.DimPartner (PartnerKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactPartnerRevenue_DimTime')
BEGIN
    ALTER TABLE dbo.FactPartnerRevenue
        ADD CONSTRAINT FK_FactPartnerRevenue_DimTime
        FOREIGN KEY (TimeKey)
        REFERENCES dbo.DimTime (TimeKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactPartnerRevenue_DimStore')
BEGIN
    ALTER TABLE dbo.FactPartnerRevenue
        ADD CONSTRAINT FK_FactPartnerRevenue_DimStore
        FOREIGN KEY (StoreKey)
        REFERENCES dbo.DimStore (StoreKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FactPartnerRevenue_DimPartner')
BEGIN
    ALTER TABLE dbo.FactPartnerRevenue
        ADD CONSTRAINT FK_FactPartnerRevenue_DimPartner
        FOREIGN KEY (PartnerKey)
        REFERENCES dbo.DimPartner (PartnerKey);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FactOrder_Time_Store_Product' AND object_id = OBJECT_ID(N'dbo.FactOrder'))
BEGIN
    CREATE INDEX IX_FactOrder_Time_Store_Product
        ON dbo.FactOrder (TimeKey, StoreKey, ProductKey)
        INCLUDE (PartnerKey, OrderCount, NetSalesAmount);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FactOrder_Partner_Time' AND object_id = OBJECT_ID(N'dbo.FactOrder'))
BEGIN
    CREATE INDEX IX_FactOrder_Partner_Time
        ON dbo.FactOrder (PartnerKey, TimeKey, StoreKey)
        INCLUDE (NetSalesAmount, OrderCount);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FactDelivery_Time_Store_Driver' AND object_id = OBJECT_ID(N'dbo.FactDelivery'))
BEGIN
    CREATE INDEX IX_FactDelivery_Time_Store_Driver
        ON dbo.FactDelivery (TimeKey, StoreKey, DriverKey)
        INCLUDE (PartnerKey, LateDeliveryCount, NetSalesAmount);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FactDelivery_Partner_Time' AND object_id = OBJECT_ID(N'dbo.FactDelivery'))
BEGIN
    CREATE INDEX IX_FactDelivery_Partner_Time
        ON dbo.FactDelivery (PartnerKey, TimeKey, StoreKey)
        INCLUDE (NetSalesAmount, LateDeliveryCount);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FactPartnerRevenue_Time_Partner_Store' AND object_id = OBJECT_ID(N'dbo.FactPartnerRevenue'))
BEGIN
    CREATE INDEX IX_FactPartnerRevenue_Time_Partner_Store
        ON dbo.FactPartnerRevenue (TimeKey, PartnerKey, StoreKey)
        INCLUDE (NetSalesAmount, SettlementAmount);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\02-schema.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\02-schema.sql');
END
GO

