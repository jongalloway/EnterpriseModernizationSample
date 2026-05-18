USE [$(StoreOpsDatabase)];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.Store', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Store
    (
        StoreId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreNumber NVARCHAR(10) NOT NULL,
        StoreName NVARCHAR(120) NOT NULL,
        RegionCode NVARCHAR(20) NOT NULL,
        PhoneNumber NVARCHAR(30) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Store_IsActive DEFAULT (1)
    );

    CREATE UNIQUE INDEX UX_Store_StoreNumber ON dbo.Store (StoreNumber);
END
GO

IF COL_LENGTH(N'dbo.Store', N'AddressLine1') IS NULL
BEGIN
    ALTER TABLE dbo.Store ADD AddressLine1 NVARCHAR(120) NULL;
END
GO

IF COL_LENGTH(N'dbo.Store', N'AddressLine2') IS NULL
BEGIN
    ALTER TABLE dbo.Store ADD AddressLine2 NVARCHAR(120) NULL;
END
GO

IF COL_LENGTH(N'dbo.Store', N'City') IS NULL
BEGIN
    ALTER TABLE dbo.Store ADD City NVARCHAR(80) NULL;
END
GO

IF COL_LENGTH(N'dbo.Store', N'StateProvinceCode') IS NULL
BEGIN
    ALTER TABLE dbo.Store ADD StateProvinceCode NVARCHAR(20) NULL;
END
GO

IF COL_LENGTH(N'dbo.Store', N'PostalCode') IS NULL
BEGIN
    ALTER TABLE dbo.Store ADD PostalCode NVARCHAR(20) NULL;
END
GO

IF COL_LENGTH(N'dbo.Store', N'TimeZoneId') IS NULL
BEGIN
    ALTER TABLE dbo.Store ADD TimeZoneId NVARCHAR(50) NULL;
END
GO

