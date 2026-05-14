# Legacy SQL Server Assets

Use this folder for the pre-migration database estate.

## Layout

- `StoreOps\` - operational schema, seed data, service procedures, and partner-cache migration procedures
- `CustomerHub\` - customer and partner master data plus extraction procedures used by store and reporting jobs
- `Reporting\` - reporting schema plus nightly rollup procedures fed from StoreOps and CustomerHub
- `Deploy\` - SQLCMD entry points, Windows command wrappers, a shared invocation helper, and environment templates for first-time deployment and validation
- `..\shared\Migration\` - cross-database orchestration scripts that call the legacy procedures in the right order

## Deployment posture

This sample assumes a DBA runs SQLCMD-mode scripts from SSMS or `sqlcmd.exe`, database-by-database, with explicit review points between schema, seed, and migration steps. That is the sort of awkward but teachable deployment story the modernization pass is supposed to expose.

## Operator entry points

- `Deploy\01-deploy-all.cmd`
- `Deploy\02-run-nightly-sync.cmd`
- `Deploy\03-smoke-test.cmd`
- `Deploy\04-deployment-audit.cmd`
- `deployment-guide.md`
