@echo off
call "%~dp000-invoke-sqlcmd.cmd" "Capturing deployment audit from" "%~dp0..\..\shared\Migration\03-deployment-audit.sql"
exit /b %ERRORLEVEL%
