# Project Context

- **Owner:** Scribe
- **Project:** EnterpriseModernizationSample
- **Stack:** C#, .NET Framework, Windows Forms, ASP.NET Web Forms, ASP.NET MVC, WCF, ASMX, Enterprise Library-style patterns, classic jQuery, SQL Server-style data access
- **Description:** Sample legacy line-of-business application built to feel like a real enterprise system started around 2005 and expanded over time.
- **Created:** 2026-05-13

## Core Context

Ripley owns solution structure, reviewer gating, and the realism of the legacy architecture.

## Recent Updates

📌 Team initialized on 2026-05-13
📌 Decision consolidation on 2026-05-13T18:54:45Z: Scribe merged 5 inbox decisions (2 user directives + 3 agent decisions) into canonical `decisions.md`. Orchestration logs created for all agents. Team decisions now locked.
  - **Your decision:** "Fabrikam Enterprise Pizza direction" — finalized name, 2005-era throwbacks, data split to `data\sqlserver\before|after|shared`
  - **Cross-team:** Hicks locked web direction to 2005 corporate portal (blue gradients, MySpace footer, promo panels). Vasquez confirmed three-database model: StoreOps, CustomerHub, Reporting.
📌 Scenario expansion on 2026-05-13T20:57:42Z: Your "Scenario expansion priorities" decision added delivery/dispatch to StoreOps, B2B/partnerships to CustomerHub, and cross-system analytics to Reporting. HR treated as workforce management, not HRIS.
  - **Cross-team:** Vasquez aligned "Scenario placement" decision with explicit database seams for delivery, workforce, and partner flows.

## Learnings

- The sample should feel like a business app that kept accumulating projects and integration seams over time.
- Cross-project consistency matters, but visible historical layering is intentional.
- Enterprize Pizza Franchise Platform is the selected baseline scenario because it supports believable legacy sprawl across desktop, web, service, and SQL Server boundaries.
- Repository framing now reserves `docs\before`, `docs\after`, `docs\foundation`, `src\before`, `src\after`, and `data\sqlserver` for legacy versus modernized work.
- Key files for this direction are `README.md`, `docs\foundation\enterprise-scenario.md`, and `.squad\decisions\inbox\ripley-enterprize-pizza-scenario.md`.
- The scenario direction has now tightened to **Fabrikam Enterprise Pizza**, which keeps the Microsoft-style naming but reads more credibly across solution, portal, and database boundaries than the joke spelling.
- The legacy feel should be carried by explicit 2005-era business details like MySpace footer links, printable coupons, faxed catering workflows, nightly POS sync, and Excel/PDF-heavy franchise operations.
- Database assets should keep the top-level `data\sqlserver` location while splitting internally into `before`, `after`, and `shared` to match the modernization framing.
- The cleanest next scenario expansion is not more random modules; it is a tight seam of dispatch/routing in StoreOps, B2B partnerships in CustomerHub, and delivery/labor/partner scorecards in Reporting, with HR kept operations-focused instead of becoming a full HR suite.
- The Unity core seam needs one explicit bootstrap contract: `CreateConfiguredContainer()` owns registrations, while `InitializeServiceLocator(IUnityContainer)` only exposes the locator for an already-configured container and should fail fast otherwise. Key files: `src\before\Fabrikam.EnterprisePizza.Core\Composition\CoreContainer.cs`, `.squad\skills\legacy-unity-core-seam\SKILL.md`, and `.squad\decisions\inbox\ripley-core-container-bootstrap-path.md`.
- CustomerHub partnership logic reads commission rates, tier thresholds, credit terms, and volume-pricing breaks from one business-rules class, so Web Forms, services, and later DAL work can share the same B2B calculations without duplicating them.

