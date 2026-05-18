:on error exit
-- SQLCMD variable samples for manual SSMS runs:
-- :setvar StoreOpsDatabase FabrikamPizza_StoreOps
-- :setvar CustomerHubDatabase FabrikamPizza_CustomerHub
-- :setvar ReportingDatabase FabrikamPizza_Reporting
-- :setvar DeployRoot C:\path\to\data\sqlserver\before\Deploy

PRINT 'Deploying Fabrikam Enterprise Pizza legacy databases.';
PRINT 'StoreOps database: $(StoreOpsDatabase)';
PRINT 'CustomerHub database: $(CustomerHubDatabase)';
PRINT 'Reporting database: $(ReportingDatabase)';

:r "$(DeployRoot)\..\CustomerHub\01-create-database.sql"
:r "$(DeployRoot)\..\CustomerHub\02-schema.sql"
:r "$(DeployRoot)\..\CustomerHub\03-seed-data.sql"
:r "$(DeployRoot)\..\CustomerHub\06-b2b-partners-and-accounts.sql"
:r "$(DeployRoot)\..\CustomerHub\07-contracts-and-referrals.sql"
:r "$(DeployRoot)\..\CustomerHub\04-service-procedures.sql"
:r "$(DeployRoot)\..\CustomerHub\05-migration-procedures.sql"

:r "$(DeployRoot)\..\StoreOps\01-create-database.sql"
:r "$(DeployRoot)\..\StoreOps\02-schema.sql"
:r "$(DeployRoot)\..\StoreOps\03-seed-data.sql"
:r "$(DeployRoot)\..\StoreOps\03-menu-products.sql"
:r "$(DeployRoot)\..\StoreOps\03-store-configuration.sql"
:r "$(DeployRoot)\..\StoreOps\03-delivery-records.sql"
:r "$(DeployRoot)\..\StoreOps\04-service-procedures.sql"
:r "$(DeployRoot)\..\StoreOps\05-migration-procedures.sql"

:r "$(DeployRoot)\..\Reporting\01-create-database.sql"
:r "$(DeployRoot)\..\Reporting\02-schema.sql"
:r "$(DeployRoot)\..\Reporting\03-seed-data.sql"
:r "$(DeployRoot)\..\Reporting\04-service-procedures.sql"
:r "$(DeployRoot)\..\Reporting\05-migration-procedures.sql"

PRINT 'Legacy database deployment scripts completed.';
