---
name: "legacy-sqlcmd-deployment-packaging"
description: "Package a legacy multi-database SQL Server estate as numbered per-database scripts with SQLCMD orchestration and explicit nightly bridge jobs."
domain: "services"
confidence: "high"
source: "earned"
---

## Context

Use this when a legacy .NET sample needs database deployment assets that feel like a real DBA-owned process instead of a polished modern migration pipeline. It fits especially well when the system owns multiple SQL Server databases and the seams between them are supposed to stay visible.

## Patterns

- Give each database its own numbered script stack: create database, schema, seed data, service procedures, then migration procedures.
- Keep deployment orchestration in SQLCMD include scripts so operators can see the order of operations instead of hiding it behind a generated tool.
- Put cross-database movement behind stored procedures and a shared orchestration script; do not pretend the legacy estate had event-driven elegance.
- Add a simple deployment-history table per database so reruns and smoke checks have something concrete to inspect.
- Keep reporting batch-fed by rebuild procedures, not live operational joins from application code.

## Examples

- `data\sqlserver\before\Deploy\00-deploy-all.sql`
- `data\sqlserver\shared\Migration\01-run-nightly-sync.sql`
- `data\sqlserver\before\StoreOps\05-migration-procedures.sql`
- `data\sqlserver\before\Reporting\05-migration-procedures.sql`

## Anti-Patterns

- One giant setup script that hides database ownership and run order.
- Letting reporting read operational tables directly at runtime because it is easier for the demo.
- Using shared migration helpers to own schema; ownership should stay with the individual database folders.
- Replacing the ugly bridge job with a modern abstraction that erases the modernization story.
