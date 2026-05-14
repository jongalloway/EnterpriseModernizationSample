@echo off
setlocal

rem Copy this file to 00-set-environment.cmd and edit the values for your SQL Server.

set LEGACY_SQL_SERVER=(local)
set LEGACY_SQL_AUTH_MODE=integrated
set LEGACY_SQL_USER=
set LEGACY_SQL_PASSWORD=

set StoreOpsDatabase=FabrikamPizza_StoreOps
set CustomerHubDatabase=FabrikamPizza_CustomerHub
set ReportingDatabase=FabrikamPizza_Reporting

endlocal & (
    set LEGACY_SQL_SERVER=%LEGACY_SQL_SERVER%
    set LEGACY_SQL_AUTH_MODE=%LEGACY_SQL_AUTH_MODE%
    set LEGACY_SQL_USER=%LEGACY_SQL_USER%
    set LEGACY_SQL_PASSWORD=%LEGACY_SQL_PASSWORD%
    set StoreOpsDatabase=%StoreOpsDatabase%
    set CustomerHubDatabase=%CustomerHubDatabase%
    set ReportingDatabase=%ReportingDatabase%
)
