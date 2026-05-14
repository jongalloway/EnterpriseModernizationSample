---
name: "architecture-documentation-layering"
description: "Separate architecture documentation into three layers for different audiences: technical blueprint, stakeholder narrative, and developer patterns."
domain: "documentation"
confidence: "high"
source: "earned"
---

## Context

When documenting a complex system migration or modernization, a single monolithic architecture document tries to serve three audiences simultaneously:

1. **Implementers** — need tactical patterns, code examples, and decision points.
2. **Stakeholders/leads** — need timeline, trade-offs, risk mitigations, and success criteria.
3. **Reviewers** — need boundary rules and enforcement points.

A monolithic document context-switches between these constantly and satisfies none well. Separate documents yield clarity.

## Patterns

### Layer 1: Technical Blueprint (`modernized-architecture.md`)

**Audience:** Implementers, infrastructure teams, code reviewers.

**Content:**
- System topology at a glance (diagram or prose).
- Domain decomposition table (what service owns what).
- Repository and solution structure.
- Boundary rules (what is forbidden, what is required).
- Technology stack with rationale.
- Data migration strategy (schema evolution, dual-write patterns).
- Deployment architecture.

**Example sections:**
- "Runtime shape" — shows how services call each other.
- "Boundary rules" — "Database ownership stays explicit" or "No cross-domain direct DB queries."
- "Known trade-offs and risks" — tables mapping issue → modern approach → trade-off.

**Tone:** Technical, precise, enforcement-ready.

### Layer 2: Migration Narrative (`migration-narrative.md`)

**Audience:** Stakeholders, team leads, business decision-makers, anyone asking "why this way?"

**Content:**
- The legacy story: how it got this way, what it does well, what causes pain.
- Why each seam shifts and what stays the same.
- Phased cutover strategy with gates (dual-write for verification).
- Business timeline and value delivery.
- Mapping legacy concepts to modern equivalents.
- How to measure progress (early wins, midpoint, endgame).
- Risks and mitigations (business perspective).
- Decision points that define success (do not rewrite business logic, preserve seams, retire old tech, measure by velocity not LOC).

**Example sections:**
- "The legacy story" — paragraphs explaining how the system evolved, what decisions were good (database seams), what causes sprawl (tech choices).
- "Phase 1: Identity boundary" — explains why OAuth 2.0 matters and what stays the same.
- "Business timeline" — table mapping period → focus → business value → technical milestones.

**Tone:** Storytelling, human-friendly, justifies trade-offs.

### Layer 3: Implementation Patterns (`implementation-patterns.md`)

**Audience:** Developers writing code in both legacy and modernized systems.

**Content:**
- Before-state patterns (how to extend legacy code when necessary, with discouragement against sprawl).
  - Example: Enterprise Library data access, WCF SOAP contracts, Web Forms user controls, NUnit tests.
  - Anti-pattern: "Do not add WCF endpoints to the modernized solution."
- After-state patterns (how to write modern code with examples).
  - Example: ASP.NET Core services with DI, EF Core DbContext, REST API conventions, xUnit with TestContainers.
  - Anti-pattern: "Do not mock the database; use TestContainers for integration tests."
- Shared principles (boundary enforcement, explicit dependencies, error handling, testing discipline, observability).
- Decision points table (question → legacy answer → modern answer → enforcement point).

**Example sections:**
- Code snippets in both states (e.g., "store lookup" via Enterprise Library gateway vs. EF Core LINQ).
- Comparison tables: "Question: How do I query data? → Legacy: Stored procedure → Modern: LINQ → Enforcement: Code review"
- "Shared principles across before and after" — values that survive the migration.

**Tone:** Practical, code-heavy, decision-focused.

## Organization

```
docs/
  foundation/          # Scenario, naming, repo framing
  before/              # Legacy topology, maps, notes
    solution-architecture.md  (Layer 1 for before-state)
    solution-map.md
  after/               # Modern topology, patterns, narrative
    modernized-architecture.md  (Layer 1 for after-state)
    migration-narrative.md      (Layer 2)
    implementation-patterns.md  (Layer 3)
    README.md          (index all three)
```

## Anti-Patterns

- **One 50-page "Architecture" document** — tries to be technical, narrative, and practical all at once; satisfies none.
- **Narrative with no patterns** — stakeholders know the timeline but developers ask "but how do I write this code?"
- **Patterns with no narrative** — developers know the code conventions but cannot explain trade-offs to management.
- **Blending before and after in one document** — creates 200 pages of prose; hard to find "how do I modernize this specific service?"

## Success Criteria

- [ ] Implementers cite patterns in code review ("this violates pattern #5 from implementation-patterns.md").
- [ ] Stakeholders use the narrative timeline to plan headcount and milestones.
- [ ] Reviewers enforce boundary rules from the blueprint without needing verbal explanation.
- [ ] New team members can onboard by reading Layer 3 (patterns) first, then Layer 2 (narrative) for context, then Layer 1 (blueprint) for implementation details.

## Related Skills

- `architecture-baseline-roadmap` — captures the blueprint layer specifically.
- `repo-vs-scenario-naming` — naming conventions appear in all three layers.
