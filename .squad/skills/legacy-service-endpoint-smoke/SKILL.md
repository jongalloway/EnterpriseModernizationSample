---
name: "legacy-service-endpoint-smoke"
description: "Smoke-test WCF and ASMX surfaces without standing up IIS"
domain: "testing"
confidence: "high"
source: "earned"
---

## Context
Use this when a legacy .NET Framework solution has web-hosted WCF or ASMX endpoints that need believable regression coverage, but the repo does not have a cheap integration harness for IIS-hosted execution.

## Patterns
- Instantiate the service class directly to prove the sample payload still comes back through the legacy code path.
- Reflect over service contracts and web-service attributes so transport-facing metadata stays visible in tests.
- Read the `.svc` or `.asmx` host directive plus the service `web.config` to catch wiring drift such as missing metadata endpoints, binding names, or enabled HTTP protocols.
- In old-style NUnit projects, add direct project references for every exposed contract assembly used by the service signatures.

## Examples
- `src\before\Fabrikam.EnterprisePizza.Legacy.Tests\SmokeTests\StoreDispatchServiceFixture.cs`
- `src\before\Fabrikam.EnterprisePizza.Legacy.Tests\SmokeTests\PartnerSyncServiceFixture.cs`

## Anti-Patterns
- Only testing the underlying business class and calling that "service coverage"
- Depending on transitive project references for WCF contract DTOs in old-style test projects
- Requiring a full IIS-hosted integration setup for smoke tests that are really about endpoint shape and basic sample behavior
