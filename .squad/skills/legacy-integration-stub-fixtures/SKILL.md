---
name: "legacy-integration-stub-fixtures"
description: "Model legacy cross-system workflows with passing seam checks plus explicit pending scenarios"
domain: "testing"
confidence: "high"
source: "earned"
---

## Context

Use this when a legacy .NET Framework solution needs believable integration coverage, but the workflow still terminates in brittle UI code-behind, SOAP hosts, or console entry points that do not justify a full harness in the current issue.

## Patterns

- Keep the work inside the existing legacy NUnit project instead of spinning up a brand-new test stack.
- Add direct `ProjectReference` entries for every assembly whose public types appear in the stub fixture.
- Prefer passing seam checks for flows that already compose in code, such as service-to-business or job-to-gateway wiring.
- Mark broader end-to-end placeholders as `Explicit` and end them with `Assert.Inconclusive(...)` that names the blocker and the exact file or seam still preventing full execution.

## Examples

- `src\before\Fabrikam.EnterprisePizza.Legacy.Tests\IntegrationStubs\CrossSystemWorkflowStubFixture.cs`
- `src\before\Fabrikam.EnterprisePizza.Legacy.Tests\IntegrationStubs\ScenarioStub.cs`

## Anti-Patterns

- Writing failing placeholder tests into the default run path.
- Hiding blockers in vague TODO comments instead of naming the missing seam.
- Turning a test-stub issue into a UI refactor just to make an end-to-end case runnable.
