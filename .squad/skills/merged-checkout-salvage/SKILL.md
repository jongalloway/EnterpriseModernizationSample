---
name: "merged-checkout-salvage"
description: "Recover intentional work from a dirty checkout whose branch is already merged"
domain: "workflow"
confidence: "high"
source: "team-discovery"
---

## When to use

Use this when a local checkout is still dirty after its branch or PR has already merged, especially if the worktree mixes real source changes with stale branch drift and generated build output.

## Playbook

1. Treat `main` as authoritative and inspect the dirty checkout against `main`, not just against `HEAD`.
2. Separate intentional source/docs changes from generated `bin/`, `obj/`, and workspace noise.
3. If the diff includes regressions from stale branch drift, do **not** keep working on that branch.
4. Create a fresh branch from `main` and re-apply only the intentional changes there.
5. Validate the rescued seam with targeted builds/tests if the full legacy solution has unrelated dependency gaps.
6. Clean generated output before committing so the rescue PR only carries durable changes.

## Why it works

A merged stale checkout is no longer a trustworthy integration surface. Rebuilding the intentional work on top of current `main` prevents accidental reversion of code, docs, SQL assets, and team-state that landed after the original branch diverged.
