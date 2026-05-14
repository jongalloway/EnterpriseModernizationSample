@echo off
setlocal EnableExtensions

set SCRIPT_DIR=%~dp0
if not exist "%SCRIPT_DIR%00-set-environment.cmd" (
    echo Missing %SCRIPT_DIR%00-set-environment.cmd
    echo Copy 00-set-environment.sample.cmd to 00-set-environment.cmd and set your SQL Server values.
    exit /b 1
)

call "%SCRIPT_DIR%00-set-environment.cmd"
if errorlevel 1 exit /b %errorlevel%

set SQLCMD_AUTH=-E
if /I "%LEGACY_SQL_AUTH_MODE%"=="sql" (
    set SQLCMD_AUTH=-U "%LEGACY_SQL_USER%" -P "%LEGACY_SQL_PASSWORD%"
)

echo Running smoke test against %LEGACY_SQL_SERVER%.
sqlcmd -b -l 5 -S "%LEGACY_SQL_SERVER%" -d master %SQLCMD_AUTH% -i "%SCRIPT_DIR%..\\..\\shared\\Migration\\02-smoke-test.sql" -v StoreOpsDatabase="%StoreOpsDatabase%" CustomerHubDatabase="%CustomerHubDatabase%" ReportingDatabase="%ReportingDatabase%"
exit /b %errorlevel%
