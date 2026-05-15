---
name: "legacy-storeops-workbench-schema"
description: "Anchor store-ops workbench screens in separate StoreOps tables for order state, status snapshots, workforce alerts, and zone bulletins, with dispatch tickets linked back to orders."
domain: "services"
confidence: "high"
source: "earned"
---

## Context

Use this when the sample needs believable store-manager and dispatch workbench data without pretending the portal and desktop shell are backed by one tidy aggregate. It fits legacy SQL Server estates where operational status, order lookup, and workforce notes grew into side tables around the core dispatch board.

## Patterns

- Keep store metadata (`Store`) separate from operational snapshot data (`StoreOperationsStatus`) so manager-on-duty, board mode, and escalation notes can drift independently.
- Give order lookup its own `StoreOrder` table and let `DispatchTicket` point back to it through a nullable foreign key; delivery and carryout should overlap, not collapse into one fake workflow.
- Keep POS staging explicit with a batch header plus line-level import items so later ETL or reconciliation work has a seam to hook into.
- Store workforce watch items and route bulletins in first-class StoreOps tables instead of hardcoding them in portal code or burying them in Reporting.
- Seed the workbench tables with enough uneven real-world data to support follow-up, warning, and normal states across more than one store.

## Examples

- `data\sqlserver\before\StoreOps\02-schema.sql`
- `data\sqlserver\before\StoreOps\03-seed-data.sql`
- `src\before\Fabrikam.EnterprisePizza.Business.StoreOps\Services\StoreOperationsWorkbenchService.cs`
- `src\before\Fabrikam.EnterprisePizza.StoreOps.Portal\Default.aspx.cs`

## Anti-Patterns

- Expanding `Store` until it becomes a junk drawer for every live operational field.
- Treating dispatch tickets as if they are the only order record in the system.
- Leaving workforce alerts and zone bulletins trapped in UI-only hardcoded lists once the SQL seam exists.
