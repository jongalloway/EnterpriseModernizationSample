---
name: "legacy-host-smoke-baseline"
description: "Lock legacy gateway, host, and batch-job outputs with sparse NUnit smoke coverage before DI refactors land."
domain: "testing"
confidence: "high"
source: "earned"
---

## Context

Use this when a legacy modernization issue is about wiring or composition, but the implementation has not landed yet. Tester-owned progress should protect the observable seams that a refactor must preserve rather than speculating on the future container API.

## Patterns

- Extend `src\before\Fabrikam.EnterprisePizza.Legacy.Tests` with **thin, believable smoke tests** instead of broad end-to-end coverage.
- Cover **consumer-facing seams** first: the gateway alias lookup, any batch job that consumes it, and the WCF/ASMX hosts that expose business data.
- Strengthen existing legacy smoke tests with explicit counts and representative values; avoid `Count > 0` when the sample already has stable seeded rows.
- Add **direct project references** for any host or contract assemblies that appear in method signatures used by the tests.
- In worktrees, restore/build the old-style NUnit project with `dotnet msbuild <project> /t:Restore /p:RestorePackagesConfig=true` and point `nunit.framework` at `%UserProfile%\.nuget\packages` if no repo-local `packages\` folder exists.
- Because the repo does not include an NUnit adapter, finish with a **Windows PowerShell runtime smoke pass** against the built host assemblies.

## Examples

- `src\before\Fabrikam.EnterprisePizza.Legacy.Tests\SmokeTests\LegacyDbGatewayFixture.cs`
- `src\before\Fabrikam.EnterprisePizza.Legacy.Tests\SmokeTests\NightlyPosImportJobFixture.cs`
- `src\before\Fabrikam.EnterprisePizza.Legacy.Tests\SmokeTests\StoreDispatchServiceFixture.cs`
- `src\before\Fabrikam.EnterprisePizza.Legacy.Tests\SmokeTests\PartnerSyncServiceFixture.cs`

## Anti-Patterns

- Writing tests that assert a specific DI container API before the implementation exists.
- Expanding the legacy smoke project into broad integration coverage.
- Relying on a repo-local `src\before\packages\` folder when the worktree restores NUnit into the user NuGet cache.
- Declaring the seam protected after a compile-only pass without running a host-level runtime smoke check.
