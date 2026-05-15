USE [$(CustomerHubDatabase)];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.PartnerAccount', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PartnerAccount
    (
        PartnerAccountId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        PartnerCode NVARCHAR(20) NOT NULL,
        PartnerName NVARCHAR(120) NOT NULL,
        RelationshipTier NVARCHAR(20) NOT NULL,
        PreferredStoreNumber NVARCHAR(10) NULL,
        StatusCode NVARCHAR(20) NOT NULL,
        LastContractRenewalDate DATETIME NULL,
        CreditHold BIT NOT NULL CONSTRAINT DF_PartnerAccount_CreditHold DEFAULT (0)
    );

    CREATE UNIQUE INDEX UX_PartnerAccount_PartnerCode ON dbo.PartnerAccount (PartnerCode);
END
GO

IF OBJECT_ID(N'dbo.CorporateAccount', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CorporateAccount
    (
        CorporateAccountId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        AccountCode NVARCHAR(20) NOT NULL,
        AccountName NVARCHAR(120) NOT NULL,
        PreferredPartnerAccountId INT NULL,
        BillingFrequency NVARCHAR(20) NOT NULL,
        ActiveFlag BIT NOT NULL CONSTRAINT DF_CorporateAccount_ActiveFlag DEFAULT (1)
    );

    CREATE UNIQUE INDEX UX_CorporateAccount_AccountCode ON dbo.CorporateAccount (AccountCode);
END
GO

IF OBJECT_ID(N'dbo.PartnerContact', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PartnerContact
    (
        PartnerContactId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        PartnerAccountId INT NOT NULL,
        ContactName NVARCHAR(100) NOT NULL,
        EmailAddress NVARCHAR(200) NULL,
        PhoneNumber NVARCHAR(30) NULL,
        IsPrimaryContact BIT NOT NULL CONSTRAINT DF_PartnerContact_IsPrimaryContact DEFAULT (0)
    );
END
GO

IF OBJECT_ID(N'dbo.FranchiseLocation', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FranchiseLocation
    (
        FranchiseLocationId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        FranchiseCode NVARCHAR(20) NOT NULL,
        FranchiseName NVARCHAR(120) NOT NULL,
        PrimaryStoreNumber NVARCHAR(10) NOT NULL,
        MarketName NVARCHAR(80) NOT NULL,
        OwnerName NVARCHAR(120) NOT NULL,
        PrimaryPartnerAccountId INT NULL,
        StatusCode NVARCHAR(20) NOT NULL,
        LastPortalSyncUtc DATETIME NOT NULL CONSTRAINT DF_FranchiseLocation_LastPortalSyncUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_FranchiseLocation_FranchiseCode ON dbo.FranchiseLocation (FranchiseCode);
    CREATE INDEX IX_FranchiseLocation_PrimaryStoreNumber ON dbo.FranchiseLocation (PrimaryStoreNumber);
END
GO

IF OBJECT_ID(N'dbo.PartnerContract', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PartnerContract
    (
        PartnerContractId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ContractCode NVARCHAR(30) NOT NULL,
        PartnerAccountId INT NOT NULL,
        CorporateAccountId INT NULL,
        FranchiseLocationId INT NULL,
        ContractType NVARCHAR(30) NOT NULL,
        PricingScheduleName NVARCHAR(80) NOT NULL,
        ReferralChannel NVARCHAR(40) NULL,
        EffectiveDate DATE NOT NULL,
        ExpirationDate DATE NULL,
        MinimumOrderAmount MONEY NOT NULL,
        DiscountPercentage DECIMAL(5,2) NOT NULL,
        CateringLeadHours SMALLINT NOT NULL,
        StatusCode NVARCHAR(20) NOT NULL,
        LastReviewedUtc DATETIME NOT NULL CONSTRAINT DF_PartnerContract_LastReviewedUtc DEFAULT (GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_PartnerContract_ContractCode ON dbo.PartnerContract (ContractCode);
    CREATE INDEX IX_PartnerContract_PartnerAccount_StatusCode ON dbo.PartnerContract (PartnerAccountId, StatusCode);
END
GO

IF OBJECT_ID(N'dbo.PartnerAccountExtract', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PartnerAccountExtract
    (
        PartnerAccountExtractId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SyncBatchId UNIQUEIDENTIFIER NOT NULL,
        PartnerCode NVARCHAR(20) NOT NULL,
        PartnerName NVARCHAR(120) NOT NULL,
        RelationshipTier NVARCHAR(20) NOT NULL,
        PreferredStoreNumber NVARCHAR(10) NULL,
        ExtractedUtc DATETIME NOT NULL CONSTRAINT DF_PartnerAccountExtract_ExtractedUtc DEFAULT (GETUTCDATE())
    );

    CREATE INDEX IX_PartnerAccountExtract_SyncBatchId ON dbo.PartnerAccountExtract (SyncBatchId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_CorporateAccount_PartnerAccount'
)
BEGIN
    ALTER TABLE dbo.CorporateAccount
        ADD CONSTRAINT FK_CorporateAccount_PartnerAccount
        FOREIGN KEY (PreferredPartnerAccountId)
        REFERENCES dbo.PartnerAccount (PartnerAccountId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_PartnerContact_PartnerAccount'
)
BEGIN
    ALTER TABLE dbo.PartnerContact
        ADD CONSTRAINT FK_PartnerContact_PartnerAccount
        FOREIGN KEY (PartnerAccountId)
        REFERENCES dbo.PartnerAccount (PartnerAccountId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_FranchiseLocation_PartnerAccount'
)
BEGIN
    ALTER TABLE dbo.FranchiseLocation
        ADD CONSTRAINT FK_FranchiseLocation_PartnerAccount
        FOREIGN KEY (PrimaryPartnerAccountId)
        REFERENCES dbo.PartnerAccount (PartnerAccountId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_PartnerContract_PartnerAccount'
)
BEGIN
    ALTER TABLE dbo.PartnerContract
        ADD CONSTRAINT FK_PartnerContract_PartnerAccount
        FOREIGN KEY (PartnerAccountId)
        REFERENCES dbo.PartnerAccount (PartnerAccountId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_PartnerContract_CorporateAccount'
)
BEGIN
    ALTER TABLE dbo.PartnerContract
        ADD CONSTRAINT FK_PartnerContract_CorporateAccount
        FOREIGN KEY (CorporateAccountId)
        REFERENCES dbo.CorporateAccount (CorporateAccountId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_PartnerContract_FranchiseLocation'
)
BEGIN
    ALTER TABLE dbo.PartnerContract
        ADD CONSTRAINT FK_PartnerContract_FranchiseLocation
        FOREIGN KEY (FranchiseLocationId)
        REFERENCES dbo.FranchiseLocation (FranchiseLocationId);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\02-schema.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\02-schema.sql');
END
GO
