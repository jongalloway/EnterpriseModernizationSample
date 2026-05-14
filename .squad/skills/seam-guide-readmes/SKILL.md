---
name: "seam-guide-readmes"
description: "Anchor top-level before/after/foundation folders with short README files so later work extends the intended seam instead of guessing."
domain: "repository-structure"
confidence: "medium"
source: "earned"
---

## Context

Use this pattern when a repo is deliberately split into top-level seams such as `before`, `after`, `foundation`, or shared data folders. It is most useful early in a project, when the directory structure exists but downstream contributors still need a crisp rule for where new material belongs.

## Patterns

- Put a short `README.md` at the root of each major seam (`docs`, `docs\foundation`, `src`, `data\sqlserver`, and similar roots).
- Describe the ownership boundary of the folder, not a full implementation plan.
- Use the README to explain why the seam exists and what kinds of artifacts belong there.
- Keep the text stable and architectural so it survives later implementation detail churn.

## Examples

- `docs\README.md` explains the split between legacy, target-state, and foundation documentation.
- `docs\foundation\README.md` states that scenario, naming, and package baselines live there.
- `src\README.md` makes the migration-state split explicit so `before` and `after` do not collapse into one code tree.

## Anti-Patterns

- Relying on one root README to explain every folder in a growing repo.
- Letting teams infer folder meaning from issue titles or tribal memory.
- Writing folder READMEs that duplicate volatile implementation details instead of documenting the seam.