IF OBJECT_ID(N'dbo.RouteZone', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RouteZone
    (
        RouteZoneId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        ZoneCode NVARCHAR(20) NOT NULL,
        ZoneName NVARCHAR(120) NOT NULL,
        SortOrder TINYINT NOT NULL
    );

    CREATE UNIQUE INDEX UX_RouteZone_Store_ZoneCode ON dbo.RouteZone (StoreId, ZoneCode);
END
GO

IF OBJECT_ID(N'dbo.Driver', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Driver
    (
        DriverId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        DriverCode NVARCHAR(20) NOT NULL,
        DisplayName NVARCHAR(100) NOT NULL,
        EmploymentStatus NVARCHAR(20) NOT NULL,
        HomeZoneCode NVARCHAR(20) NULL,
        EligibilityExpirationDate DATETIME NULL
    );

    CREATE UNIQUE INDEX UX_Driver_DriverCode ON dbo.Driver (DriverCode);
END
GO

IF COL_LENGTH(N'dbo.Driver', N'AvailabilityStatus') IS NULL
BEGIN
    ALTER TABLE dbo.Driver ADD AvailabilityStatus NVARCHAR(20) NULL;
END
GO

IF COL_LENGTH(N'dbo.Driver', N'MobilePhone') IS NULL
BEGIN
    ALTER TABLE dbo.Driver ADD MobilePhone NVARCHAR(30) NULL;
END
GO

IF COL_LENGTH(N'dbo.Driver', N'LastStatusChangeUtc') IS NULL
BEGIN
    ALTER TABLE dbo.Driver ADD LastStatusChangeUtc DATETIME NULL;
END
GO

IF OBJECT_ID(N'dbo.StoreOperationsStatus', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StoreOperationsStatus
    (
        StoreOperationsStatusId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        DistrictName NVARCHAR(60) NOT NULL,
        DispatchTerminalId NVARCHAR(20) NOT NULL,
        ManagerOnDuty NVARCHAR(100) NOT NULL,
        BoardMode NVARCHAR(30) NOT NULL,
        LastStatusRefreshUtc DATETIME NOT NULL,
        StoreStatus NVARCHAR(30) NOT NULL,
        EscalationNote NVARCHAR(250) NULL
    );

    CREATE UNIQUE INDEX UX_StoreOperationsStatus_StoreId ON dbo.StoreOperationsStatus (StoreId);
END
GO

IF OBJECT_ID(N'dbo.StoreOperatingHours', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StoreOperatingHours
    (
        StoreOperatingHoursId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        DayOfWeekNumber TINYINT NOT NULL,
        OpenTime TIME NOT NULL,
        CloseTime TIME NOT NULL,
        DeliveryCutoffTime TIME NOT NULL,
        LobbyCloseTime TIME NULL
    );

    CREATE UNIQUE INDEX UX_StoreOperatingHours_Store_DayOfWeek ON dbo.StoreOperatingHours (StoreId, DayOfWeekNumber);
END
GO

IF OBJECT_ID(N'dbo.StoreConfiguration', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StoreConfiguration
    (
        StoreConfigurationId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        DeliveryRadiusMiles DECIMAL(5,2) NOT NULL,
        MaxConcurrentDeliveries INT NOT NULL,
        MaxStackedStops INT NOT NULL,
        DispatchLeadMinutes INT NOT NULL,
        CarryoutHoldMinutes INT NOT NULL,
        AcceptsThirdPartyDispatch BIT NOT NULL,
        CateringWarmHoldMinutes INT NOT NULL,
        LastMenuRefreshUtc DATETIME NOT NULL
    );

    CREATE UNIQUE INDEX UX_StoreConfiguration_StoreId ON dbo.StoreConfiguration (StoreId);
END
GO

IF OBJECT_ID(N'dbo.MenuCategory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MenuCategory
    (
        MenuCategoryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CategoryCode NVARCHAR(20) NOT NULL,
        CategoryName NVARCHAR(80) NOT NULL,
        SortOrder TINYINT NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_MenuCategory_IsActive DEFAULT (1)
    );

    CREATE UNIQUE INDEX UX_MenuCategory_CategoryCode ON dbo.MenuCategory (CategoryCode);
END
GO

IF OBJECT_ID(N'dbo.MenuProduct', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MenuProduct
    (
        MenuProductId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ProductCode NVARCHAR(30) NOT NULL,
        MenuCategoryId INT NOT NULL,
        ProductName NVARCHAR(120) NOT NULL,
        ProductSize NVARCHAR(40) NULL,
        LegacyPosCode NVARCHAR(20) NULL,
        BasePrice MONEY NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_MenuProduct_IsActive DEFAULT (1),
        IsDeliveryEligible BIT NOT NULL CONSTRAINT DF_MenuProduct_IsDeliveryEligible DEFAULT (1),
        SortOrder TINYINT NOT NULL
    );

    CREATE UNIQUE INDEX UX_MenuProduct_ProductCode ON dbo.MenuProduct (ProductCode);
END
GO

IF OBJECT_ID(N'dbo.StoreOrder', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StoreOrder
    (
        StoreOrderId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        OrderNumber INT NOT NULL,
        StoreId INT NOT NULL,
        CustomerName NVARCHAR(120) NOT NULL,
        ChannelCode NVARCHAR(20) NOT NULL,
        ServiceMode NVARCHAR(20) NOT NULL,
        PromiseUtc DATETIME NOT NULL,
        TicketTotal MONEY NOT NULL,
        KitchenStatus NVARCHAR(20) NOT NULL,
        DispatchStatus NVARCHAR(20) NOT NULL,
        PaymentStatus NVARCHAR(20) NOT NULL,
        CreatedUtc DATETIME NOT NULL CONSTRAINT DF_StoreOrder_CreatedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_StoreOrder_OrderNumber ON dbo.StoreOrder (OrderNumber);
    CREATE INDEX IX_StoreOrder_Store_PromiseUtc ON dbo.StoreOrder (StoreId, PromiseUtc);
END
GO

IF OBJECT_ID(N'dbo.DispatchTicket', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DispatchTicket
    (
        DispatchTicketId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TicketNumber INT NOT NULL,
        StoreOrderId INT NULL,
        StoreId INT NOT NULL,
        DriverId INT NULL,
        RouteZoneId INT NOT NULL,
        OrderOrigin NVARCHAR(20) NOT NULL,
        PromiseUtc DATETIME NOT NULL,
        StatusCode NVARCHAR(20) NOT NULL,
        TotalAmount MONEY NOT NULL,
        LastUpdatedUtc DATETIME NOT NULL CONSTRAINT DF_DispatchTicket_LastUpdatedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_DispatchTicket_TicketNumber ON dbo.DispatchTicket (TicketNumber);
    CREATE INDEX IX_DispatchTicket_Store_StatusCode ON dbo.DispatchTicket (StoreId, StatusCode);
END
GO

IF COL_LENGTH(N'dbo.DispatchTicket', N'StoreOrderId') IS NULL
BEGIN
    ALTER TABLE dbo.DispatchTicket
        ADD StoreOrderId INT NULL;
END
GO

IF OBJECT_ID(N'dbo.DeliveryRecord', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DeliveryRecord
    (
        DeliveryRecordId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        DispatchTicketId INT NOT NULL,
        StoreOrderId INT NOT NULL,
        StoreId INT NOT NULL,
        DriverId INT NOT NULL,
        RouteZoneId INT NOT NULL,
        RoutedUtc DATETIME NOT NULL,
        DepartedUtc DATETIME NOT NULL,
        DeliveredUtc DATETIME NOT NULL,
        ReturnedUtc DATETIME NULL,
        RouteMiles DECIMAL(6,2) NOT NULL,
        DeliveryMinutes INT NOT NULL,
        TipAmount MONEY NOT NULL,
        OutcomeCode NVARCHAR(20) NOT NULL,
        RouteSummary NVARCHAR(200) NULL
    );

    CREATE UNIQUE INDEX UX_DeliveryRecord_DispatchTicketId ON dbo.DeliveryRecord (DispatchTicketId);
    CREATE INDEX IX_DeliveryRecord_Store_DeliveredUtc ON dbo.DeliveryRecord (StoreId, DeliveredUtc);
END
GO

IF OBJECT_ID(N'dbo.PosOrderImportBatch', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PosOrderImportBatch
    (
        PosOrderImportBatchId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        SourceSystem NVARCHAR(50) NOT NULL,
        BatchDate DATE NOT NULL,
        ImportedUtc DATETIME NOT NULL,
        BatchStatus NVARCHAR(20) NOT NULL,
        ItemCount INT NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.PosOrderImportItem', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PosOrderImportItem
    (
        PosOrderImportItemId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        PosOrderImportBatchId INT NOT NULL,
        OrderNumber INT NOT NULL,
        ChannelCode NVARCHAR(20) NOT NULL,
        ServiceMode NVARCHAR(20) NOT NULL,
        ImportStatus NVARCHAR(20) NOT NULL,
        ImportedTicketTotal MONEY NOT NULL,
        ExceptionNote NVARCHAR(200) NULL
    );

    CREATE UNIQUE INDEX UX_PosOrderImportItem_Batch_OrderNumber ON dbo.PosOrderImportItem (PosOrderImportBatchId, OrderNumber);
END
GO

IF OBJECT_ID(N'dbo.WorkforceAlert', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WorkforceAlert
    (
        WorkforceAlertId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        TeamName NVARCHAR(50) NOT NULL,
        ConcernText NVARCHAR(200) NOT NULL,
        ActionRequired NVARCHAR(200) NOT NULL,
        SeverityCode NVARCHAR(20) NOT NULL,
        EffectiveUtc DATETIME NOT NULL,
        ResolvedUtc DATETIME NULL
    );

    CREATE INDEX IX_WorkforceAlert_Store_EffectiveUtc ON dbo.WorkforceAlert (StoreId, EffectiveUtc);
END
GO

IF OBJECT_ID(N'dbo.RouteZoneBulletin', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RouteZoneBulletin
    (
        RouteZoneBulletinId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        RouteZoneId INT NULL,
        BulletinText NVARCHAR(250) NOT NULL,
        EffectiveUtc DATETIME NOT NULL,
        ExpiresUtc DATETIME NULL,
        RequiresAcknowledgement BIT NOT NULL CONSTRAINT DF_RouteZoneBulletin_RequiresAcknowledgement DEFAULT (0)
    );

    CREATE INDEX IX_RouteZoneBulletin_Store_EffectiveUtc ON dbo.RouteZoneBulletin (StoreId, EffectiveUtc);
END
GO

IF OBJECT_ID(N'dbo.PayrollDailyImport', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PayrollDailyImport
    (
        PayrollDailyImportId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        WorkDate DATE NOT NULL,
        ScheduledHours DECIMAL(9,2) NOT NULL,
        WorkedHours DECIMAL(9,2) NOT NULL,
        OvertimeHours DECIMAL(9,2) NOT NULL,
        RegularLaborCost MONEY NOT NULL,
        OvertimeLaborCost MONEY NOT NULL,
        AgencyLaborCost MONEY NOT NULL,
        ScheduledDriverSlots INT NOT NULL,
        FilledDriverSlots INT NOT NULL,
        OpenDriverSlots INT NOT NULL,
        CrossTrainedTeamMembers INT NOT NULL,
        CalloutCount INT NOT NULL,
        NetSales MONEY NOT NULL,
        ImportedUtc DATETIME NOT NULL CONSTRAINT DF_PayrollDailyImport_ImportedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_PayrollDailyImport_Store_WorkDate ON dbo.PayrollDailyImport (StoreId, WorkDate);
END
GO

IF OBJECT_ID(N'dbo.PayrollOvertimeWeeklyImport', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PayrollOvertimeWeeklyImport
    (
        PayrollOvertimeWeeklyImportId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        WeekEndingDate DATE NOT NULL,
        DriverOvertimeHours DECIMAL(9,2) NOT NULL,
        KitchenOvertimeHours DECIMAL(9,2) NOT NULL,
        ShiftLeadOvertimeHours DECIMAL(9,2) NOT NULL,
        ImportedUtc DATETIME NOT NULL CONSTRAINT DF_PayrollOvertimeWeeklyImport_ImportedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_PayrollOvertimeWeeklyImport_Store_WeekEndingDate ON dbo.PayrollOvertimeWeeklyImport (StoreId, WeekEndingDate);
END
GO

IF OBJECT_ID(N'dbo.PayrollTurnoverMonthlyImport', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PayrollTurnoverMonthlyImport
    (
        PayrollTurnoverMonthlyImportId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StoreId INT NOT NULL,
        SummaryMonth DATE NOT NULL,
        BeginningHeadcount INT NOT NULL,
        HireCount INT NOT NULL,
        SeparationCount INT NOT NULL,
        EndingHeadcount INT NOT NULL,
        ImportedUtc DATETIME NOT NULL CONSTRAINT DF_PayrollTurnoverMonthlyImport_ImportedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_PayrollTurnoverMonthlyImport_Store_SummaryMonth ON dbo.PayrollTurnoverMonthlyImport (StoreId, SummaryMonth);
END
GO

IF OBJECT_ID(N'dbo.PartnerAccountCache', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PartnerAccountCache
    (
        PartnerAccountCacheId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        PartnerCode NVARCHAR(20) NOT NULL,
        PartnerName NVARCHAR(120) NOT NULL,
        RelationshipTier NVARCHAR(20) NOT NULL,
        PreferredStoreNumber NVARCHAR(10) NULL,
        LastSyncUtc DATETIME NOT NULL CONSTRAINT DF_PartnerAccountCache_LastSyncUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_PartnerAccountCache_PartnerCode ON dbo.PartnerAccountCache (PartnerCode);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_RouteZone_Store')
BEGIN
    ALTER TABLE dbo.RouteZone
        ADD CONSTRAINT FK_RouteZone_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Driver_Store')
BEGIN
    ALTER TABLE dbo.Driver
        ADD CONSTRAINT FK_Driver_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_StoreOperationsStatus_Store')
BEGIN
    ALTER TABLE dbo.StoreOperationsStatus
        ADD CONSTRAINT FK_StoreOperationsStatus_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_StoreOperatingHours_Store')
BEGIN
    ALTER TABLE dbo.StoreOperatingHours
        ADD CONSTRAINT FK_StoreOperatingHours_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_StoreConfiguration_Store')
BEGIN
    ALTER TABLE dbo.StoreConfiguration
        ADD CONSTRAINT FK_StoreConfiguration_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MenuProduct_MenuCategory')
BEGIN
    ALTER TABLE dbo.MenuProduct
        ADD CONSTRAINT FK_MenuProduct_MenuCategory
        FOREIGN KEY (MenuCategoryId)
        REFERENCES dbo.MenuCategory (MenuCategoryId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_StoreOrder_Store')
BEGIN
    ALTER TABLE dbo.StoreOrder
        ADD CONSTRAINT FK_StoreOrder_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DispatchTicket_Store')
BEGIN
    ALTER TABLE dbo.DispatchTicket
        ADD CONSTRAINT FK_DispatchTicket_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DispatchTicket_Driver')
BEGIN
    ALTER TABLE dbo.DispatchTicket
        ADD CONSTRAINT FK_DispatchTicket_Driver
        FOREIGN KEY (DriverId)
        REFERENCES dbo.Driver (DriverId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DispatchTicket_RouteZone')
BEGIN
    ALTER TABLE dbo.DispatchTicket
        ADD CONSTRAINT FK_DispatchTicket_RouteZone
        FOREIGN KEY (RouteZoneId)
        REFERENCES dbo.RouteZone (RouteZoneId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DispatchTicket_StoreOrder')
BEGIN
    ALTER TABLE dbo.DispatchTicket
        ADD CONSTRAINT FK_DispatchTicket_StoreOrder
        FOREIGN KEY (StoreOrderId)
        REFERENCES dbo.StoreOrder (StoreOrderId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DeliveryRecord_DispatchTicket')
BEGIN
    ALTER TABLE dbo.DeliveryRecord
        ADD CONSTRAINT FK_DeliveryRecord_DispatchTicket
        FOREIGN KEY (DispatchTicketId)
        REFERENCES dbo.DispatchTicket (DispatchTicketId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DeliveryRecord_StoreOrder')
BEGIN
    ALTER TABLE dbo.DeliveryRecord
        ADD CONSTRAINT FK_DeliveryRecord_StoreOrder
        FOREIGN KEY (StoreOrderId)
        REFERENCES dbo.StoreOrder (StoreOrderId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DeliveryRecord_Store')
BEGIN
    ALTER TABLE dbo.DeliveryRecord
        ADD CONSTRAINT FK_DeliveryRecord_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DeliveryRecord_Driver')
BEGIN
    ALTER TABLE dbo.DeliveryRecord
        ADD CONSTRAINT FK_DeliveryRecord_Driver
        FOREIGN KEY (DriverId)
        REFERENCES dbo.Driver (DriverId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DeliveryRecord_RouteZone')
BEGIN
    ALTER TABLE dbo.DeliveryRecord
        ADD CONSTRAINT FK_DeliveryRecord_RouteZone
        FOREIGN KEY (RouteZoneId)
        REFERENCES dbo.RouteZone (RouteZoneId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PosOrderImportBatch_Store')
BEGIN
    ALTER TABLE dbo.PosOrderImportBatch
        ADD CONSTRAINT FK_PosOrderImportBatch_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PosOrderImportItem_PosOrderImportBatch')
BEGIN
    ALTER TABLE dbo.PosOrderImportItem
        ADD CONSTRAINT FK_PosOrderImportItem_PosOrderImportBatch
        FOREIGN KEY (PosOrderImportBatchId)
        REFERENCES dbo.PosOrderImportBatch (PosOrderImportBatchId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_WorkforceAlert_Store')
BEGIN
    ALTER TABLE dbo.WorkforceAlert
        ADD CONSTRAINT FK_WorkforceAlert_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_RouteZoneBulletin_Store')
BEGIN
    ALTER TABLE dbo.RouteZoneBulletin
        ADD CONSTRAINT FK_RouteZoneBulletin_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_RouteZoneBulletin_RouteZone')
BEGIN
    ALTER TABLE dbo.RouteZoneBulletin
        ADD CONSTRAINT FK_RouteZoneBulletin_RouteZone
        FOREIGN KEY (RouteZoneId)
        REFERENCES dbo.RouteZone (RouteZoneId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PayrollDailyImport_Store')
BEGIN
    ALTER TABLE dbo.PayrollDailyImport
        ADD CONSTRAINT FK_PayrollDailyImport_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PayrollOvertimeWeeklyImport_Store')
BEGIN
    ALTER TABLE dbo.PayrollOvertimeWeeklyImport
        ADD CONSTRAINT FK_PayrollOvertimeWeeklyImport_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PayrollTurnoverMonthlyImport_Store')
BEGIN
    ALTER TABLE dbo.PayrollTurnoverMonthlyImport
        ADD CONSTRAINT FK_PayrollTurnoverMonthlyImport_Store
        FOREIGN KEY (StoreId)
        REFERENCES dbo.Store (StoreId);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'StoreOps\02-schema.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'StoreOps\02-schema.sql');
END
GO
