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

IF OBJECT_ID(N'dbo.usp_CustomerHubPartners_GetActive', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubPartners_GetActive;
GO

CREATE PROCEDURE dbo.usp_CustomerHubPartners_GetActive
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pa.PartnerAccountId,
        pa.PartnerCode,
        pa.PartnerName,
        pa.RelationshipTier,
        pa.PreferredStoreNumber,
        pa.StatusCode,
        pa.LastContractRenewalDate,
        pa.CreditHold
    FROM dbo.PartnerAccount pa
    WHERE pa.StatusCode IN (N'Active', N'Onboarding')
    ORDER BY pa.RelationshipTier DESC, pa.PartnerName;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubPartners_GetByCode', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubPartners_GetByCode;
GO

CREATE PROCEDURE dbo.usp_CustomerHubPartners_GetByCode
    @PartnerCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pa.PartnerAccountId,
        pa.PartnerCode,
        pa.PartnerName,
        pa.RelationshipTier,
        pa.PreferredStoreNumber,
        pa.StatusCode,
        pa.LastContractRenewalDate,
        pa.CreditHold
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = @PartnerCode;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubPartners_Save', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubPartners_Save;
GO

CREATE PROCEDURE dbo.usp_CustomerHubPartners_Save
    @PartnerAccountId INT = NULL,
    @PartnerCode NVARCHAR(20),
    @PartnerName NVARCHAR(120),
    @RelationshipTier NVARCHAR(20),
    @PreferredStoreNumber NVARCHAR(10) = NULL,
    @StatusCode NVARCHAR(20),
    @LastContractRenewalDate DATETIME = NULL,
    @CreditHold BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.PartnerAccount WHERE PartnerAccountId = ISNULL(@PartnerAccountId, -1) OR PartnerCode = @PartnerCode)
    BEGIN
        UPDATE dbo.PartnerAccount
        SET PartnerCode = @PartnerCode,
            PartnerName = @PartnerName,
            RelationshipTier = @RelationshipTier,
            PreferredStoreNumber = @PreferredStoreNumber,
            StatusCode = @StatusCode,
            LastContractRenewalDate = @LastContractRenewalDate,
            CreditHold = @CreditHold
        WHERE (@PartnerAccountId IS NOT NULL AND PartnerAccountId = @PartnerAccountId)
           OR (@PartnerAccountId IS NULL AND PartnerCode = @PartnerCode);
    END
    ELSE
    BEGIN
        INSERT INTO dbo.PartnerAccount
        (
            PartnerCode,
            PartnerName,
            RelationshipTier,
            PreferredStoreNumber,
            StatusCode,
            LastContractRenewalDate,
            CreditHold
        )
        VALUES
        (
            @PartnerCode,
            @PartnerName,
            @RelationshipTier,
            @PreferredStoreNumber,
            @StatusCode,
            @LastContractRenewalDate,
            @CreditHold
        );
    END

    SELECT TOP (1)
        pa.PartnerAccountId,
        pa.PartnerCode,
        pa.PartnerName,
        pa.RelationshipTier,
        pa.PreferredStoreNumber,
        pa.StatusCode,
        pa.LastContractRenewalDate,
        pa.CreditHold
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = @PartnerCode
    ORDER BY pa.PartnerAccountId DESC;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubPartners_Delete', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubPartners_Delete;
GO

CREATE PROCEDURE dbo.usp_CustomerHubPartners_Delete
    @PartnerCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PartnerAccount
    SET StatusCode = N'Inactive'
    WHERE PartnerCode = @PartnerCode;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT) AS Deleted;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubContracts_GetByStatus', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubContracts_GetByStatus;
GO

CREATE PROCEDURE dbo.usp_CustomerHubContracts_GetByStatus
    @StatusCode NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pc.PartnerContractId,
        pc.ContractCode,
        pc.PartnerAccountId,
        pc.CorporateAccountId,
        pc.FranchiseLocationId,
        pc.ContractType,
        pc.PricingScheduleName,
        pc.ReferralChannel,
        pc.EffectiveDate,
        pc.ExpirationDate,
        pc.MinimumOrderAmount,
        pc.DiscountPercentage,
        pc.CateringLeadHours,
        pc.StatusCode,
        pc.LastReviewedUtc
    FROM dbo.PartnerContract pc
    WHERE @StatusCode IS NULL OR pc.StatusCode = @StatusCode
    ORDER BY pc.EffectiveDate DESC, pc.ContractCode;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubContracts_GetByCode', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubContracts_GetByCode;
GO

