---
name: "stacked-pr-restack"
description: "Restack a child PR onto main after its base PR merges, then revalidate before clearing draft."
domain: "git, pull-requests, testing"
confidence: "high"
source: "earned"
---

## Context

Use this when a PR was intentionally stacked on another squad branch and the parent PR has since merged into `main`.

## Patterns

- Fetch `origin` first and confirm the child branch is the only commit left beyond `origin/main`.
- Rebase the child branch onto `origin/main`, then `git push --force-with-lease` so the remote PR keeps the same head branch with a clean merge base.
- Retarget the PR to `main` only after the rebase succeeds.
- Re-run the exact validation the stacked PR depended on before clearing draft status; for legacy NUnit branches here, that means restore the solution, build `Fabrikam.EnterprisePizza.Tests.Unit`, and execute `scripts\Invoke-LegacyNUnit.ps1`.
- Request Copilot review with `gh pr edit --add-reviewer "@copilot"`; if reviewer assignment is unavailable, fall back to the repo's accepted `@copilot review` PR comment.

## Anti-Patterns

- Changing the PR base without rebasing the branch first.
- Force-pushing without `--force-with-lease`.
- Leaving generated `bin\` or `obj\` output in the worktree after validation.
- Clearing draft before the rebased branch has a fresh validation pass against `main`.
