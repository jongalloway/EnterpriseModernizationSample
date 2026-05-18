USE [$(CustomerHubDatabase)];
GO

UPDATE dbo.CorporateAccount
SET AccountTier = N'Gold',
    ExternalAccountCode = N'EXT-CAT-0140',
    AccountManagerName = N'Kelly Vargas',
    StatusCode = N'Active'
WHERE AccountCode = N'CAT-0140';
GO

UPDATE dbo.CorporateAccount
SET AccountTier = N'Silver',
    ExternalAccountCode = N'EXT-LEAGUE-8821',
    AccountManagerName = N'Marcus Hale',
    StatusCode = N'Active'
WHERE AccountCode = N'LEAGUE-8821';
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerAccount WHERE PartnerCode = N'PCHAIN-1001')
BEGIN
    INSERT INTO dbo.PartnerAccount (PartnerCode, PartnerName, RelationshipTier, PreferredStoreNumber, StatusCode, LastContractRenewalDate, CreditHold)
    VALUES (N'PCHAIN-1001', N'Blue Yonder Pizza Express', N'Platinum', N'031', N'Active', '2026-02-14', 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerAccount WHERE PartnerCode = N'CATER-2400')
BEGIN
    INSERT INTO dbo.PartnerAccount (PartnerCode, PartnerName, RelationshipTier, PreferredStoreNumber, StatusCode, LastContractRenewalDate, CreditHold)
    VALUES (N'CATER-2400', N'Fourth Coffee Catering Collective', N'Gold', N'014', N'Active', '2025-11-30', 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerAccount WHERE PartnerCode = N'CORP-3300')
BEGIN
    INSERT INTO dbo.PartnerAccount (PartnerCode, PartnerName, RelationshipTier, PreferredStoreNumber, StatusCode, LastContractRenewalDate, CreditHold)
    VALUES (N'CORP-3300', N'Litware Corporate Dining', N'Silver', N'022', N'Active', '2025-09-15', 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContact WHERE EmailAddress = N'daniela.cross@blueyonder.example')
BEGIN
    INSERT INTO dbo.PartnerContact (PartnerAccountId, ContactName, EmailAddress, PhoneNumber, IsPrimaryContact)
    SELECT PartnerAccountId, N'Daniela Cross', N'daniela.cross@blueyonder.example', N'555-3101', 1
    FROM dbo.PartnerAccount
    WHERE PartnerCode = N'PCHAIN-1001';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContact WHERE EmailAddress = N'samira.khan@fourthcoffee.example')
BEGIN
    INSERT INTO dbo.PartnerContact (PartnerAccountId, ContactName, EmailAddress, PhoneNumber, IsPrimaryContact)
    SELECT PartnerAccountId, N'Samira Khan', N'samira.khan@fourthcoffee.example', N'555-2400', 1
    FROM dbo.PartnerAccount
    WHERE PartnerCode = N'CATER-2400';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PartnerContact WHERE EmailAddress = N'malcolm.ives@litware.example')
BEGIN
    INSERT INTO dbo.PartnerContact (PartnerAccountId, ContactName, EmailAddress, PhoneNumber, IsPrimaryContact)
    SELECT PartnerAccountId, N'Malcolm Ives', N'malcolm.ives@litware.example', N'555-3300', 1
    FROM dbo.PartnerAccount
    WHERE PartnerCode = N'CORP-3300';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.FranchiseLocation WHERE FranchiseCode = N'FRN-031')
BEGIN
    INSERT INTO dbo.FranchiseLocation
    (
        FranchiseCode,
        FranchiseName,
        PrimaryStoreNumber,
        MarketName,
        OwnerName,
        PrimaryPartnerAccountId,
        StatusCode,
        LastPortalSyncUtc
    )
    SELECT
        N'FRN-031',
        N'Blue Yonder Airport Franchise',
        N'031',
        N'Airport Connector',
        N'Celeste Moreno',
        pa.PartnerAccountId,
        N'Active',
        '2026-05-18T01:39:47.894'
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'PCHAIN-1001';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CorporateAccount WHERE AccountCode = N'CHAIN-1001')
BEGIN
    INSERT INTO dbo.CorporateAccount (AccountCode, AccountName, PreferredPartnerAccountId, BillingFrequency, ActiveFlag, AccountTier, ExternalAccountCode, AccountManagerName, StatusCode)
    SELECT N'CHAIN-1001', N'Blue Yonder Travel Plazas', pa.PartnerAccountId, N'Weekly', 1, N'Platinum', N'EXT-CHAIN-1001', N'Daniela Cross', N'Active'
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'PCHAIN-1001';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CorporateAccount WHERE AccountCode = N'CAT-2400')
BEGIN
    INSERT INTO dbo.CorporateAccount (AccountCode, AccountName, PreferredPartnerAccountId, BillingFrequency, ActiveFlag, AccountTier, ExternalAccountCode, AccountManagerName, StatusCode)
    SELECT N'CAT-2400', N'Fourth Coffee Event Catering', pa.PartnerAccountId, N'Monthly', 1, N'Gold', N'EXT-CAT-2400', N'Samira Khan', N'Active'
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'CATER-2400';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CorporateAccount WHERE AccountCode = N'B2B-3300')
BEGIN
    INSERT INTO dbo.CorporateAccount (AccountCode, AccountName, PreferredPartnerAccountId, BillingFrequency, ActiveFlag, AccountTier, ExternalAccountCode, AccountManagerName, StatusCode)
    SELECT N'B2B-3300', N'Litware Campus Dining Program', pa.PartnerAccountId, N'Quarterly', 1, N'Silver', N'EXT-B2B-3300', N'Malcolm Ives', N'Active'
    FROM dbo.PartnerAccount pa
    WHERE pa.PartnerCode = N'CORP-3300';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\06-b2b-partners-and-accounts.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\06-b2b-partners-and-accounts.sql');
END
GO