CREATE PROCEDURE dbo.usp_CustomerHubContracts_GetByCode
    @ContractCode NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pc.PartnerContractId,
        pc.ContractCode,
        pc.PartnerAccountId,
        pc.CorporateAccountId,
        pc.FranchiseLocationId,
        pc.ContractType,
        pc.PricingScheduleName,
        pc.ReferralChannel,
        pc.EffectiveDate,
        pc.ExpirationDate,
        pc.MinimumOrderAmount,
        pc.DiscountPercentage,
        pc.CateringLeadHours,
        pc.StatusCode,
        pc.LastReviewedUtc
    FROM dbo.PartnerContract pc
    WHERE pc.ContractCode = @ContractCode;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubContracts_Save', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubContracts_Save;
GO

CREATE PROCEDURE dbo.usp_CustomerHubContracts_Save
    @PartnerContractId INT = NULL,
    @ContractCode NVARCHAR(30),
    @PartnerAccountId INT,
    @CorporateAccountId INT = NULL,
    @FranchiseLocationId INT = NULL,
    @ContractType NVARCHAR(30),
    @PricingScheduleName NVARCHAR(80),
    @ReferralChannel NVARCHAR(40) = NULL,
    @EffectiveDate DATE,
    @ExpirationDate DATE = NULL,
    @MinimumOrderAmount MONEY,
    @DiscountPercentage DECIMAL(5,2),
    @CateringLeadHours SMALLINT,
    @StatusCode NVARCHAR(20),
    @LastReviewedUtc DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.PartnerContract WHERE PartnerContractId = ISNULL(@PartnerContractId, -1) OR ContractCode = @ContractCode)
    BEGIN
        UPDATE dbo.PartnerContract
        SET ContractCode = @ContractCode,
            PartnerAccountId = @PartnerAccountId,
            CorporateAccountId = @CorporateAccountId,
            FranchiseLocationId = @FranchiseLocationId,
            ContractType = @ContractType,
            PricingScheduleName = @PricingScheduleName,
            ReferralChannel = @ReferralChannel,
            EffectiveDate = @EffectiveDate,
            ExpirationDate = @ExpirationDate,
            MinimumOrderAmount = @MinimumOrderAmount,
            DiscountPercentage = @DiscountPercentage,
            CateringLeadHours = @CateringLeadHours,
            StatusCode = @StatusCode,
            LastReviewedUtc = ISNULL(@LastReviewedUtc, GETUTCDATE())
        WHERE (@PartnerContractId IS NOT NULL AND PartnerContractId = @PartnerContractId)
           OR (@PartnerContractId IS NULL AND ContractCode = @ContractCode);
    END
    ELSE
    BEGIN
        INSERT INTO dbo.PartnerContract
        (
            ContractCode,
            PartnerAccountId,
            CorporateAccountId,
            FranchiseLocationId,
            ContractType,
            PricingScheduleName,
            ReferralChannel,
            EffectiveDate,
            ExpirationDate,
            MinimumOrderAmount,
            DiscountPercentage,
            CateringLeadHours,
            StatusCode,
            LastReviewedUtc
        )
        VALUES
        (
            @ContractCode,
            @PartnerAccountId,
            @CorporateAccountId,
            @FranchiseLocationId,
            @ContractType,
            @PricingScheduleName,
            @ReferralChannel,
            @EffectiveDate,
            @ExpirationDate,
            @MinimumOrderAmount,
            @DiscountPercentage,
            @CateringLeadHours,
            @StatusCode,
            ISNULL(@LastReviewedUtc, GETUTCDATE())
        );
    END

    SELECT TOP (1)
        pc.PartnerContractId,
        pc.ContractCode,
        pc.PartnerAccountId,
        pc.CorporateAccountId,
        pc.FranchiseLocationId,
        pc.ContractType,
        pc.PricingScheduleName,
        pc.ReferralChannel,
        pc.EffectiveDate,
        pc.ExpirationDate,
        pc.MinimumOrderAmount,
        pc.DiscountPercentage,
        pc.CateringLeadHours,
        pc.StatusCode,
        pc.LastReviewedUtc
    FROM dbo.PartnerContract pc
    WHERE pc.ContractCode = @ContractCode
    ORDER BY pc.PartnerContractId DESC;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubContracts_UpdateStatus', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubContracts_UpdateStatus;
GO

