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

## 2026-05-14: Ripley Workitem Setup Complete

Ralph can now monitor 52 concrete GitHub issues across 9 phases, routed to squad members. Phase 1 gates Phases 2-9; parallel execution enabled once Phase 1 reaches ~50% completion.

