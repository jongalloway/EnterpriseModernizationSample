# Decision: salvage intentional work from merged checkout on a fresh main branch

**Date:** 2026-05-15T03:56:17.791+02:00  
**By:** Ripley  
**Issue:** #66

## Context

The active checkout was still on `squad/49-complete-architecture-documentation` even though PR #57 was already merged into `main`. The worktree contained a mix of intentional service-seam changes, stale branch drift against `main`, and large volumes of generated `bin/` and `obj/` output.

## Decision

Do not merge the dirty checkout as-is. Create a fresh branch from `main`, salvage only the intentional legacy seam changes there, and leave regressions from the stale branch behind.

## What was preserved

- WCF dispatch host now resolves `IDispatchCoordinator` through a lightweight legacy service locator and exposes a `DispatchBoardSnapshot` contract.
- ASMX partner sync now resolves `IPartnerAccountService` the same way and exposes a partner snapshot envelope.
- `Fabrikam.EnterprisePizza.Data` now carries explicit StoreOps and CustomerHub repository seams on top of the existing reporting gateway.
- `Fabrikam.EnterprisePizza.Tests.Unit` captures the new connection catalog, gateway, dispatch, and partner seam behavior.

## What was intentionally excluded

- Reversions of portal projects, workforce reporting code, SQL deployment assets, and other work already merged into `main`.
- Generated `bin/`, `obj/`, and other build output from the stale checkout.

## Consequences

- Recovery work stays reviewable and mergeable because it starts from current `main` instead of trying to reconcile a stale branch in place.
- Full-solution builds still depend on pre-existing external legacy packages (`System.Web.Mvc`, `AjaxControlToolkit`) that are not restored in this environment; validate the rescued seam through targeted project builds instead.