CREATE PROCEDURE dbo.usp_CustomerHubContracts_UpdateStatus
    @ContractCode NVARCHAR(30),
    @StatusCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PartnerContract
    SET StatusCode = @StatusCode,
        LastReviewedUtc = GETUTCDATE()
    WHERE ContractCode = @ContractCode;

    SELECT
        pc.PartnerContractId,
        pc.ContractCode,
        pc.PartnerAccountId,
        pc.CorporateAccountId,
        pc.FranchiseLocationId,
        pc.ContractType,
        pc.PricingScheduleName,
        pc.ReferralChannel,
        pc.EffectiveDate,
        pc.ExpirationDate,
        pc.MinimumOrderAmount,
        pc.DiscountPercentage,
        pc.CateringLeadHours,
        pc.StatusCode,
        pc.LastReviewedUtc
    FROM dbo.PartnerContract pc
    WHERE pc.ContractCode = @ContractCode;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubAccounts_GetByTier', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubAccounts_GetByTier;
GO

CREATE PROCEDURE dbo.usp_CustomerHubAccounts_GetByTier
    @AccountTier NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ca.CorporateAccountId,
        ca.AccountCode,
        ca.AccountName,
        ca.PreferredPartnerAccountId,
        ca.BillingFrequency,
        ca.ActiveFlag,
        ca.AccountTier,
        ca.ExternalAccountCode,
        ca.AccountManagerName,
        ca.StatusCode
    FROM dbo.CorporateAccount ca
    WHERE @AccountTier IS NULL OR ca.AccountTier = @AccountTier
    ORDER BY ca.AccountCode;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubAccounts_GetByCode', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubAccounts_GetByCode;
GO

CREATE PROCEDURE dbo.usp_CustomerHubAccounts_GetByCode
    @AccountCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ca.CorporateAccountId,
        ca.AccountCode,
        ca.AccountName,
        ca.PreferredPartnerAccountId,
        ca.BillingFrequency,
        ca.ActiveFlag,
        ca.AccountTier,
        ca.ExternalAccountCode,
        ca.AccountManagerName,
        ca.StatusCode
    FROM dbo.CorporateAccount ca
    WHERE ca.AccountCode = @AccountCode;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubAccounts_Save', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubAccounts_Save;
GO

CREATE PROCEDURE dbo.usp_CustomerHubAccounts_Save
    @CorporateAccountId INT = NULL,
    @AccountCode NVARCHAR(20),
    @AccountName NVARCHAR(120),
    @PreferredPartnerAccountId INT = NULL,
    @BillingFrequency NVARCHAR(20),
    @ActiveFlag BIT,
    @AccountTier NVARCHAR(20),
    @ExternalAccountCode NVARCHAR(30) = NULL,
    @AccountManagerName NVARCHAR(120) = NULL,
    @StatusCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.CorporateAccount WHERE CorporateAccountId = ISNULL(@CorporateAccountId, -1) OR AccountCode = @AccountCode)
    BEGIN
        UPDATE dbo.CorporateAccount
        SET AccountCode = @AccountCode,
            AccountName = @AccountName,
            PreferredPartnerAccountId = @PreferredPartnerAccountId,
            BillingFrequency = @BillingFrequency,
            ActiveFlag = @ActiveFlag,
            AccountTier = @AccountTier,
            ExternalAccountCode = @ExternalAccountCode,
            AccountManagerName = @AccountManagerName,
            StatusCode = @StatusCode
        WHERE (@CorporateAccountId IS NOT NULL AND CorporateAccountId = @CorporateAccountId)
           OR (@CorporateAccountId IS NULL AND AccountCode = @AccountCode);
    END
    ELSE
    BEGIN
        INSERT INTO dbo.CorporateAccount
        (
            AccountCode,
            AccountName,
            PreferredPartnerAccountId,
            BillingFrequency,
            ActiveFlag,
            AccountTier,
            ExternalAccountCode,
            AccountManagerName,
            StatusCode
        )
        VALUES
        (
            @AccountCode,
            @AccountName,
            @PreferredPartnerAccountId,
            @BillingFrequency,
            @ActiveFlag,
            @AccountTier,
            @ExternalAccountCode,
            @AccountManagerName,
            @StatusCode
        );
    END

    SELECT TOP (1)
        ca.CorporateAccountId,
        ca.AccountCode,
        ca.AccountName,
        ca.PreferredPartnerAccountId,
        ca.BillingFrequency,
        ca.ActiveFlag,
        ca.AccountTier,
        ca.ExternalAccountCode,
        ca.AccountManagerName,
        ca.StatusCode
    FROM dbo.CorporateAccount ca
    WHERE ca.AccountCode = @AccountCode
    ORDER BY ca.CorporateAccountId DESC;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubAccounts_Deactivate', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubAccounts_Deactivate;
GO

