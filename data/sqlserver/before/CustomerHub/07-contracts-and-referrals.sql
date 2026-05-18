USE [$(CustomerHubDatabase)];
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContract WHERE ContractCode = N'CT-1001-2026')
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
    SELECT
        N'CT-1001-2026',
        pa.PartnerAccountId,
        ca.CorporateAccountId,
        fl.FranchiseLocationId,
        N'ChainSupply',
        N'Airport Plaza Master Pricing',
        N'ChannelManager',
        '2026-01-01',
        '2026-12-31',
        425.00,
        10.50,
        4,
        N'Active',
        '2026-05-14T09:30:00.000'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.CorporateAccount ca ON ca.AccountCode = N'CHAIN-1001'
    INNER JOIN dbo.FranchiseLocation fl ON fl.FranchiseCode = N'FRN-031'
    WHERE pa.PartnerCode = N'PCHAIN-1001';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContract WHERE ContractCode = N'CT-2400-2026')
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
    SELECT
        N'CT-2400-2026',
        pa.PartnerAccountId,
        ca.CorporateAccountId,
        fl.FranchiseLocationId,
        N'CateringProgram',
        N'Executive Catering Launch',
        N'FieldSales',
        '2026-06-01',
        '2027-05-31',
        250.00,
        7.75,
        8,
        N'PendingApproval',
        '2026-05-18T01:39:47.894'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.CorporateAccount ca ON ca.AccountCode = N'CAT-2400'
    INNER JOIN dbo.FranchiseLocation fl ON fl.FranchiseCode = N'FRN-014'
    WHERE pa.PartnerCode = N'CATER-2400';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContract WHERE ContractCode = N'CT-3300-2025')
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
    SELECT
        N'CT-3300-2025',
        pa.PartnerAccountId,
        ca.CorporateAccountId,
        fl.FranchiseLocationId,
        N'CorporateDining',
        N'Campus Bronze Plan',
        N'PartnerReferral',
        '2025-09-01',
        '2026-08-31',
        180.00,
        4.25,
        12,
        N'Suspended',
        '2026-05-03T16:15:00.000'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.CorporateAccount ca ON ca.AccountCode = N'B2B-3300'
    INNER JOIN dbo.FranchiseLocation fl ON fl.FranchiseCode = N'FRN-022'
    WHERE pa.PartnerCode = N'CORP-3300';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContract WHERE ContractCode = N'CT-3300-2024')
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
    SELECT
        N'CT-3300-2024',
        pa.PartnerAccountId,
        ca.CorporateAccountId,
        fl.FranchiseLocationId,
        N'CorporateDining',
        N'Campus Bronze Legacy',
        N'PartnerReferral',
        '2024-09-01',
        '2025-08-31',
        165.00,
        3.75,
        12,
        N'Expired',
        '2025-08-31T23:59:00.000'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.CorporateAccount ca ON ca.AccountCode = N'B2B-3300'
    INNER JOIN dbo.FranchiseLocation fl ON fl.FranchiseCode = N'FRN-022'
    WHERE pa.PartnerCode = N'CORP-3300';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerReferral WHERE ReferralCode = N'REF-1001-ALPHA')
BEGIN
    INSERT INTO dbo.PartnerReferral (ReferralCode, PartnerAccountId, CorporateAccountId, ReferralChannel, ReferrerName, AttributionCode, ReferredOn, StatusCode, Notes)
    SELECT N'REF-1001-ALPHA', pa.PartnerAccountId, ca.CorporateAccountId, N'ChannelManager', N'Daniela Cross', N'ATR-ALPHA', '2026-01-12', N'Converted', N'Airport plaza launch lead converted to annual pricing.'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.CorporateAccount ca ON ca.AccountCode = N'CHAIN-1001'
    WHERE pa.PartnerCode = N'PCHAIN-1001';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerReferral WHERE ReferralCode = N'REF-2400-BETA')
