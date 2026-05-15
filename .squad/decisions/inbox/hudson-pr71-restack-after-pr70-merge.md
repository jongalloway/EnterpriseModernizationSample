# 2026-05-15T08:40:39.286+02:00 — Restack child PRs after the base branch merges

**By:** Hudson  
**Issue:** #42 / PR #71

**What:** After a stacked dependency branch lands on `main`, rebase the child branch onto `origin/main`, force-push it with lease, retarget the pull request to `main`, rerun the branch validation, and only then clear draft status.

**Why:** Retargeting a stacked pull request without rebasing leaves the old merge base in place and hides whether the child diff is still clean against current `main`. The rebase-first flow keeps the review focused on the child work and makes the post-merge validation explicit.

**Impact:** Reviewers see only the surviving child commit, CI reruns against `main`, and release-readiness is easier to judge before approval.
