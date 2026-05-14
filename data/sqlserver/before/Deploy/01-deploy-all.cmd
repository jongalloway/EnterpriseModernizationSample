@echo off
call "%~dp000-invoke-sqlcmd.cmd" "Deploying Fabrikam Enterprise Pizza databases on" "%~dp000-deploy-all.sql" "%~dp0"
exit /b %ERRORLEVEL%
