@echo off
setlocal EnableExtensions

if "%~2"=="" (
    echo Usage: %~nx0 "status message" "sql script path" ["deploy root"]
    exit /b 1
)

set "STATUS_MESSAGE=%~1"
set "SQL_SCRIPT=%~2"
set "DEPLOY_ROOT=%~3"
set "SCRIPT_DIR=%~dp0"
set "ENV_FILE=%SCRIPT_DIR%00-set-environment.cmd"

if not exist "%ENV_FILE%" (
    echo Missing %ENV_FILE%
    echo Copy 00-set-environment.sample.cmd to 00-set-environment.cmd and set your SQL Server values.
    exit /b 1
)

call "%ENV_FILE%"
if errorlevel 1 exit /b %errorlevel%

if not defined LEGACY_SQL_SERVER (
    echo LEGACY_SQL_SERVER is required.
    exit /b 1
)

if /I "%LEGACY_SQL_AUTH_MODE%"=="sql" if not defined LEGACY_SQL_USER (
    echo LEGACY_SQL_USER is required when LEGACY_SQL_AUTH_MODE=sql.
    exit /b 1
)

echo %STATUS_MESSAGE% %LEGACY_SQL_SERVER%.

if /I "%LEGACY_SQL_AUTH_MODE%"=="sql" (
    set "SQLCMDPASSWORD=%LEGACY_SQL_PASSWORD%"
    if defined DEPLOY_ROOT (
        sqlcmd -b -l 5 -S "%LEGACY_SQL_SERVER%" -d master -U "%LEGACY_SQL_USER%" -i "%SQL_SCRIPT%" -v DeployRoot="%DEPLOY_ROOT%" StoreOpsDatabase="%StoreOpsDatabase%" CustomerHubDatabase="%CustomerHubDatabase%" ReportingDatabase="%ReportingDatabase%"
    ) else (
        sqlcmd -b -l 5 -S "%LEGACY_SQL_SERVER%" -d master -U "%LEGACY_SQL_USER%" -i "%SQL_SCRIPT%" -v StoreOpsDatabase="%StoreOpsDatabase%" CustomerHubDatabase="%CustomerHubDatabase%" ReportingDatabase="%ReportingDatabase%"
    )
    set "EXITCODE=%ERRORLEVEL%"
    endlocal & exit /b %EXITCODE%
)

if defined DEPLOY_ROOT (
    sqlcmd -b -l 5 -S "%LEGACY_SQL_SERVER%" -d master -E -i "%SQL_SCRIPT%" -v DeployRoot="%DEPLOY_ROOT%" StoreOpsDatabase="%StoreOpsDatabase%" CustomerHubDatabase="%CustomerHubDatabase%" ReportingDatabase="%ReportingDatabase%"
) else (
    sqlcmd -b -l 5 -S "%LEGACY_SQL_SERVER%" -d master -E -i "%SQL_SCRIPT%" -v StoreOpsDatabase="%StoreOpsDatabase%" CustomerHubDatabase="%CustomerHubDatabase%" ReportingDatabase="%ReportingDatabase%"
)

set "EXITCODE=%ERRORLEVEL%"
endlocal & exit /b %EXITCODE%
