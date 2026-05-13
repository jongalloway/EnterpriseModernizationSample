# Squad Decisions

## Active Decisions

### 2026-05-13T20:38:19.745+02:00: User directive
**By:** Scribe (via Copilot)
**What:** Use Enterprise Library, Patterns & Practices guidance, Stock Trader, and the Blue Yonder CAB sample as reference points for the legacy .NET sample's architecture and feel.
**Why:** User request — captured for team memory

### 2026-05-13T20:42:26.144+02:00: User directive
**By:** Scribe (via Copilot)
**What:** Structure the repository so it can hold docs and source folders for both before and after migration, and model the system with one or more SQL Server databases.
**Why:** User request — captured for team memory

### 2026-05-13T20:42:26.144+02:00: Enterprize Pizza baseline scenario
**By:** Ripley
**Decision:** Use **Enterprize Pizza Franchise Platform** as the baseline legacy scenario for the sample.

**Rationale:**
- The domain is easy to understand and slightly playful without breaking credibility.
- It cleanly justifies a 10+ project .NET estate spanning WinForms, Web Forms, ASP.NET MVC, WCF, ASMX, and multiple SQL Server databases.
- It gives modernization seams that are obvious enough for docs, source layout, and later reviewer decisions.

**Repository impact:**
- Reserve `docs\before\` and `docs\after\` for legacy and target-state documentation.
- Reserve `src\before\` and `src\after\` for pre- and post-migration implementations.
- Keep SQL Server assets under `data\sqlserver\`, assuming multiple databases instead of a single monolith.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
