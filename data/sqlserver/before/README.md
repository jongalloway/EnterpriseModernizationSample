# Legacy SQL Server Assets

Use this folder for the pre-migration database estate.

## Layout

- `StoreOps\` - operational schema, seed data, service procedures, and partner-cache migration procedures
- `CustomerHub\` - customer and partner master data plus extraction procedures used by store and reporting jobs
- `Reporting\` - reporting schema plus nightly rollup procedures fed from StoreOps and CustomerHub
- `Deploy\` - SQLCMD entry points for first-time deployment and smoke validation; set `DeployRoot` when running `00-deploy-all.sql` from a different working directory
- `..\shared\Migration\` - cross-database orchestration scripts that call the legacy procedures in the right order

## Deployment posture

This sample assumes a DBA runs SQLCMD-mode scripts from SSMS or `sqlcmd.exe`, database-by-database, with explicit review points between schema, seed, and migration steps. That is the sort of awkward but teachable deployment story the modernization pass is supposed to expose.