📌 Backlog generated on 2026-05-13T21:07:52Z:
- Created 52 concrete workitems across 9 phases targeting 14 projects, 3 databases, and full legacy enterprise sprawl.
- Phase 1 (Foundation & Topology, 11 items) is critical path; must complete first to unblock Phases 2–9.
- Phases 2–3 (Data & Business Logic, 14 items) can run in parallel; feed Phases 4–7.
- Phases 4–7 (Web, Services, Tests, Desktop, 21 items) parallelize by subsystem once Phase 1–3 complete.
- Phases 8–9 (Reporting & Documentation, 6 items) finalize.
- Each workitem targets 1–2 days of work; scoped for implementation, not brainstorming.
- Tech stack confirmed: Enterprise Library 6.0, Autofac 4.9.2, NUnit 3.12, ASP.NET Identity 2.2.3, WCF, ASMX, Web Forms + AJAX Control Toolkit, Windows Forms, SSRS, legacy batch scheduling.
- Dependency graph enables parallel execution; no team member blocked waiting for another.
- Decision logged to `.squad\decisions\inbox\ripley-workitems.md` with cross-team coordination notes for Hicks (web branding), Vasquez (database ETL), and Scribe (foundation docs).

📌 Scenario expansion locked on 2026-05-13T18:57:42.795Z:
  - **Decision:** Scenario expansion priorities — delivery dispatch/routing, workforce ops, B2B partnerships, delivery/labor/partner analytics
  - **StoreOps:** Route planning, dispatch boards, driver zones, mileage/reimbursement
  - **CustomerHub:** Partner account setup, contract pricing, referral channels, shared order/catering
  - **Reporting:** On-time delivery, route efficiency, labor overtime/turnover, partner profitability
  - **Cross-team:** Vasquez confirmed placement across databases with intentional legacy seams

📌 Workitem consolidation on 2026-05-13T21:09:53Z:
- Produced 52 concrete, prioritized workitems across 9 phases (52 items total, 1–2 days each, 14 projects)
- Phase 1 (Foundation & Topology, 11 items) is critical path blocker; must complete first
- Phases 2–3 (Data & Business Logic, 14 items) can parallelize once Phase 1 ~50% complete
- Phases 4–7 (Web, Services, Tests, Desktop, 21 items) parallelize by subsystem once Phases 1–3 complete
- Phases 8–9 (Reporting & Documentation, 6 items) finalize
- Tech stack locked: Enterprise Library 6.0, Autofac 4.9.2, NUnit 3.12, ASP.NET Identity 2.2.3, WCF, ASMX, Web Forms + AJAX
- Dependency graph enables parallel execution; no team member blocked waiting for another
- Orchestration log created at `.squad\orchestration-log\2026-05-13T21-09-53Z-ripley-workitem-planning.md`
- Scribe confirms Phase 1 dry-run is critical before full assignment
- Backlog ready for team assignment; Vasquez Phase 2 (data) unblocks Phases 3, 5; Hicks Phase 4 (web) unblocks downstream UI work
- Repo naming should stay descriptive at the container level: `EnterpriseModernizationSample` fits a repository that holds before/after docs, source, and data, while **Fabrikam Enterprise Pizza** should remain the scenario, solution, and domain-facing name inside the sample.
- The clean naming seam is "repo as modernization container, scenario as business identity" because it preserves the broader sample framing without throwing away the stronger domain branding already locked in across decisions.
- Key files for this naming guidance are `README.md`, `.squad\decisions.md`, and `.squad\decisions\inbox\ripley-repo-naming-guidance.md`.

📌 Public GitHub repository created on 2026-05-13T21:34:06.816+02:00:
- Created public repo: https://github.com/jongalloway/EnterpriseModernizationSample
- Origin remote configured and pushing to main branch
- Local workitems and squad decisions pushed successfully
- Repository is now open for team collaboration and public reference

📌 Workitem completion audit on 2026-05-14T03:02:28.105+02:00:
- **Verdict:** NOT ALL COMPLETED (Phase 1 topology in place; Phases 2–9 not yet started)
- **Phase 1 Status:** ~40% complete
  - ✅ Solution file with all 14 projects defined
  - ✅ Project directories with .csproj scaffolding
  - ✅ Repository structure (src/before, docs/foundation, data/sqlserver with before/after/shared split)
  - ✅ Foundation documentation (scenario, solution-map, package matrix)
  - ❌ No DI container setup (Enterprise Library, Autofac, Castle Windsor)
  - ❌ No legacy patterns wired (ASMX, WCF, SOAP endpoints, DataSets, ad-hoc SQL)
- **Phases 2–9 Status:** 0% complete
  - ❌ No database schema or SQL scripts (Phase 2 blocker)
  - ❌ No business logic implementation (Phases 2–3)
  - ❌ No web UI projects filled out (Phase 4)
  - ❌ No service/integration code (Phase 5)
  - ❌ No testing infrastructure (Phases 6–7)
  - ❌ No reporting or finalization (Phases 8–9)
