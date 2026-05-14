USE [$(CustomerHubDatabase)];
GO

IF OBJECT_ID(N'dbo.usp_CorporateAccounts_GetPreferredPartners', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CorporateAccounts_GetPreferredPartners;
GO

CREATE PROCEDURE dbo.usp_CorporateAccounts_GetPreferredPartners
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pa.PartnerCode,
        pa.PartnerName,
        pa.RelationshipTier,
        pa.PreferredStoreNumber,
        ca.AccountCode,
        ca.AccountName
    FROM dbo.PartnerAccount pa
    LEFT JOIN dbo.CorporateAccount ca
        ON ca.PreferredPartnerAccountId = pa.PartnerAccountId
    WHERE pa.StatusCode = N'Active'
    ORDER BY pa.RelationshipTier, pa.PartnerName;
END
GO

IF OBJECT_ID(N'dbo.usp_CorporateAccounts_GetPartnerContacts', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CorporateAccounts_GetPartnerContacts;
GO

CREATE PROCEDURE dbo.usp_CorporateAccounts_GetPartnerContacts
    @PartnerCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pa.PartnerCode,
        pc.ContactName,
        pc.EmailAddress,
        pc.PhoneNumber,
        pc.IsPrimaryContact
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.PartnerContact pc
        ON pc.PartnerAccountId = pa.PartnerAccountId
    WHERE pa.PartnerCode = @PartnerCode
    ORDER BY pc.IsPrimaryContact DESC, pc.ContactName;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\04-service-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\04-service-procedures.sql');
END
GO
