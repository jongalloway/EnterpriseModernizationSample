:on error exit
-- SQLCMD variable samples for manual SSMS runs:
-- :setvar StoreOpsDatabase FabrikamPizza_StoreOps
-- :setvar CustomerHubDatabase FabrikamPizza_CustomerHub
-- :setvar ReportingDatabase FabrikamPizza_Reporting

PRINT 'Deployment audit: deployment history by database';

SELECT
    N'StoreOps' AS DatabaseArea,
    COUNT(*) AS AppliedScriptCount,
    MAX(AppliedUtc) AS LastAppliedUtc
FROM [$(StoreOpsDatabase)].dbo.DatabaseDeploymentHistory
UNION ALL
SELECT
    N'CustomerHub',
    COUNT(*),
    MAX(AppliedUtc)
FROM [$(CustomerHubDatabase)].dbo.DatabaseDeploymentHistory
UNION ALL
SELECT
    N'Reporting',
    COUNT(*),
    MAX(AppliedUtc)
FROM [$(ReportingDatabase)].dbo.DatabaseDeploymentHistory;

PRINT 'Deployment audit: cross-database bridge data';

SELECT
    COUNT(*) AS PartnerAccountCacheRows,
    MAX(LastSyncUtc) AS LastPartnerSyncUtc
FROM [$(StoreOpsDatabase)].dbo.PartnerAccountCache;

SELECT TOP (5)
    ReportingBatchRunId,
    SyncBatchId,
    SummaryDate,
    StartedUtc,
    CompletedUtc,
    RunStatus
FROM [$(ReportingDatabase)].dbo.ReportingBatchRun
ORDER BY ReportingBatchRunId DESC;

SELECT
    N'DimStore' AS ReportingObject,
    COUNT(*) AS TotalRows
FROM [$(ReportingDatabase)].dbo.DimStore
UNION ALL
SELECT N'DimDriver', COUNT(*) FROM [$(ReportingDatabase)].dbo.DimDriver
UNION ALL
SELECT N'DimPartner', COUNT(*) FROM [$(ReportingDatabase)].dbo.DimPartner
UNION ALL
SELECT N'DimTime', COUNT(*) FROM [$(ReportingDatabase)].dbo.DimTime
UNION ALL
SELECT N'DimProduct', COUNT(*) FROM [$(ReportingDatabase)].dbo.DimProduct
UNION ALL
SELECT N'FactOrder', COUNT(*) FROM [$(ReportingDatabase)].dbo.FactOrder
UNION ALL
SELECT N'FactDelivery', COUNT(*) FROM [$(ReportingDatabase)].dbo.FactDelivery
UNION ALL
SELECT N'FactPartnerRevenue', COUNT(*) FROM [$(ReportingDatabase)].dbo.FactPartnerRevenue
UNION ALL
SELECT N'DeliveryDailySummary', COUNT(*) FROM [$(ReportingDatabase)].dbo.DeliveryDailySummary
UNION ALL
SELECT N'PartnerProfitabilitySummary', COUNT(*) FROM [$(ReportingDatabase)].dbo.PartnerProfitabilitySummary;