- **Supporting evidence:**
  - 35 C# files across solution (mostly placeholder/token code like OrderSummary.cs)
  - Database folders exist with only README files; no SQL scripts
  - No GitHub issues created for workitem tracking
  - Git history shows only planning/decision commits, no implementation commits
  - Tech stack (Enterprise Library, Autofac, NUnit, ASP.NET Identity 2.2.3, WCF, ASMX) not yet applied
- **Gap quantification:** Estimated 5–8% of 52-item backlog complete by deliverable count
- **Largest unfinished areas:** Database schema (Phase 2 critical path), DI composition (Phase 1), business logic (Phases 2–3), web UI (Phase 4), services (Phase 5), testing (Phase 6–7), reporting (Phases 8–9)
- **Recommendation:** Begin Phase 1 DI container + service composition work immediately; Phase 2 database schema work is critical path blocker for Phases 3–5

📌 Decision archive round-up on 2026-05-14T01:02:28Z:
- Scribe merged 7 inbox decisions into canonical decisions.md; deduped 4 already-merged entries
- **New decisions locked:**
  - ASP.NET Web Forms Control Vendor Recommendation (Hicks): AJAX Control Toolkit v20.1.0 (MIT, archived Oct 2024)
  - Repository Naming Guidance (Ripley): Keep repo as `EnterpriseModernizationSample`, scenario as `Fabrikam Enterprise Pizza`
  - Non-Microsoft Web Forms Control Dependency: Reinforces organic vendor mix common to legacy systems
- **Cross-team impact:** Web Forms phase now has explicit control stack (ACT for AJAX, validated for 2008–2015 authenticity). Repo naming clears ambiguity for documentation and branding.

📌 GitHub workitem setup on 2026-05-14T03:12:16.029+02:00:
- **Created all 52 workitems as GitHub issues:**
  - Phase 1 (11 items): Foundation & Topology — solution architecture, project scaffolding, DI/library setup
  - Phases 2–3 (14 items): Data & Business Logic — database schema, seeding, DAL, business logic
  - Phases 4–7 (21 items): Web/Services/Tests/Desktop — Web Forms portal, WCF/ASMX services, Windows Forms, NUnit tests
  - Phases 8–9 (6 items): Reporting & Documentation — SSRS/batch reporting, final documentation
- **Squad routing labels created:** squad, squad:ripley, squad:bishop, squad:hicks, squad:vasquez, squad:hudson
- **Each issue labeled with:** squad (inbox marker) + squad:{member} (owner)
- **Ralph now has full visibility:** All 52 items ready to monitor, triage, and track
- **Repository state:** Executable backlog live; no dependencies blocking Phase 1 start
- A durable architecture baseline for this repo needs three layers in one place: current solution topology, boundary rules that prevent cross-domain drift, and a modernization roadmap that cuts along business seams instead of mirroring legacy project names.

## 2026-05-14: Ripley Workitem Setup Complete

Ralph can now monitor 52 concrete GitHub issues across 9 phases, routed to squad members. Phase 1 gates Phases 2-9; parallel execution enabled once Phase 1 reaches ~50% completion.
## 2026-05-14: Squad orchestration session

- Ripley: Issue #1 solution architecture complete. Created docs\before\solution-architecture.md, recorded modernization-seams decision. Baseline topology established for all downstream work.
- Vasquez: Issue #3 data project foundation complete. Fabrikam.EnterprisePizza.Data project built with connection catalog, stored-procedure gateway, repository plumbing. Recorded data-project-seam decision. Ready for service layer integration.

**Team impact:**
- Both decisions merged into canonical decisions.md
- Orchestration logs written: .squad/orchestration-log/
- Session summary logged: .squad/log/
- Next gate: Monitor Phase 1 ~50% completion before gating Phase 2+

