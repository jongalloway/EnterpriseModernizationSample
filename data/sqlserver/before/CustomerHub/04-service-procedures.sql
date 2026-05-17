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
        ca.AccountName,
        activeContract.ContractCode,
        activeContract.PricingScheduleName,
        activeContract.ReferralChannel,
        fl.FranchiseCode,
        fl.FranchiseName
    FROM dbo.PartnerAccount pa
    LEFT JOIN dbo.CorporateAccount ca
        ON ca.PreferredPartnerAccountId = pa.PartnerAccountId
    OUTER APPLY
    (
        SELECT TOP (1)
            pc.ContractCode,
            pc.PricingScheduleName,
            pc.ReferralChannel,
            pc.FranchiseLocationId
        FROM dbo.PartnerContract pc
        WHERE pc.PartnerAccountId = pa.PartnerAccountId
          AND pc.StatusCode = N'Active'
        ORDER BY pc.EffectiveDate DESC, pc.PartnerContractId DESC
    ) activeContract
    LEFT JOIN dbo.FranchiseLocation fl
        ON fl.FranchiseLocationId = activeContract.FranchiseLocationId
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

IF OBJECT_ID(N'dbo.usp_CorporateAccounts_GetActiveContracts', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CorporateAccounts_GetActiveContracts;
GO

CREATE PROCEDURE dbo.usp_CorporateAccounts_GetActiveContracts
    @AccountCode NVARCHAR(20) = NULL,
    @FranchiseCode NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pc.ContractCode,
        pc.ContractType,
        pa.PartnerCode,
        pa.PartnerName,
        ca.AccountCode,
        ca.AccountName,
        fl.FranchiseCode,
        fl.FranchiseName,
        pc.PricingScheduleName,
        pc.ReferralChannel,
        pc.EffectiveDate,
        pc.ExpirationDate,
        pc.MinimumOrderAmount,
        pc.DiscountPercentage,
        pc.CateringLeadHours,
        pc.StatusCode
    FROM dbo.PartnerContract pc
    INNER JOIN dbo.PartnerAccount pa
        ON pa.PartnerAccountId = pc.PartnerAccountId
    LEFT JOIN dbo.CorporateAccount ca
        ON ca.CorporateAccountId = pc.CorporateAccountId
    LEFT JOIN dbo.FranchiseLocation fl
        ON fl.FranchiseLocationId = pc.FranchiseLocationId
    WHERE pc.StatusCode = N'Active'
      AND (@AccountCode IS NULL OR ca.AccountCode = @AccountCode)
      AND (@FranchiseCode IS NULL OR fl.FranchiseCode = @FranchiseCode)
    ORDER BY ISNULL(ca.AccountCode, fl.FranchiseCode), pc.ContractCode;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\04-service-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\04-service-procedures.sql');
END
GO
