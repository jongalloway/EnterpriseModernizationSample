:on error exit
:setvar StoreOpsDatabase FabrikamPizza_StoreOps
:setvar CustomerHubDatabase FabrikamPizza_CustomerHub
:setvar ReportingDatabase FabrikamPizza_Reporting

PRINT 'Deploying Fabrikam Enterprise Pizza legacy databases.';
PRINT 'StoreOps database: $(StoreOpsDatabase)';
PRINT 'CustomerHub database: $(CustomerHubDatabase)';
PRINT 'Reporting database: $(ReportingDatabase)';

:r ..\CustomerHub\01-create-database.sql
:r ..\CustomerHub\02-schema.sql
:r ..\CustomerHub\03-seed-data.sql
:r ..\CustomerHub\04-service-procedures.sql
:r ..\CustomerHub\05-migration-procedures.sql

:r ..\StoreOps\01-create-database.sql
:r ..\StoreOps\02-schema.sql
:r ..\StoreOps\03-seed-data.sql
:r ..\StoreOps\04-service-procedures.sql
:r ..\StoreOps\05-migration-procedures.sql

:r ..\Reporting\01-create-database.sql
:r ..\Reporting\02-schema.sql
:r ..\Reporting\03-seed-data.sql
:r ..\Reporting\04-service-procedures.sql
:r ..\Reporting\05-migration-procedures.sql

PRINT 'Legacy database deployment scripts completed.';
