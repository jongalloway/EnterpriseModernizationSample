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
- 2026-05-14T15:35:48.472+02:00 — Cross-system legacy test stubs fit best in `src\before\Fabrikam.EnterprisePizza.Legacy.Tests\IntegrationStubs\`; keep one fixture for passing seam checks (`DispatchHost`, `PartnerSync`, `PosSync`) and use `Explicit` + `Assert.Inconclusive` only for broader workflows that still dead-end in UI code-behind or console entry points.
- 2026-05-14T15:35:48.472+02:00 — `dotnet msbuild src\before\Fabrikam.EnterprisePizza.Legacy.Tests\Fabrikam.EnterprisePizza.Legacy.Tests.csproj /t:Build /p:Configuration=Debug` works once `src\before\packages\NUnit.3.12.0\lib\net45\nunit.framework.dll` exists; `dotnet test` on this old-style NUnit project only confirms build shape and does not provide meaningful execution reporting without extra adapter wiring.
- 2026-05-14T03:23:27.208+02:00 — Old-style net48 NUnit projects in this repo need direct project references to exposed contract assemblies like `Shared.Contracts`; relying on transitive references from `Business.StoreOps` will not keep smoke tests compiling once service return types cross that seam.
- 2026-05-14T15:35:48.472+02:00 — Service endpoint smoke coverage in `src\before\Fabrikam.EnterprisePizza.Legacy.Tests` is more believable when it checks the web-host files (`StoreDispatchService.svc`, `PartnerSync.asmx`) and `web.config` transport settings alongside the stubbed service payloads.
- 2026-05-14T15:35:48.472+02:00 — Legacy NUnit smoke tests can safely locate `src\before` at runtime by walking up from `AppDomain.CurrentDomain.BaseDirectory` until `Fabrikam.EnterprisePizza.Legacy.sln` is found, which keeps file-based assertions stable across runner locations.
- 2026-05-14T15:35:48.472+02:00 — In this repo, `packages.config` NUnit restores land in the user-level NuGet cache during CLI builds, so old-style test projects need a fallback `HintPath` to `$(USERPROFILE)\.nuget\packages\nunit\3.12.0\lib\net45\nunit.framework.dll` if the legacy `src\before\packages` folder is absent.
- 2026-05-15T08:40:39.286+02:00 — Issue #40 is best unblocked by restoring `src\before\Fabrikam.EnterprisePizza.Legacy.sln`, then running old-style NUnit assemblies with `dotnet vstest` plus `NUnit3TestAdapter`; the reusable entry point now lives in `scripts\Invoke-LegacyNUnit.ps1`, and the CI workflow is `.github\workflows\legacy-nunit-tests.yml`.
- 2026-05-15T08:40:39.286+02:00 — `Fabrikam.EnterprisePizza.Tests.Unit` is a reliable foundation for follow-on issues #41 and #42 once `NUnit3TestAdapter` 4.5.0 is restored, but `Fabrikam.EnterprisePizza.Legacy.Tests` still fails under a real runner until the service-host paths initialize `LegacyServiceLocator` outside `Global.asax`.

## 2026-05-14: Ripley Workitem Setup Complete

Your Phase 1 and Phase 7 issues (Foundation NUnit baseline, comprehensive testing) are routed with 'squad:hudson' label. Phase 7 depends on all subsystems (Phases 1-6).
## 2026-05-14 Team Session

✓ **Hicks** (Web Dev): Added Fabrikam.EnterprisePizza.Portal and opened draft PR #54.
✓ **Bishop** (Desktop Dev): Completed WinForms dispatch desktop shell with working grid, dialog, and settings-backed behavior.
✓ **Hudson** (Tester): Added .NET Framework 4.8 NUnit 3.12 unit test project and seam-level fixtures.

→ Scribe: Merged bishop-dispatch-shell-settings decision into canonical decisions.md. Cross-team dependencies remain on track per Phase 1 coordination.
- 2026-05-14T15:35:48.472+02:00 — Phase 7.6 is worth unblocking with smoke baselines in `src\before\Fabrikam.EnterprisePizza.Legacy.Tests` that lock `LegacyDbGateway`, `NightlyPosImportJob`, `StoreDispatchService`, and `PartnerSyncService` outputs before any Unity/service-locator refactor lands.
- 2026-05-14T15:35:48.472+02:00 — In issue worktrees, the legacy NUnit project builds reliably when `nunit.framework` points at `%UserProfile%\.nuget\packages\nunit\3.12.0\lib\net45\nunit.framework.dll` and the project is restored with `dotnet msbuild ... /t:Restore /p:RestorePackagesConfig=true`.