📌 Ripley Issue #2 completion on 2026-05-14T01:23:27Z:
- **Task:** Issue #2 core project — Fabrikam.EnterprisePizza.Core as real legacy core seam
- **Outcome:** Completed with draft PR #53
- **Decision recorded:** "Modernization seams over project mirroring" — after-state decomposes along business boundaries (StoreOps, CustomerHub, commerce edge, integration/batch, Reporting) not one-to-one project replacements
- **Cross-team:** Ripley baseline topology ready; Vasquez data/DAL work can integrate against documented seams. Phase 1 ~50% complete unlocks Phases 2+ gating release.
- **Orchestration log:** .squad/orchestration-log/2026-05-14T01-23-27Z-ripley.md

📌 Copilot Review Workflow Setup on 2026-05-14T09:58:57.628+02:00:
- **Task:** Assign all open PRs (#53, #54, #55, #56) to Copilot for review and establish review workflow
- **Supported Mechanism:** `gh pr edit --add-reviewer "@copilot"` (official GitHub CLI feature)
- **Outcome:** ✅ Copilot now active reviewer on all 4 PRs; each submitted initial review comments within minutes
- **What Works:** Copilot reviews code, generates comments, identifies issues, provides file-by-file analysis
- **What Doesn't Automate:** Comment dismissal, PR approval, review resolution, CI triggering — all remain manual process
- **Decision recorded:** "Copilot review workflow setup" — documents mechanism, current state, and manual next steps for handling comments
- **Next Step:** Monitor PRs for Copilot comments; address comments manually; squad members retain approval authority
- **Team Impact:** Copilot now provides real-time advisory review on all open PRs; squad members remain merge gate
- The canonical legacy solution entry point is now `src\before\Fabrikam.EnterprisePizza.Legacy.sln`; `.slnx` should not be added or maintained in this repo.
- Relevant enforcement points for this cleanup are `README.md`, `src\before\README.md`, `docs\before\solution-map.md`, and `.squad\skills\legacy-nunit-project-skeleton\SKILL.md`.

📌 Issue #49 completion on 2026-05-14T15:35:48.472+02:00:
- **Task:** Complete solution architecture documentation — finalize before/after migration narrative and implementation patterns
- **Outcome:** Completed with draft PR #57
- **Deliverables:**
  - `docs\after\modernized-architecture.md` (13.9 KB) — target-state system topology, domain decomposition, boundary rules, data migration, tech stack, phased roadmap with gates
  - `docs\after\migration-narrative.md` (16.6 KB) — before/after story: why each seam shifts, phased cutover with dual-write patterns, risk mitigations, success checkpoints
  - `docs\after\implementation-patterns.md` (17.2 KB) — repeatable code patterns for legacy (stored procedures, WCF, Web Forms, NUnit) and modern (EF Core, REST, async/await, xUnit) with decision points
  - Updated `docs\after\README.md` to index all three documents
- **Key decision:** Three separate documents instead of one monolithic file — modernized-architecture for blueprint, migration-narrative for justification/timeline, implementation-patterns for practical guidance.
- **Architecture principles locked:**
  1. Preserve three-database split (StoreOps, CustomerHub, Reporting) — this is a **good** legacy decision, not a smell.
  2. Decompose by business seams, not project mirroring — modern services follow domain boundaries.
  3. Phased cutover with dual-write validation — services coexist during transition; reconciliation queries ensure data consistency.
  4. Explicit API boundaries (REST/gRPC) — never add cross-domain direct DB queries in after-state.
  5. Implementation patterns guide when to use: stored procedures (legacy only), EF Core (modern only), REST (modern only), never new WCF.
- **Roadmap phased with gates:**
  - Phase A (weeks 1–2): Foundation + OAuth 2.0 identity boundary
  - Phase B (weeks 2–4): Storefront (MVC → Core MVC) + modern identity
  - Phase C (weeks 3–6): StoreOps service (business logic + EF Core → modern REST API)
  - Phase D (weeks 5–8): CustomerHub service (Web Forms + ASMX → Core service + REST API)
  - Phase E (weeks 6–10): Reporting pipeline (console app + batch → Worker Service in Kubernetes)
  - Phase F (weeks 9+): Cutover + legacy retirement
- **Cross-team impact:** All three services (Storefront, StoreOps, CustomerHub) have clear before→after mappings. Reporting roadmap guides Vasquez's ETL work. Implementation patterns guide all code review.
- **Decision recorded:** `.squad/decisions/inbox/ripley-complete-architecture-documentation.md` — documents context, decision, rationale, consequences, and success criteria.
- **Repo files:** Created in `docs/after/` with README updated; indexed from main `docs/README.md` (existing).
- **Lesson:** Separating technical blueprint from stakeholder narrative from developer patterns yields clarity on three different audiences. Monolithic architecture docs try to be all three and fail at each.
- The clean seam for early legacy DI is to let `Fabrikam.EnterprisePizza.Core` own Unity registrations plus the CommonServiceLocator bridge, while host and service projects consume that seam later instead of inventing their own containers.
- Directory-shaping work lands more cleanly when each top-level seam has its own guide README, so later teams can extend docs, docs\\foundation, src, and data\\sqlserver without inventing new layout rules.
- On 2026-05-15T03:18:24.845+02:00, PR #55 proved the repository-structure seam is best enforced with short README files at `docs\README.md`, `docs\foundation\README.md`, `src\README.md`, and `data\sqlserver\README.md`, while the root `README.md` stays the entry point that links those guides together.
- The current repo still has no dedicated root-level docs validation harness, so docs-only PR validation is limited to git diff hygiene, path/link sanity, and PR mergeability checks rather than an automated docs build.
- **2026-05-15T03:56:17.791+02:00 merged-checkout salvage:** When a branch is already merged but the checkout still contains meaningful edits, treat the checkout as evidence, not the merge vehicle. Rebuild the good work on a fresh branch from `main` and leave stale-branch regressions behind.
- **Targeted rescue seam:** The preserved legacy seam spans `src\before\Fabrikam.EnterprisePizza.Services.DispatchHost`, `src\before\Fabrikam.EnterprisePizza.Services.PartnerSync`, `src\before\Fabrikam.EnterprisePizza.Data`, `src\before\Fabrikam.EnterprisePizza.Shared.Contracts`, and `src\before\Fabrikam.EnterprisePizza.Tests.Unit`.
- **Validation boundary:** In this repo, targeted `dotnet msbuild` builds for rescued legacy projects can succeed even when the full legacy solution still fails on pre-existing external package gaps in `Fabrikam.EnterprisePizza.Web.Storefront` (`System.Web.Mvc`) and `Fabrikam.EnterprisePizza.StoreOps.Portal` (`AjaxControlToolkit`).
- **2026-05-15T04:37:01.332+02:00 repo cleanup triage:** The remaining dirty checkout split cleanly into three buckets: files already identical to `main`, partial stale-branch drift that should be dropped, and a small durable docs/skill set worth preserving.
- **Preserve the architecture baseline, not the stale branch:** `docs\before\solution-architecture.md` and its index references were still valuable, but the orphaned `DeliveryDashboardRepository.cs` seam was only partial local work and should stay in backup storage rather than ride along as duplicate drift.
- **Cleanup safety net:** For merged-branch cleanup in this repo, take a full stash snapshot first, reset the checkout to `main`, then selectively reapply only the durable artifacts you can justify in current topology.
- **2026-05-15T04:58:30.796+02:00 governance-slice recovery:** When the remaining stash value is policy and documentation, recover it on a fresh branch from `main`, not by reopening the stale implementation branch.
- **Coherence rule:** Governance recovery should bundle reviewer policy (`.copilot\skills\git-workflow\SKILL.md`, `.squad\team.md`, `.squad\routing.md`, `.squad\agents\ripley\charter.md`) with the doc pointers that tell contributors where the legacy baseline actually lives (`README.md`, `docs\before\README.md`, `docs\before\solution-map.md`, `docs\before\solution-architecture.md`, `src\before\README.md`).
- **Stash retention rule:** Keep the backup stash until the governance/docs recovery PR is merged and the local checkout is back on `main`; only then is it defensible to consider dropping the stash.
- The root `README.md` now needs to lead with what **Fabrikam Enterprise Pizza** is, then describe the included business domains and legacy project inventory instead of explaining scenario-selection history or repository-creation process.
- For this repo, the clearest entry-point diagram is a Mermaid topology in `README.md` that shows user-facing apps, WCF/ASMX service seams, business/shared libraries, the three SQL Server databases, `Integrations.PosSync`, and `Reporting.Batch`.
- Key files for this documentation seam are `README.md`, `docs\before\solution-architecture.md`, and `src\before\README.md`.
