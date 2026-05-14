:on error exit
:setvar StoreOpsDatabase FabrikamPizza_StoreOps
:setvar CustomerHubDatabase FabrikamPizza_CustomerHub
:setvar ReportingDatabase FabrikamPizza_Reporting

DECLARE @SyncBatchId UNIQUEIDENTIFIER;

EXEC [$(CustomerHubDatabase)].dbo.usp_PartnerSync_BuildStoreOpsExtract
    @SyncBatchId = @SyncBatchId OUTPUT;

EXEC [$(StoreOpsDatabase)].dbo.usp_PartnerSync_ApplyCustomerHubExtract
    @SyncBatchId = @SyncBatchId;

EXEC [$(ReportingDatabase)].dbo.usp_Reporting_NightlyRebuild
    @SummaryDate = NULL,
    @SyncBatchId = @SyncBatchId;
