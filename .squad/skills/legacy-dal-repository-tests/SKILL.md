---
name: "legacy-dal-repository-tests"
description: "Validate legacy repository seams by exercising real gateway stubs and DataSet mapping in NUnit."
domain: "testing"
confidence: "high"
source: "earned"
---

## Context

Use this when the repo already has a fake legacy gateway (`LegacyDbGateway`) that returns `DataSet` stubs and you need believable DAL coverage without mocking frameworks.

## Patterns

- Put DAL tests in `Tests.Unit\Repositories\{Area}\...Fixture.cs` so repository mapping stays separate from service tests.
- Instantiate the real repository with the real gateway stub; let stored-procedure constants and table names fail naturally if they drift.
- Cover one happy-path mapping assertion per repository method, then add the empty-row/null edge where the gateway intentionally returns no data.
- Add direct project references for any assemblies that appear in repository method signatures; old-style .NET Framework test projects do not compile reliably through transitive references.

## Example Shape

- `Repositories\StoreOps\DispatchTicketRepositoryFixture.cs`
- `Repositories\CustomerHub\PartnerAccountRepositoryFixture.cs`
- `Repositories\Reporting\WorkforceReportRepositoryFixture.cs`

## Anti-Patterns

- Testing repository logic only through higher-level services when the issue is specifically about DAL seams.
- Replacing the gateway with mocks that bypass stored-procedure names, `DataSet` table names, or row-shape assumptions.
- Forgetting the empty result path for stubs like store `999`, which should stay observable for regression checks.
