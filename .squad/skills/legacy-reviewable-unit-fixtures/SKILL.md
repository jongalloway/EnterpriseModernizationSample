---
name: "legacy-reviewable-unit-fixtures"
description: "Make legacy NUnit fixtures reviewer-proof by asserting transformed inputs at the seam and capturing call arguments inside inline stubs."
domain: "testing"
confidence: "high"
source: "earned"
---

## Context
Use this when adding or tightening .NET Framework NUnit fixtures around legacy services that normalize inputs or call repository seams with fixed arguments. It fits this repo because many tests use private in-file stubs instead of a mocking framework.

## Patterns
- Feed normalization tests an input that actually changes after trimming or uppercasing.
- Assert the normalized value at the dependency seam, not just on the final projection.
- Capture fixed repository arguments such as `weeksBack` on the stub and assert them directly.
- Remove stub configuration properties that are never read by the stub implementation.

## Examples
- `src\before\Fabrikam.EnterprisePizza.Tests.Unit\Services\StoreOperationsWorkbenchServiceFixture.cs`
- `src\before\Fabrikam.EnterprisePizza.Tests.Unit\Services\WorkforceReportingServiceFixture.cs`
- PR #69 reviewer follow-up for normalization coverage and overtime-trend argument verification.

## Anti-Patterns
- Reusing already-normalized inputs in tests that claim to verify normalization.
- Leaving dead stub properties in place just because a test once set them.
- Only asserting output counts when the real risk is an incorrect dependency call.
