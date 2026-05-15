---
name: "merged-worktree-cleanup"
description: "Safe cleanup pattern for stale local worktrees after branches land in main"
domain: "version-control"
confidence: "high"
source: "extracted from Ripley cleanup session"
---

## Context

Use this when the repo has multiple local worktrees and you need to remove branches that have already landed in `main` without discarding meaningful local work.

## Patterns

### Trust `origin/main` First
Fetch with prune, compare `main...origin/main`, and only fast-forward local `main` if it has zero unique commits. Do not classify branches against a stale local `main`.

### Delete by Verified Merge State
After local `main` is current, delete a worktree branch only if `git merge-base --is-ancestor <branch> main` succeeds. Use the same rule for standalone local branches.

### Dirty Worktree Triage
If a merged worktree is dirty, inspect the status before deleting it. Force-remove only when the remaining dirt is disposable workspace noise such as `bin/`, `obj/`, or incidental local solution-file churn; otherwise keep the worktree and report the blocker.

### Recover Durable Docs Separately
If the remaining value is governance, policy, or documentation, recover that slice on a fresh branch from current `main` instead of reviving the stale implementation branch. Pair policy files with the guide/README updates that explain the same seam so the restored branch reads as one coherent change, not a grab bag.

## Anti-Patterns

- Deleting worktrees based on a stale local `main`
- Using `git branch -d` from an unrelated current branch as the only safety check
- Force-removing a dirty worktree before confirming the dirt is disposable
