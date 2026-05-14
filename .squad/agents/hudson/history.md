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
