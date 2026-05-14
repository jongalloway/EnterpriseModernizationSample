@echo off
call "%~dp000-invoke-sqlcmd.cmd" "Running smoke test against" "%~dp0..\..\shared\Migration\02-smoke-test.sql"
exit /b %ERRORLEVEL%