CREATE PROCEDURE dbo.usp_CustomerHubAccounts_Deactivate
    @AccountCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CorporateAccount
    SET ActiveFlag = 0,
        StatusCode = N'Inactive'
    WHERE AccountCode = @AccountCode;

    SELECT
        ca.CorporateAccountId,
        ca.AccountCode,
        ca.AccountName,
        ca.PreferredPartnerAccountId,
        ca.BillingFrequency,
        ca.ActiveFlag,
        ca.AccountTier,
        ca.ExternalAccountCode,
        ca.AccountManagerName,
        ca.StatusCode
    FROM dbo.CorporateAccount ca
    WHERE ca.AccountCode = @AccountCode;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubReferrals_GetByPartnerCode', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubReferrals_GetByPartnerCode;
GO

CREATE PROCEDURE dbo.usp_CustomerHubReferrals_GetByPartnerCode
    @PartnerCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pr.PartnerReferralId,
        pr.ReferralCode,
        pr.PartnerAccountId,
        pr.CorporateAccountId,
        pr.ReferralChannel,
        pr.ReferrerName,
        pr.AttributionCode,
        pr.ReferredOn,
        pr.StatusCode,
        pr.Notes
    FROM dbo.PartnerReferral pr
    INNER JOIN dbo.PartnerAccount pa
        ON pa.PartnerAccountId = pr.PartnerAccountId
    WHERE pa.PartnerCode = @PartnerCode
    ORDER BY pr.ReferredOn DESC, pr.ReferralCode;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubReferrals_Save', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubReferrals_Save;
GO

CREATE PROCEDURE dbo.usp_CustomerHubReferrals_Save
    @PartnerReferralId INT = NULL,
    @ReferralCode NVARCHAR(30),
    @PartnerAccountId INT,
    @CorporateAccountId INT = NULL,
    @ReferralChannel NVARCHAR(40),
    @ReferrerName NVARCHAR(120),
    @AttributionCode NVARCHAR(30),
    @ReferredOn DATE,
    @StatusCode NVARCHAR(20),
    @Notes NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.PartnerReferral WHERE PartnerReferralId = ISNULL(@PartnerReferralId, -1) OR ReferralCode = @ReferralCode)
    BEGIN
        UPDATE dbo.PartnerReferral
        SET ReferralCode = @ReferralCode,
            PartnerAccountId = @PartnerAccountId,
            CorporateAccountId = @CorporateAccountId,
            ReferralChannel = @ReferralChannel,
            ReferrerName = @ReferrerName,
            AttributionCode = @AttributionCode,
            ReferredOn = @ReferredOn,
            StatusCode = @StatusCode,
            Notes = @Notes
        WHERE (@PartnerReferralId IS NOT NULL AND PartnerReferralId = @PartnerReferralId)
           OR (@PartnerReferralId IS NULL AND ReferralCode = @ReferralCode);
    END
    ELSE
    BEGIN
        INSERT INTO dbo.PartnerReferral
        (
            ReferralCode,
            PartnerAccountId,
            CorporateAccountId,
            ReferralChannel,
            ReferrerName,
            AttributionCode,
            ReferredOn,
            StatusCode,
            Notes
        )
        VALUES
        (
            @ReferralCode,
            @PartnerAccountId,
            @CorporateAccountId,
            @ReferralChannel,
            @ReferrerName,
            @AttributionCode,
            @ReferredOn,
            @StatusCode,
            @Notes
        );
    END

    SELECT TOP (1)
        pr.PartnerReferralId,
        pr.ReferralCode,
        pr.PartnerAccountId,
        pr.CorporateAccountId,
        pr.ReferralChannel,
        pr.ReferrerName,
        pr.AttributionCode,
        pr.ReferredOn,
        pr.StatusCode,
        pr.Notes
    FROM dbo.PartnerReferral pr
    WHERE pr.ReferralCode = @ReferralCode
    ORDER BY pr.PartnerReferralId DESC;
END
GO

IF OBJECT_ID(N'dbo.usp_CustomerHubReferrals_GetCommissionHistory', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CustomerHubReferrals_GetCommissionHistory;
GO

CREATE PROCEDURE dbo.usp_CustomerHubReferrals_GetCommissionHistory
    @PartnerCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        rch.ReferralCommissionHistoryId,
        rch.PartnerReferralId,
        rch.CommissionPeriodStart,
        rch.CommissionPeriodEnd,
        rch.CommissionAmount,
        rch.CommissionStatus,
        rch.PaidUtc
    FROM dbo.ReferralCommissionHistory rch
    INNER JOIN dbo.PartnerReferral pr
        ON pr.PartnerReferralId = rch.PartnerReferralId
    INNER JOIN dbo.PartnerAccount pa
        ON pa.PartnerAccountId = pr.PartnerAccountId
    WHERE pa.PartnerCode = @PartnerCode
    ORDER BY rch.CommissionPeriodStart DESC, rch.ReferralCommissionHistoryId DESC;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\04-service-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\04-service-procedures.sql');
END
GO
