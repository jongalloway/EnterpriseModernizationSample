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

IF OBJECT_ID(N'dbo.DispatchTicket', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DispatchTicket
    (
        DispatchTicketId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TicketNumber INT NOT NULL,
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

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PosOrderImportBatch_Store')
BEGIN
    ALTER TABLE dbo.PosOrderImportBatch
        ADD CONSTRAINT FK_PosOrderImportBatch_Store
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
