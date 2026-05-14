---
created_at: 2026-05-14T15:35:48.472+02:00
agent: Hudson
---

# Integration stub seams stay in Legacy.Tests

## Context

Issue #43 asked for believable integration test stubs across the legacy estate without turning QA work into primary feature delivery.

## Decision

Keep the integration coverage in `src\before\Fabrikam.EnterprisePizza.Legacy.Tests` and model it as:

1. passing seam checks for workflows that already compose in code (`DispatchHost` ↔ `Business.StoreOps`, `PartnerSync` ↔ `Business.CustomerHub`, `PosSync` ↔ `Data`), and
2. `Explicit` scenario stubs with `Assert.Inconclusive` when the remaining gap sits in WinForms, Web Forms code-behind, or console entry points that do not expose a testable seam yet.

## Why

- It gives the repo believable cross-system coverage today without inventing fake harnesses.
- It documents the next failure-prone workflows in executable form.
- It avoids dragging UI rewrites into a testing issue.
