---
name: "architecture-baseline-roadmap"
description: "Write a legacy architecture baseline that is useful for implementation and later modernization work."
domain: "architecture"
confidence: "high"
source: "earned"
---

## Context

Use this when a repo already has some scaffolding, but the team still needs one authoritative document that explains what exists now, what boundaries must hold, and how the modernization should sequence. This is especially useful in legacy-modernization samples where the before-state is intentionally sprawling.

## Patterns

- Start with the **current posture**: state plainly whether the repo is scaffold, partial implementation, or mature legacy code.
- Separate **business domains** from **project list**. A believable architecture doc explains both.
- Document **runtime topology** and **solution inventory** independently; one shows traffic flow, the other shows code ownership.
- Add **boundary rules** that reviewers can enforce, not just descriptive prose.
- Make the roadmap a **sequence with gates**, not a pile of future ideas.
- Define the **after-state seam** in business terms so modernization does not become project-for-project cloning.

## Recommended sections

1. Architectural posture
2. System topology at a glance
3. Repository and solution structure
4. Boundary rules
5. Known modernization pain points
6. Roadmap with gates
7. Recommended after-state seams
8. Reviewer guidance

## Anti-Patterns

- Writing only a project catalog with no business ownership model
- Writing only a target architecture and pretending the before-state does not matter
- Letting roadmap items ignore hard dependency gates such as database ownership or contract seams
- Mirroring every legacy project in the after-state plan without questioning whether the project boundary was accidental
