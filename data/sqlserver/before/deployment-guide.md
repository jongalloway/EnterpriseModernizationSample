# Legacy database deployment guide

**Document date:** 2026-05-14T15:35:48.472+02:00

This runbook covers the pre-migration SQL Server estate for **Fabrikam Enterprise Pizza**. It is written for the sort of DBA-owned deployment process a mid-2000s enterprise team would actually carry around: SQLCMD scripts, manual checkpoints, and a nightly bridge job that keeps the seams honest.

## Databases

- `FabrikamPizza_StoreOps` - store orders, dispatch tickets, delivery-zone bulletins, workforce alerts, driver, route, and POS staging data
- `FabrikamPizza_CustomerHub` - corporate accounts, partner masters, and partner extract staging
- `FabrikamPizza_Reporting` - nightly-fed delivery, labor, and partner rollups

## Prerequisites

1. SQL Server access with rights to create databases and stored procedures.
2. SSMS SQLCMD mode enabled, or `sqlcmd.exe` available on the deployment host.
3. A deployment operator account that can execute cross-database procedures after the databases are created.
4. Agreement that reporting remains batch-fed; nobody points live operational code at the reporting tables.
5. A local `data\sqlserver\before\Deploy\00-set-environment.cmd` copied from `00-set-environment.sample.cmd` and filled in for the target SQL Server.

## Deployment assets

- `data\sqlserver\before\Deploy\00-deploy-all.sql` - SQLCMD include script for first-time database creation, schema, seeds, and procedures
- `data\sqlserver\before\Deploy\00-invoke-sqlcmd.cmd` - shared wrapper helper that loads the environment file, keeps auth handling in one place, and passes the SQLCMD variables without relying on duplicate preamble blocks
- `data\sqlserver\before\Deploy\01-deploy-all.cmd` - command-line wrapper that runs the full deployment through `sqlcmd.exe`
- `data\sqlserver\before\Deploy\02-run-nightly-sync.cmd` - command-line wrapper for the CustomerHub → StoreOps → Reporting bridge job
- `data\sqlserver\before\Deploy\03-smoke-test.cmd` - command-line wrapper for the post-deployment smoke checks
- `data\sqlserver\before\Deploy\04-deployment-audit.cmd` - command-line wrapper for the deployment-history and bridge-data audit query
- `data\sqlserver\shared\Migration\03-deployment-audit.sql` - post-run audit query for deployment history, partner cache, and reporting batch state

## Environment file

Copy `data\sqlserver\before\Deploy\00-set-environment.sample.cmd` to `00-set-environment.cmd` and fill in:

- `LEGACY_SQL_SERVER` - SQL Server instance name or `server\instance`
- `LEGACY_SQL_AUTH_MODE` - `integrated` or `sql`
- `LEGACY_SQL_USER` / `LEGACY_SQL_PASSWORD` - only when SQL authentication is required; the wrappers pass the user name to `sqlcmd` and keep the password in `SQLCMDPASSWORD` so it does not land on the command line
- `StoreOpsDatabase`, `CustomerHubDatabase`, `ReportingDatabase` - database names if the DBA uses a non-default naming convention

## First-time deployment order

1. Run `data\sqlserver\before\Deploy\01-deploy-all.cmd`.
2. Review each database's `dbo.DatabaseDeploymentHistory` rows and confirm the script stack landed in CustomerHub, StoreOps, and Reporting.
3. Run `data\sqlserver\before\Deploy\02-run-nightly-sync.cmd` to populate the StoreOps partner cache and the reporting summaries.
4. Run `data\sqlserver\before\Deploy\03-smoke-test.cmd`.
5. Run `data\sqlserver\before\Deploy\04-deployment-audit.cmd` and save the output as the deployment record.

## SSMS SQLCMD mode alternative

If the DBA insists on SSMS instead of the command wrappers:

1. Open `data\sqlserver\before\Deploy\00-deploy-all.sql`.
2. Turn on **Query > SQLCMD Mode**.
3. Uncomment or add the `:setvar` sample lines at the top of the script and point `DeployRoot` at the `Deploy` folder if you are not running the script from there.
4. Execute the deploy script, then run `data\sqlserver\shared\Migration\01-run-nightly-sync.sql`, `02-smoke-test.sql`, and `03-deployment-audit.sql` in that order.

## Command-line examples

```cmd
cd /d data\sqlserver\before\Deploy
copy 00-set-environment.sample.cmd 00-set-environment.cmd
notepad 00-set-environment.cmd

01-deploy-all.cmd
02-run-nightly-sync.cmd
03-smoke-test.cmd
04-deployment-audit.cmd
```

## What the scripts do

- Each database gets an idempotent create script, a schema script, seed data, service procedures, and migration procedures.
- Service procedures line up with the legacy application seams: dispatch board reads from StoreOps, preferred partner lists read from CustomerHub, and dashboard summaries read from Reporting.
- Migration procedures deliberately keep cross-database plumbing visible. CustomerHub builds a partner extract, StoreOps refreshes its local partner cache from that extract, and Reporting rebuilds daily snapshots from both operational databases.
- The command wrappers are thin on purpose: a shared helper loads the environment file, passes SQLCMD variables once, picks integrated or SQL authentication, and then gets out of the way.
- Wrapper-provided `-v` values are now the authoritative variable source; the `.sql` entry points only keep commented sample `:setvar` lines for manual SSMS runs so there is no confusion about which value wins.
- CustomerHub keeps a rolling partner extract history instead of letting the staging table grow forever, and StoreOps treats the current batch as the full active-partner picture so inactive or removed partners fall back out of cache on the next sync.

## Operational cadence

- **Daytime:** WCF and ASMX-era consumers read from StoreOps and CustomerHub procedures.
- **Nightly:** DBA or SQL Agent orchestration runs the shared migration script.
- **Morning checks:** Operators run the smoke-test and deployment-audit scripts, then spot-check `ReportingBatchRun`, `PartnerAccountCache`, and `DatabaseDeploymentHistory`.

## SQL Agent posture

This sample assumes the first deployment is manual and the nightly bridge becomes a SQL Agent job later. If someone wants to wire the batch into SQL Agent, point the job step at `02-run-nightly-sync.cmd` or call `data\sqlserver\shared\Migration\01-run-nightly-sync.sql` directly from a SQLCMD-mode step. Either way, the ugly batch seam stays visible.

## Rollback posture

There is no magical one-click rollback here. If a deployment step misfires, restore from backups or rerun the affected idempotent script after correcting the input. That ugliness is part of the sample's modernization story.
