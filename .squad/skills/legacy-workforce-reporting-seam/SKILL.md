---
name: "legacy-workforce-reporting-seam"
description: "Keep payroll as an external feed by staging workforce snapshots in StoreOps and loading reporting summaries through nightly SQL procedures and a DataSet-backed repository seam."
domain: "services"
confidence: "high"
source: "earned"
---

## Context

Use this when the sample needs believable labor analytics without turning a store operations app into a fake HR suite. It fits legacy .NET stacks where SQLCMD deployment scripts, batch ETL, and repository wrappers should expose brittle seams instead of hiding them.

## Patterns

- Keep payroll/benefits outside the app, but land imported labor snapshots in `StoreOps` tables with names that sound like nightly feed artifacts.
- Load `Reporting` tables from those StoreOps snapshots via numbered migration procedures so deployment order and ETL behavior stay obvious.
- Expose reporting reads through stored procedure names and a `DataSet`-returning gateway, then map rows inside a repository so the batch layer does not hard-code report values.
- Validate the seam from both directions: SQL smoke scripts should execute the new report procedures, and the legacy batch console should print the same report family in human-readable form.

## Examples

- `data\sqlserver\before\StoreOps\02-schema.sql`
- `data\sqlserver\before\Reporting\04-service-procedures.sql`
- `src\before\Fabrikam.EnterprisePizza.Data\Repositories\Reporting\WorkforceReportRepository.cs`
- `src\before\Fabrikam.EnterprisePizza.Reporting.Batch\Program.cs`

## Anti-Patterns

- Inventing a polished all-in-one HR domain that erases the existing external payroll boundary.
- Computing labor analytics directly in the console app with inline fake rows.
- Adding report tables without wiring them into nightly rebuild procedures and smoke validation.
