---
name: "legacy-dal-seam"
description: "Build a believable mid-2000s .NET data layer around connection catalogs, stored-procedure wrappers, DataSets, and repository facades."
domain: "services"
confidence: "high"
source: "earned"
---

## Context

Use this when a legacy .NET sample needs a data access layer that feels real enough for modernization tooling, but you do not want to hide the rough edges behind a modern ORM abstraction.

## Patterns

- Keep database boundaries explicit with a connection-name catalog or enum (`StoreOps`, `CustomerHub`, `Reporting`).
- Centralize stored procedure names so consumers do not scatter magic strings across business and service code.
- Put the ugly plumbing in a gateway that returns `DataSet`/`DataTable` shapes.
- Expose repository classes above that gateway so business services stop hard-coding records inline.
- Let the business layer depend on repositories, not on raw `DataSet` plumbing, even if the repository internals stay ugly.
- For batch integrations, let the job own scheduling and connection aliases, but pull its latest-batch/status read through a StoreOps repository seam instead of a naked gateway helper.

## Example Shape

- `Configuration\LegacyConnectionCatalog.cs`
- `StoredProcedures\LegacyStoredProcedures.cs`
- `Gateways\LegacyDbGateway.cs`
- `Repositories\StoreOps\...Repository.cs`
- `Repositories\CustomerHub\...Repository.cs`
- `Repositories\Reporting\...Repository.cs`
- `Integrations\...\Job.cs` consuming a repository-backed batch snapshot

## Anti-Patterns

- Leaving the DAL as a single string helper with no stored-procedure or repository seam.
- Hard-coding fake data in business services once a data project exists.
- Collapsing all database areas into one generic connection name when the scenario depends on split legacy estates.
- Replacing the legacy seam with a polished modern repository abstraction that erases the migration story.
