USE [$(CustomerHubDatabase)];
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerAccount WHERE PartnerCode = N'CORP-1002')
BEGIN
    INSERT INTO dbo.PartnerAccount (PartnerCode, PartnerName, RelationshipTier, PreferredStoreNumber, StatusCode, LastContractRenewalDate, CreditHold)
    VALUES (N'CORP-1002', N'Contoso Office Parks', N'Gold', N'014', N'Active', DATEADD(DAY, -210, GETUTCDATE()), 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerAccount WHERE PartnerCode = N'COMM-8821')
BEGIN
    INSERT INTO dbo.PartnerAccount (PartnerCode, PartnerName, RelationshipTier, PreferredStoreNumber, StatusCode, LastContractRenewalDate, CreditHold)
    VALUES (N'COMM-8821', N'Northwind Youth Sports League', N'Community', N'014', N'Active', DATEADD(DAY, -160, GETUTCDATE()), 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerAccount WHERE PartnerCode = N'EVT-4405')
BEGIN
    INSERT INTO dbo.PartnerAccount (PartnerCode, PartnerName, RelationshipTier, PreferredStoreNumber, StatusCode, LastContractRenewalDate, CreditHold)
    VALUES (N'EVT-4405', N'Adventure Works Bike Expo', N'Seasonal', N'022', N'Active', DATEADD(DAY, -120, GETUTCDATE()), 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CorporateAccount WHERE AccountCode = N'CAT-0140')
BEGIN
    INSERT INTO dbo.CorporateAccount (AccountCode, AccountName, PreferredPartnerAccountId, BillingFrequency, ActiveFlag)
    SELECT N'CAT-0140', N'Fabrikam Regional Catering Desk', pa.PartnerAccountId, N'Monthly', 1
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'CORP-1002';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CorporateAccount WHERE AccountCode = N'LEAGUE-8821')
BEGIN
    INSERT INTO dbo.CorporateAccount (AccountCode, AccountName, PreferredPartnerAccountId, BillingFrequency, ActiveFlag)
    SELECT N'LEAGUE-8821', N'Northwind League Concessions', pa.PartnerAccountId, N'PerEvent', 1
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'COMM-8821';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContact WHERE EmailAddress = N'kelly.vargas@contoso.example')
BEGIN
    INSERT INTO dbo.PartnerContact (PartnerAccountId, ContactName, EmailAddress, PhoneNumber, IsPrimaryContact)
    SELECT PartnerAccountId, N'Kelly Vargas', N'kelly.vargas@contoso.example', N'555-0140', 1
    FROM dbo.PartnerAccount
    WHERE PartnerCode = N'CORP-1002';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContact WHERE EmailAddress = N'marcus.hale@northwind.example')
BEGIN
    INSERT INTO dbo.PartnerContact (PartnerAccountId, ContactName, EmailAddress, PhoneNumber, IsPrimaryContact)
    SELECT PartnerAccountId, N'Marcus Hale', N'marcus.hale@northwind.example', N'555-8821', 1
    FROM dbo.PartnerAccount
    WHERE PartnerCode = N'COMM-8821';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContact WHERE EmailAddress = N'anika.doyle@adventureworks.example')
BEGIN
    INSERT INTO dbo.PartnerContact (PartnerAccountId, ContactName, EmailAddress, PhoneNumber, IsPrimaryContact)
    SELECT PartnerAccountId, N'Anika Doyle', N'anika.doyle@adventureworks.example', N'555-4405', 1
    FROM dbo.PartnerAccount
    WHERE PartnerCode = N'EVT-4405';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.FranchiseLocation WHERE FranchiseCode = N'FRN-014')
BEGIN
    INSERT INTO dbo.FranchiseLocation
    (
        FranchiseCode,
        FranchiseName,
        PrimaryStoreNumber,
        MarketName,
        OwnerName,
        PrimaryPartnerAccountId,
        StatusCode
    )
    SELECT
        N'FRN-014',
        N'Northwest Corporate Corridor Franchise',
        N'014',
        N'Northwest Corridor',
        N'Renee Alvarez',
        pa.PartnerAccountId,
        N'Active'
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'CORP-1002';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.FranchiseLocation WHERE FranchiseCode = N'FRN-022')
BEGIN
    INSERT INTO dbo.FranchiseLocation
    (
        FranchiseCode,
        FranchiseName,
        PrimaryStoreNumber,
        MarketName,
        OwnerName,
        PrimaryPartnerAccountId,
        StatusCode
    )
    SELECT
        N'FRN-022',
        N'Eastside Events Franchise',
        N'022',
        N'Eastside Expo Belt',
        N'Noah Whitaker',
        pa.PartnerAccountId,
        N'Active'
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'EVT-4405';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContract WHERE ContractCode = N'CT-014-CAT')
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
        StatusCode
    )
    SELECT
        N'CT-014-CAT',
        pa.PartnerAccountId,
        ca.CorporateAccountId,
        fl.FranchiseLocationId,
        N'CorporateCatering',
        N'Northwest Gold Catering',
        N'FieldSales',
        CONVERT(DATE, DATEADD(DAY, -210, GETUTCDATE())),
        CONVERT(DATE, DATEADD(DAY, 155, GETUTCDATE())),
        150.00,
        8.50,
        6,
        N'Active'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.CorporateAccount ca
        ON ca.AccountCode = N'CAT-0140'
    INNER JOIN dbo.FranchiseLocation fl
        ON fl.FranchiseCode = N'FRN-014'
    WHERE pa.PartnerCode = N'CORP-1002';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContract WHERE ContractCode = N'CT-014-LEAGUE')
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
        StatusCode
    )
    SELECT
        N'CT-014-LEAGUE',
        pa.PartnerAccountId,
        ca.CorporateAccountId,
        fl.FranchiseLocationId,
        N'CommunityProgram',
        N'League Concessions',
        N'FranchiseReferral',
        CONVERT(DATE, DATEADD(DAY, -160, GETUTCDATE())),
        CONVERT(DATE, DATEADD(DAY, 95, GETUTCDATE())),
        75.00,
        5.00,
        12,
        N'Active'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.CorporateAccount ca
        ON ca.AccountCode = N'LEAGUE-8821'
    INNER JOIN dbo.FranchiseLocation fl
        ON fl.FranchiseCode = N'FRN-014'
    WHERE pa.PartnerCode = N'COMM-8821';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContract WHERE ContractCode = N'CT-022-EVT')
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
        StatusCode
    )
    SELECT
        N'CT-022-EVT',
        pa.PartnerAccountId,
        NULL,
        fl.FranchiseLocationId,
        N'EventSupport',
        N'Expo Seasonal Package',
        N'EventBroker',
        CONVERT(DATE, DATEADD(DAY, -120, GETUTCDATE())),
        CONVERT(DATE, DATEADD(DAY, 45, GETUTCDATE())),
        300.00,
        4.00,
        24,
        N'Active'
    FROM dbo.PartnerAccount pa
    INNER JOIN dbo.FranchiseLocation fl
        ON fl.FranchiseCode = N'FRN-022'
    WHERE pa.PartnerCode = N'EVT-4405';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\03-seed-data.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\03-seed-data.sql');
END
GO
