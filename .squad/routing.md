# Work Routing

How to decide who handles what.

## Routing Table

| Work Type | Route To | Examples |
|-----------|----------|----------|
| Solution shape, architecture, reviewer gating | Ripley | Split projects, define modernization seams, review cross-cutting changes |
| Windows Forms and desktop workflows | Bishop | Legacy client shell, dialogs, grid-heavy forms, config-driven desktop behavior |
| Web Forms, MVC, and jQuery-era UI | Hicks | Master pages, Web Forms pages, MVC controllers/views, old JavaScript behavior |
| WCF, ASMX, SOA, and data access plumbing | Vasquez | Service contracts, XML-heavy integrations, Enterprise Library-style layers |
| Testing and legacy parity checks | Hudson | Smoke tests, scenario coverage, regression notes, reviewer feedback |
| Good-fit autonomous coding tasks | @copilot | Small isolated fixes, docs updates, scaffolding, dependency bumps |
| Code review | Ripley | Review PRs, check quality, enforce consistency across projects |
| Testing | Hudson | Write tests, find edge cases, verify fixes |
| Scope & priorities | Ripley | What to build next, trade-offs, decisions |
| Session logging | Scribe | Automatic — never needs routing |
| Backlog monitoring | Ralph | Board status, issue pickup, PR follow-through |

## Issue Routing

| Label | Action | Who |
|-------|--------|-----|
| `squad` | Triage: analyze issue, assign `squad:{member}` label | Lead |
| `squad:ripley` | Pick up architecture, planning, or review-heavy work | Ripley |
| `squad:bishop` | Pick up Windows Forms or desktop workflow work | Bishop |
| `squad:hicks` | Pick up Web Forms, MVC, or front-end web work | Hicks |
| `squad:vasquez` | Pick up WCF, ASMX, or service/data integration work | Vasquez |
| `squad:hudson` | Pick up tests, validation, and bug reproduction work | Hudson |
| `squad:copilot` | Pick up autonomous coding-agent work that fits the capability profile | @copilot |

### How Issue Assignment Works

1. When a GitHub issue gets the `squad` label, the **Lead** triages it — analyzing content, assigning the right `squad:{member}` label, and commenting with triage notes.
2. When a `squad:{member}` label is applied, that member picks up the issue in their next session.
3. When `squad:copilot` is applied, the coding agent picks up the issue automatically when auto-assign is enabled in `team.md`.
4. Members can reassign by removing their label and adding another member's label.
5. The `squad` label is the "inbox" — untriaged issues waiting for Lead review.

## Rules

1. **Eager by default** — spawn all agents who could usefully start work, including anticipatory downstream work.
2. **Scribe always runs** after substantial work, always as `mode: "background"`. Never blocks.
3. **Quick facts → coordinator answers directly.** Don't spawn an agent for "what port does the server run on?"
4. **When two agents could handle it**, pick the one whose domain is the primary concern.
5. **"Team, ..." → fan-out.** Spawn all relevant agents in parallel as `mode: "background"`.
6. **Anticipate downstream work.** If a feature is being built, spawn the tester to write test cases from requirements simultaneously.
7. **Issue-labeled work** — when a `squad:{member}` label is applied to an issue, route to that member. The Lead handles all `squad` (base label) triage.
8. **Legacy sample realism beats tidiness.** Preserve the intentionally uneven architecture unless the task is explicitly modernization-focused.