BEGIN
    INSERT INTO dbo.PartnerReferral (ReferralCode, PartnerAccountId, CorporateAccountId, ReferralChannel, ReferrerName, AttributionCode, ReferredOn, StatusCode, Notes)
    SELECT N'REF-2400-BETA', pa.PartnerAccountId, ca.CorporateAccountId, N'FieldSales', N'Samira Khan', N'ATR-BETA', '2026-04-02', N'PendingCommission', N'Executive lunch program awaiting first quarterly commission run.'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.CorporateAccount ca ON ca.AccountCode = N'CAT-2400'
    WHERE pa.PartnerCode = N'CATER-2400';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerReferral WHERE ReferralCode = N'REF-3300-GAMMA')
BEGIN
    INSERT INTO dbo.PartnerReferral (ReferralCode, PartnerAccountId, CorporateAccountId, ReferralChannel, ReferrerName, AttributionCode, ReferredOn, StatusCode, Notes)
    SELECT N'REF-3300-GAMMA', pa.PartnerAccountId, ca.CorporateAccountId, N'PartnerReferral', N'Malcolm Ives', N'ATR-GAMMA', '2025-10-10', N'AtRisk', N'Campus dining expansion is paused while the suspended contract is reviewed.'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.CorporateAccount ca ON ca.AccountCode = N'B2B-3300'
    WHERE pa.PartnerCode = N'CORP-3300';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ReferralCommissionHistory WHERE PartnerReferralId = (SELECT TOP (1) PartnerReferralId FROM dbo.PartnerReferral WHERE ReferralCode = N'REF-1001-ALPHA') AND CommissionPeriodStart = '2026-02-01')
BEGIN
    INSERT INTO dbo.ReferralCommissionHistory (PartnerReferralId, CommissionPeriodStart, CommissionPeriodEnd, CommissionAmount, CommissionStatus, PaidUtc)
    SELECT PartnerReferralId, '2026-02-01', '2026-02-28', 1825.00, N'Paid', '2026-03-15T10:00:00.000'
    FROM dbo.PartnerReferral
    WHERE ReferralCode = N'REF-1001-ALPHA';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ReferralCommissionHistory WHERE PartnerReferralId = (SELECT TOP (1) PartnerReferralId FROM dbo.PartnerReferral WHERE ReferralCode = N'REF-1001-ALPHA') AND CommissionPeriodStart = '2026-03-01')
BEGIN
    INSERT INTO dbo.ReferralCommissionHistory (PartnerReferralId, CommissionPeriodStart, CommissionPeriodEnd, CommissionAmount, CommissionStatus, PaidUtc)
    SELECT PartnerReferralId, '2026-03-01', '2026-03-31', 1995.00, N'Paid', '2026-04-15T10:00:00.000'
    FROM dbo.PartnerReferral
    WHERE ReferralCode = N'REF-1001-ALPHA';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ReferralCommissionHistory WHERE PartnerReferralId = (SELECT TOP (1) PartnerReferralId FROM dbo.PartnerReferral WHERE ReferralCode = N'REF-2400-BETA') AND CommissionPeriodStart = '2026-04-01')
BEGIN
    INSERT INTO dbo.ReferralCommissionHistory (PartnerReferralId, CommissionPeriodStart, CommissionPeriodEnd, CommissionAmount, CommissionStatus, PaidUtc)
    SELECT PartnerReferralId, '2026-04-01', '2026-04-30', 640.00, N'Accrued', NULL
    FROM dbo.PartnerReferral
    WHERE ReferralCode = N'REF-2400-BETA';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ReferralCommissionHistory WHERE PartnerReferralId = (SELECT TOP (1) PartnerReferralId FROM dbo.PartnerReferral WHERE ReferralCode = N'REF-3300-GAMMA') AND CommissionPeriodStart = '2026-01-01')
BEGIN
    INSERT INTO dbo.ReferralCommissionHistory (PartnerReferralId, CommissionPeriodStart, CommissionPeriodEnd, CommissionAmount, CommissionStatus, PaidUtc)
    SELECT PartnerReferralId, '2026-01-01', '2026-01-31', 275.00, N'OnHold', NULL
    FROM dbo.PartnerReferral
    WHERE ReferralCode = N'REF-3300-GAMMA';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\07-contracts-and-referrals.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\07-contracts-and-referrals.sql');
END
GO
