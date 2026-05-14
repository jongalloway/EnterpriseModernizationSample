:setvar StoreOpsDatabase FabrikamPizza_StoreOps
:setvar CustomerHubDatabase FabrikamPizza_CustomerHub
:setvar ReportingDatabase FabrikamPizza_Reporting
:setvar DeployRoot .

PRINT 'Deploying Fabrikam Enterprise Pizza legacy databases.';

:r "$(DeployRoot)\..\CustomerHub\01-create-database.sql"
:r "$(DeployRoot)\..\CustomerHub\02-schema.sql"
:r "$(DeployRoot)\..\CustomerHub\03-seed-data.sql"
:r "$(DeployRoot)\..\CustomerHub\04-service-procedures.sql"
:r "$(DeployRoot)\..\CustomerHub\05-migration-procedures.sql"

:r "$(DeployRoot)\..\StoreOps\01-create-database.sql"
:r "$(DeployRoot)\..\StoreOps\02-schema.sql"
:r "$(DeployRoot)\..\StoreOps\03-seed-data.sql"
:r "$(DeployRoot)\..\StoreOps\04-service-procedures.sql"
:r "$(DeployRoot)\..\StoreOps\05-migration-procedures.sql"

:r "$(DeployRoot)\..\Reporting\01-create-database.sql"
:r "$(DeployRoot)\..\Reporting\02-schema.sql"
:r "$(DeployRoot)\..\Reporting\03-seed-data.sql"
:r "$(DeployRoot)\..\Reporting\04-service-procedures.sql"
:r "$(DeployRoot)\..\Reporting\05-migration-procedures.sql"

PRINT 'Legacy database deployment scripts completed.';
