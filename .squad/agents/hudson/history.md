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
- 2026-05-14T03:23:27.208+02:00 — Old-style net48 NUnit projects in this repo need direct project references to exposed contract assemblies like `Shared.Contracts`; relying on transitive references from `Business.StoreOps` will not keep smoke tests compiling once service return types cross that seam.
- 2026-05-14T15:35:48.472+02:00 — Service endpoint smoke coverage in `src\before\Fabrikam.EnterprisePizza.Legacy.Tests` is more believable when it checks the web-host files (`StoreDispatchService.svc`, `PartnerSync.asmx`) and `web.config` transport settings alongside the stubbed service payloads.
- 2026-05-14T15:35:48.472+02:00 — Legacy NUnit smoke tests can safely locate `src\before` at runtime by walking up from `AppDomain.CurrentDomain.BaseDirectory` until `Fabrikam.EnterprisePizza.Legacy.sln` is found, which keeps file-based assertions stable across runner locations.
- 2026-05-14T15:35:48.472+02:00 — In this repo, `packages.config` NUnit restores land in the user-level NuGet cache during CLI builds, so old-style test projects need a fallback `HintPath` to `$(USERPROFILE)\.nuget\packages\nunit\3.12.0\lib\net45\nunit.framework.dll` if the legacy `src\before\packages` folder is absent.

## 2026-05-14: Ripley Workitem Setup Complete

Your Phase 1 and Phase 7 issues (Foundation NUnit baseline, comprehensive testing) are routed with 'squad:hudson' label. Phase 7 depends on all subsystems (Phases 1-6).
## 2026-05-14 Team Session

✓ **Hicks** (Web Dev): Added Fabrikam.EnterprisePizza.Portal and opened draft PR #54.
✓ **Bishop** (Desktop Dev): Completed WinForms dispatch desktop shell with working grid, dialog, and settings-backed behavior.
✓ **Hudson** (Tester): Added .NET Framework 4.8 NUnit 3.12 unit test project and seam-level fixtures.

→ Scribe: Merged bishop-dispatch-shell-settings decision into canonical decisions.md. Cross-team dependencies remain on track per Phase 1 coordination.
