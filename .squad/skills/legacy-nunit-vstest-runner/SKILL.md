---
name: "legacy-nunit-vstest-runner"
description: "Run old-style .NET Framework NUnit projects with NuGet restore plus dotnet vstest"
domain: "testing"
confidence: "high"
source: "earned"
---

## Context

Use this when a non-SDK .NET Framework test project already references `nunit.framework`, but `dotnet test` only reports a successful build and never executes the fixtures.

## Patterns

- Restore the legacy solution with `nuget restore` so `packages.config` dependencies land in `src\before\packages`.
- Add `NUnit3TestAdapter` as a development dependency to the unit-test project's `packages.config`.
- Build the test project with `dotnet msbuild ... /t:Build /p:Configuration=Debug`.
- Run the compiled assembly with `dotnet vstest` and an explicit `--TestAdapterPath` that prefers `src\before\packages\NUnit3TestAdapter.<version>\build\net462` and falls back to `%USERPROFILE%\.nuget\packages\...`.
- Keep the runner in a reusable PowerShell script so CI and local repros share the same execution path.

## Examples

- `scripts\Invoke-LegacyNUnit.ps1`
- `.github\workflows\legacy-nunit-tests.yml`
- `src\before\Fabrikam.EnterprisePizza.Tests.Unit\packages.config`

## Anti-Patterns

- Trusting `dotnet test` output alone on old-style NUnit projects.
- Baking adapter paths directly into multiple workflow steps instead of one script.
- Pulling legacy host-smoke tests into the default CI lane before their service-locator bootstrap is explicit.
