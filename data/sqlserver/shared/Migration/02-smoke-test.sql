:setvar StoreOpsDatabase FabrikamPizza_StoreOps
:setvar CustomerHubDatabase FabrikamPizza_CustomerHub
:setvar ReportingDatabase FabrikamPizza_Reporting

PRINT 'Smoke test: preferred partners';
EXEC [$(CustomerHubDatabase)].dbo.usp_CorporateAccounts_GetPreferredPartners;

PRINT 'Smoke test: dispatch board';
EXEC [$(StoreOpsDatabase)].dbo.usp_DispatchBoard_GetActiveTickets @StoreNumber = N'014';

PRINT 'Smoke test: reporting dashboard';
EXEC [$(ReportingDatabase)].dbo.usp_DeliveryDashboard_GetDailySummary @StoreNumber = N'014', @SummaryDate = NULL;

PRINT 'Smoke test: labor cost';
EXEC [$(ReportingDatabase)].dbo.usp_WorkforceReports_GetLaborCostSummary @StoreNumber = N'014', @SummaryDate = NULL;

PRINT 'Smoke test: overtime trend';
EXEC [$(ReportingDatabase)].dbo.usp_WorkforceReports_GetOvertimeTrend @StoreNumber = N'014', @WeeksBack = 4;

PRINT 'Smoke test: turnover summary';
EXEC [$(ReportingDatabase)].dbo.usp_WorkforceReports_GetTurnoverSummary @StoreNumber = N'014', @SummaryMonth = NULL;

PRINT 'Smoke test: staffing summary';
EXEC [$(ReportingDatabase)].dbo.usp_WorkforceReports_GetStaffingSummary @StoreNumber = N'014', @SummaryDate = NULL;

SELECT 'StoreOps deployment history' AS CheckName, COUNT(*) AS RowCount
FROM [$(StoreOpsDatabase)].dbo.DatabaseDeploymentHistory
UNION ALL
SELECT 'CustomerHub deployment history', COUNT(*)
FROM [$(CustomerHubDatabase)].dbo.DatabaseDeploymentHistory
UNION ALL
SELECT 'Reporting deployment history', COUNT(*)
FROM [$(ReportingDatabase)].dbo.DatabaseDeploymentHistory;
