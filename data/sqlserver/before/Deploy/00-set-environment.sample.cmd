@echo off
rem Copy this file to 00-set-environment.cmd and edit the values for your SQL Server.
rem Use quoted set syntax if the password includes shell metacharacters such as ^&, |, <, or >.

set "LEGACY_SQL_SERVER=(local)"
set "LEGACY_SQL_AUTH_MODE=integrated"
set "LEGACY_SQL_USER="
set "LEGACY_SQL_PASSWORD="

set "StoreOpsDatabase=FabrikamPizza_StoreOps"
set "CustomerHubDatabase=FabrikamPizza_CustomerHub"
set "ReportingDatabase=FabrikamPizza_Reporting"

exit /b 0
