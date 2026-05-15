# 2026-05-15T08:40:39.286+02:00 — Issue #42 stacks on the NUnit foundation branch

**By:** Hudson  
**Issue:** #42

**What:** Implement DAL repository coverage for issue #42 on top of `squad/40-set-up-nunit-test-framework` rather than plain `main`, then open the PR against that foundation branch until the runner work lands.

**Why:** The verified NUnit execution path for `Fabrikam.EnterprisePizza.Tests.Unit` currently lives on the issue #40 branch (`scripts\Invoke-LegacyNUnit.ps1` plus `.github\workflows\legacy-nunit-tests.yml`). Stacking the DAL tests there keeps the diff limited to repository fixtures instead of re-bundling the runner setup into issue #42.

**Impact:** Reviewers can validate issue #42 with executable NUnit results immediately, but the branch must be retargeted or rebased once the issue #40 foundation merges.
