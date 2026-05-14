:on error exit
:setvar StoreOpsDatabase FabrikamPizza_StoreOps
:setvar CustomerHubDatabase FabrikamPizza_CustomerHub
:setvar ReportingDatabase FabrikamPizza_Reporting

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
    COUNT(*) AS DeliveryDailySummaryRows
FROM [$(ReportingDatabase)].dbo.DeliveryDailySummary;

SELECT
    COUNT(*) AS PartnerProfitabilitySummaryRows
FROM [$(ReportingDatabase)].dbo.PartnerProfitabilitySummary;
