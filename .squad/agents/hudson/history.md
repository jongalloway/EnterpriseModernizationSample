# Project Context

- **Owner:** Scribe
- **Project:** EnterpriseModernizationSample
- **Stack:** C#, .NET Framework, Windows Forms, ASP.NET Web Forms, ASP.NET MVC, WCF, ASMX, Enterprise Library-style patterns, classic jQuery, SQL Server-style data access
- **Description:** Sample legacy line-of-business application built to feel like a real enterprise system started around 2005 and expanded over time.
- **Created:** 2026-05-13

## Core Context

Hudson owns regression thinking, repro detail, and reviewer enforcement for the modernization sample.

## Recent Updates

📌 Team initialized on 2026-05-13

## Learnings

- The sample needs believable fragile edges, not just old technology names.
- Review and test coverage should preserve legacy realism while keeping the repo usable.
- 2026-05-14T15:35:48.472+02:00 — Phase 7.6 is worth unblocking with smoke baselines in `src\before\Fabrikam.EnterprisePizza.Legacy.Tests` that lock `LegacyDbGateway`, `NightlyPosImportJob`, `StoreDispatchService`, and `PartnerSyncService` outputs before any Unity/service-locator refactor lands.
- 2026-05-14T15:35:48.472+02:00 — In issue worktrees, the legacy NUnit project builds reliably when `nunit.framework` points at `%UserProfile%\.nuget\packages\nunit\3.12.0\lib\net45\nunit.framework.dll` and the project is restored with `dotnet msbuild ... /t:Restore /p:RestorePackagesConfig=true`.
