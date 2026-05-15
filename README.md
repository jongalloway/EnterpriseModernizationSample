# Enterprise Modernization Sample

This repository is framed to show the system **before** and **after** migration, starting from a deliberately layered 2005-era .NET estate.

## Selected baseline scenario

**Fabrikam Enterprise Pizza** — a regional pizza franchise platform with store ordering, call-center sales, corporate catering, delivery dispatch, and third-party POS integrations.

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

Directory guide files live at `docs\README.md`, `docs\foundation\README.md`, `src\README.md`, and `data\sqlserver\README.md` so downstream work can extend the structure without guessing where material belongs.

See `docs\foundation\enterprise-scenario.md` for the selected scenario brief, 2005-era domain details, and repository framing that set this direction.
See `docs\before\solution-architecture.md` for the current legacy topology, project boundaries, and modernization roadmap.
The canonical legacy solution entry point is `src\before\Fabrikam.EnterprisePizza.Legacy.sln`; the repository does not maintain a sibling `.slnx` file.
For the legacy DBA deployment story, start at `data\sqlserver\before\deployment-guide.md`.
