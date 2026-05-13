# Enterprise Modernization Sample

This repository is framed to show the system **before** and **after** migration, starting from a deliberately layered 2005-era .NET estate.

## Selected baseline scenario

**Enterprize Pizza Franchise Platform** — a regional pizza chain with franchise stores, call-center ordering, corporate catering, store dispatch, and third-party POS integrations.

Why this works:
- It is recognizable, a little funny, and still believable as a legacy enterprise system.
- It naturally supports WinForms, Web Forms, ASP.NET MVC, WCF, ASMX, and multiple SQL Server databases without feeling forced.
- It gives the modernization story clear seams: store ops, customer ordering, franchise portals, integrations, and reporting.

## Repository framing

- `docs\before\` — legacy behavior, screenshots, architecture notes, and operational context
- `docs\after\` — target architecture, migration notes, and modernization decisions
- `docs\foundation\` — scenario, naming, and repository-shaping documents
- `src\before\` — legacy .NET Framework solution and projects
- `src\after\` — migrated services, web apps, workers, and shared components
- `data\sqlserver\` — SQL Server scripts, schema notes, and sample database assets

See `docs\foundation\enterprise-scenario.md` for the scenario options and recommendation that set this direction.
