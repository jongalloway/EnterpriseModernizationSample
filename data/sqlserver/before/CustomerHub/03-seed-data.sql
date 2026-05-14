USE [$(CustomerHubDatabase)];
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerAccount WHERE PartnerCode = N'CORP-1002')
BEGIN
    INSERT INTO dbo.PartnerAccount (PartnerCode, PartnerName, RelationshipTier, PreferredStoreNumber, StatusCode, LastContractRenewalDate, CreditHold)
    VALUES
        (N'CORP-1002', N'Contoso Office Parks', N'Gold', N'014', N'Active', DATEADD(DAY, -210, GETUTCDATE()), 0),
        (N'COMM-8821', N'Northwind Youth Sports League', N'Community', N'014', N'Active', DATEADD(DAY, -160, GETUTCDATE()), 0),
        (N'EVT-4405', N'Adventure Works Bike Expo', N'Seasonal', N'022', N'Active', DATEADD(DAY, -120, GETUTCDATE()), 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CorporateAccount WHERE AccountCode = N'CAT-0140')
BEGIN
    INSERT INTO dbo.CorporateAccount (AccountCode, AccountName, PreferredPartnerAccountId, BillingFrequency, ActiveFlag)
    SELECT N'CAT-0140', N'Fabrikam Regional Catering Desk', pa.PartnerAccountId, N'Monthly', 1
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'CORP-1002';

    INSERT INTO dbo.CorporateAccount (AccountCode, AccountName, PreferredPartnerAccountId, BillingFrequency, ActiveFlag)
    SELECT N'LEAGUE-8821', N'Northwind League Concessions', pa.PartnerAccountId, N'PerEvent', 1
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'COMM-8821';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContact)
BEGIN
    INSERT INTO dbo.PartnerContact (PartnerAccountId, ContactName, EmailAddress, PhoneNumber, IsPrimaryContact)
    SELECT PartnerAccountId, N'Kelly Vargas', N'kelly.vargas@contoso.example', N'555-0140', 1
    FROM dbo.PartnerAccount
    WHERE PartnerCode = N'CORP-1002'
    UNION ALL
    SELECT PartnerAccountId, N'Marcus Hale', N'marcus.hale@northwind.example', N'555-8821', 1
    FROM dbo.PartnerAccount
    WHERE PartnerCode = N'COMM-8821'
    UNION ALL
    SELECT PartnerAccountId, N'Anika Doyle', N'anika.doyle@adventureworks.example', N'555-4405', 1
    FROM dbo.PartnerAccount
    WHERE PartnerCode = N'EVT-4405';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\03-seed-data.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\03-seed-data.sql');
END
GO
