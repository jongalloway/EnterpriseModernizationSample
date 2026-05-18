# Project Context

- **Owner:** Scribe
- **Project:** EnterpriseModernizationSample
- **Stack:** C#, .NET Framework, Windows Forms, ASP.NET Web Forms, ASP.NET MVC, WCF, ASMX, Enterprise Library-style patterns, classic jQuery, SQL Server-style data access
- **Description:** Sample legacy line-of-business application built to feel like a real enterprise system started around 2005 and expanded over time.
- **Created:** 2026-05-13

## Core Context

Hicks owns the legacy web surface across Web Forms, MVC, and old JavaScript behavior.

## Recent Updates

📌 Team initialized on 2026-05-13
📌 Decision consolidation on 2026-05-13T18:54:45Z: Scribe merged 5 inbox decisions into canonical `decisions.md`. Your "Hicks web direction" decision locked: 2005 corporate portal tone, blue gradients, MySpace footer, promo panels, newsletter signup, "best viewed" hints.
  - **Cross-team:** Ripley finalized Fabrikam Enterprise Pizza with 2005-era throwbacks and data split to before/after/shared. Vasquez defined three-database model for legacy SQL Server estate.

## Learnings

- The sample should include visibly different web eras living side by side.
- Older jQuery usage and server-heavy page models are part of the desired realism.
- Fabrikam Enterprise Pizza works best as the outward-facing web brand, with the 2005-era experience leaning into polished portal chrome and dated social/web cues.
- **2026-05-13:** Investigated legacy Web Forms control library candidates. **Recommended: AJAX Control Toolkit (ACT) v20.1.0**—MIT open source, officially archived Oct 2024, peak usage 2008–2015. Perfect for signaling organic technical debt and modernization narrative. Candidates evaluated: DayPilot Lite, ComponentArt (commercial/discontinued), Coolite (OSS base archived). ACT won on realism, licensing, and authenticity to the 2010s Web Forms era.
- **2026-05-14T03:23:27.208+02:00:** A believable StoreOps intranet shell lands better as its own Web Forms portal project with a master page, blue-gradient supervisor chrome, and small AJAX-era touches layered directly over StoreOps business services.
- **2026-05-14T03:23:27.208+02:00:** The main Web Forms portal works best as a glossy partner shell that links the more specialized legacy surfaces (storefront, franchise bulletin center, and corporate account tools) instead of replacing them. A master page, dashboard copy, and a little optional jQuery-era chrome make the surface feel older than the MVC storefront without turning it into parody.
- **2026-05-14T09:58:57.628+02:00:** For `src\before\Fabrikam.EnterprisePizza.Portal`, prefer self-contained browser behavior over dead package references: `Scripts\portal.js` now uses vanilla DOM APIs, `Site.master.designer.cs` stays aligned with the generated Web Forms `ContentPlaceHolder` type, and `Default.aspx` binds through encoding helpers so output stays safe by default.
- **2026-05-18T01:39:47.894-07:00:** Era-authentic portal chrome reads best when the Web Forms master page owns the newsletter box, promo badge rail, marquee ticker, and footer nostalgia so every page inherits the same 2005-2008 marketing shell without duplicating one-off chrome in content pages.

📌 Decision archive round-up on 2026-05-14T01:02:28Z (Scribe):
- ACT v20.1.0 recommendation locked into canonical decisions.md
- **Impact:** Web Forms phase now has explicit control stack validated for 2008–2015 authenticity. Enables clear modernization narrative (ACT → modern JS). Licensing confirmed MIT (no restrictions).
- **Cross-team:** Ripley and Vasquez notified of repo naming clarity and updated workitem status assessment.

## 2026-05-14: Ripley Workitem Setup Complete

Your Phase 4 issues (Web Forms, ACT, Identity) are routed with 'squad:hicks' label. Depends on Phase 1 and Phase 3. Can parallelize once Phase 1 reaches ~50% completion.
## 2026-05-14 Team Session

✓ **Hicks** (Web Dev): Added Fabrikam.EnterprisePizza.Portal and opened draft PR #54.
✓ **Bishop** (Desktop Dev): Completed WinForms dispatch desktop shell with working grid, dialog, and settings-backed behavior.
✓ **Hudson** (Tester): Added .NET Framework 4.8 NUnit 3.12 unit test project and seam-level fixtures.

→ Scribe: Merged bishop-dispatch-shell-settings decision into canonical decisions.md. Cross-team dependencies remain on track per Phase 1 coordination.

## 2026-05-14 PR Validation Cycle

**Hicks PR Work (2026-05-14T07:58:57.628Z):**
- **PR #54 (Portal):** Removed dead jQuery dependency, rewrote portal.js in vanilla JS, hardened Default.aspx bindings, reordered Portal project in .sln. Full portal project build validated.
- **PR #56 (StoreOps):** Validated namespace suggestion from Copilot, confirmed build success with build-backed reply, restored and built StoreOps portal project.
- **Decision record:** Added safe binding patterns for portal composition
- **Skill artifact:** Binding hardening guidance added to team knowledge base

**Cross-team updates:**
- Inbox merge completed: Copilot Review Workflow (Ripley), Enterprise Library DI seam (Vasquez) merged into canonical decisions.md
- Orchestration log: Hicks session captured at 2026-05-14T07:58:57.628Z
- All PR validation work on track; no blockers to Phase 2–4 parallel execution
