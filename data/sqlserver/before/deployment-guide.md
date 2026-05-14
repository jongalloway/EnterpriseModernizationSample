# Legacy database deployment guide

**Document date:** 2026-05-14T15:35:48.472+02:00

This runbook covers the pre-migration SQL Server estate for **Fabrikam Enterprise Pizza**. It is written for the sort of DBA-owned deployment process a mid-2000s enterprise team would actually carry around: SQLCMD scripts, manual checkpoints, and a nightly bridge job that keeps the seams honest.

## Databases

- `FabrikamPizza_StoreOps` - store dispatch, driver, route, and POS staging data
- `FabrikamPizza_CustomerHub` - corporate accounts, partner masters, and partner extract staging
- `FabrikamPizza_Reporting` - nightly-fed delivery, labor, and partner rollups

## Prerequisites

1. SQL Server access with rights to create databases and stored procedures.
2. SSMS SQLCMD mode enabled, or `sqlcmd.exe` available on the deployment host.
3. A deployment operator account that can execute cross-database procedures after the databases are created.
4. Agreement that reporting remains batch-fed; nobody points live operational code at the reporting tables.

## First-time deployment order

1. Run `data\sqlserver\before\Deploy\00-deploy-all.sql`.
2. Run `data\sqlserver\shared\Migration\01-run-nightly-sync.sql` to populate the StoreOps partner cache and the reporting summaries.
3. Run `data\sqlserver\shared\Migration\02-smoke-test.sql`.
4. Capture row counts from `dbo.DatabaseDeploymentHistory` in each database as the deployment record.

## What the scripts do

- Each database gets an idempotent create script, a schema script, seed data, service procedures, and migration procedures.
- Service procedures line up with the legacy application seams: dispatch board reads from StoreOps, preferred partner lists read from CustomerHub, and dashboard summaries read from Reporting.
- Migration procedures deliberately keep cross-database plumbing visible. CustomerHub builds a partner extract, StoreOps refreshes its local partner cache from that extract, and Reporting rebuilds daily snapshots from both operational databases.

## Operational cadence

- **Daytime:** WCF and ASMX-era consumers read from StoreOps and CustomerHub procedures.
- **Nightly:** DBA or SQL Agent orchestration runs the shared migration script.
- **Morning checks:** Operators run the smoke-test script and spot-check `ReportingBatchRun`, `PartnerAccountCache`, and `DatabaseDeploymentHistory`.

## Rollback posture

There is no magical one-click rollback here. If a deployment step misfires, restore from backups or rerun the affected idempotent script after correcting the input. That ugliness is part of the sample's modernization story.
