@echo off
call "%~dp000-invoke-sqlcmd.cmd" "Running nightly sync bridge on" "%~dp0..\..\shared\Migration\01-run-nightly-sync.sql"
exit /b %ERRORLEVEL%
