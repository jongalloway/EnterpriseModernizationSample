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

### 2026-05-13T20:51:37.753+02:00: User directive
**By:** Scribe (via Copilot)
**What:** Use a Microsoft-style business name in the scenario, with Fabrikam Enterprise Pizza as a strong candidate, and carry 2005-era throwbacks throughout the design and business domain, including dated trends such as a MySpace footer link.
**Why:** User request — captured for team memory

### 2026-05-13T20:54:45.785+02:00: User directive
**By:** Scribe (via Copilot)
**What:** The app should feel organically grown over time, include some older NuGet packages that were popular in the early 2010s, and include business logic projects in the solution.
**Why:** User request — captured for team memory

### 2026-05-13T20:52:10.614+02:00: Hicks web direction
**By:** Hicks
**What:** Push the public-facing web surfaces toward a clear 2005 corporate-web identity under **Fabrikam Enterprise Pizza**, using slightly over-polished portal chrome, upbeat enterprise marketing copy, and era-authentic social/web references.
**Why:** This gives Web Forms, MVC, and portal content a shared visual story without making the sample feel parody-heavy or historically incoherent.

**Implementation notes:**
- Treat **Fabrikam Enterprise Pizza** as the polished marketing-facing name, while older internal wording can still drift between portal areas.
- Favor blue gradients, beveled boxes, badge promos, utility nav, and "trusted partner" language over modern minimalism.
- Use dated-but-believable web culture touches such as a MySpace footer link, newsletter signup copy, "best viewed" hints, and limited-time promo badges.

### 2026-05-13T20:52:10.614+02:00: Fabrikam Enterprise Pizza direction
**By:** Ripley
**What:** Use **Fabrikam Enterprise Pizza** as the baseline business name, carry explicit 2005-era throwbacks throughout the domain and presentation, and frame SQL Server assets under `data\sqlserver\before`, `data\sqlserver\after`, and `data\sqlserver\shared`.
**Why:** The Microsoft-style name matches the user's direction without weakening credibility, the throwbacks make the legacy sample legible at a glance, and the database split gives the before/after migration story a clear seam instead of one catch-all data folder.

### 2026-05-13T20:52:10.614+02:00: Fabrikam pizza data layout
**By:** Vasquez
**What:** Use **Fabrikam Enterprise Pizza** as the business name and model the legacy SQL Server estate as three databases: `FabrikamPizza_StoreOps`, `FabrikamPizza_CustomerHub`, and `FabrikamPizza_Reporting`.
**Why:** That split feels authentically mid-2000s: core order/store writes stay isolated, customer/franchise data grows into its own integration-heavy database, and reporting is peeled off for nightly ETL so operations do not trust live transactional queries.

### 2026-05-13T20:57:42.795+02:00: Scenario expansion priorities
**By:** Ripley
**What:** Expand the Fabrikam Enterprise Pizza sample first with delivery dispatch/routing in **StoreOps**, B2B partnerships and corporate account workflows in **CustomerHub**, and matching delivery/labor/partner analytics in **Reporting**. Treat HR as store-operations workforce management (scheduling, certifications, time exceptions, driver eligibility) rather than a full enterprise HRIS.
**Why:** These additions deepen the existing three-database split without bloating the sample into unrelated ERP territory. They create believable legacy seams across desktop, web, services, and ETL/reporting while keeping the modernization story focused on operational routing, relationship management, and cross-system reporting.

**Implementation notes:**
- Put route planning, dispatch boards, driver zones, and mileage/reimbursement flows in **StoreOps**.
- Put partner account setup, contract pricing, referral channels, and shared order/catering interactions in **CustomerHub**.
- Put on-time delivery, route efficiency, labor overtime/turnover, and partner profitability scorecards in **Reporting**.
- Keep any HR detail bounded to what store managers and dispatch supervisors would realistically own in a legacy pizza platform.

### 2026-05-13T20:57:42.795+02:00: Scenario placement for delivery, workforce, and partner flows
**By:** Vasquez
**What:** Put delivery mapping and routing plus store-facing workforce workflows in `FabrikamPizza_StoreOps`, keep B2B partnership masters and external-account workflows in `FabrikamPizza_CustomerHub`, and push rollups, settlements, and scorecards into `FabrikamPizza_Reporting`; treat payroll and benefits as an external nightly integration, not a first-class in-app domain.
**Why:** That split preserves believable legacy seams: dispatch and staffing decisions need low-latency store data, partner contracts and shared contacts drift into a relationship-heavy hub, and reporting stays batch-oriented. Keeping full HR outside the app also creates the sort of brittle import boundary a modernization story can expose honestly.

### 2026-05-13T20:57:42.795+02:00: AJAX Control Toolkit use
**By:** User directive (captured by Scribe)
**What:** Some of the legacy web pages should use ASP.NET AJAX Control Toolkit controls and patterns.
**Why:** This reinforces the late-2000s / early-2010s organic-growth story of the sample and includes period-authentic web control patterns on selected Web Forms pages instead of plain server controls.

**Implementation notes:**
- Include AJAX Control Toolkit on selected Web Forms pages
- Use to reinforce late-2000s / early-2010s organic-growth story
- Fold into solution topology and page-level planning for legacy web apps

### 2026-05-13T20:57:42.795+02:00: Solution breadth over database alignment
**By:** User directive (captured by Scribe)
**What:** The sample should prioritize having enough projects and architectural seams to work well in presentations and to stress-test modernization tooling. Exact database alignment is now secondary to solution breadth.
**Why:** Solution breadth and architectural sprawl are more important for compelling demonstrations and tooling validation than perfect data-model purity. The next planning pass should expand business logic, service, web, desktop, integration, and reporting projects to reach a convincingly overgrown enterprise solution.

**Implementation notes:**
- Optimize next planning pass for project count and architectural sprawl
- Keep database boundaries believable, but don't let data-model purity reduce project count
- Expand to business logic, service, web, desktop, integration, and reporting projects

### 2026-05-13T21:07:52.000+02:00: Database seeding and legacy identity
**By:** User directive (captured by Scribe)
**What:** The sample should include database population scripts, and it should use an older version of ASP.NET Identity to reflect a common modernization challenge.
**Why:** SQL Server seed and population scripts should be first-class repo artifacts, not an afterthought. Making legacy authentication visible in the before-state by using an older ASP.NET Identity version reflects a real modernization pain point that samples typically gloss over.

**Implementation notes:**
- Add SQL Server seed and population scripts as first-class repo artifacts
- Use older ASP.NET Identity version (e.g., 2.2.3) to reflect legacy authentication patterns
- Reflect both items in project planning, package/version selection, and database setup workitems

### 2026-05-13T21:07:52.000+02:00: Legacy testing and DI patterns
**By:** User directive (captured by Scribe)
**What:** The sample should include test projects using an older testing framework such as NUnit, and some parts of the app should use an older DI library such as Enterprise Library or Autofac.
**Why:** Including at least one explicitly legacy-feeling test surface reinforces the "grown organically over time" story. Using uneven dependency injection patterns across the solution (not universal modern patterns) exposes the modernization seams that motivate the upgrade story.

**Implementation notes:**
- Add test projects using NUnit (older framework) alongside modern test tooling
- Use uneven DI patterns (Enterprise Library, Autofac, Castle Windsor) across solution
- Fold these choices into project planning and business-layer composition
- Leave some services using no DI (ad-hoc service locator patterns) to reinforce legacy feel

### 2026-05-13T21:07:52.000+02:00: Validation testing and CI/CD infrastructure
**By:** User directive (captured by Scribe)
**What:** The sample should have robust validation testing infrastructure, but much of that can live outside the legacy solution rather than pretending the enterprise app had strong native test coverage. The project should also have a path toward CI/CD.
**Why:** Keeping intentionally limited in-solution legacy tests but adding stronger external validation harnesses creates a realistic modernization story. Treating CI/CD as part of the supporting modernization/testing story (not proof that the original app was well engineered) is more credible and valuable for demonstrations.

**Implementation notes:**
- Keep some intentionally limited in-solution legacy tests, not comprehensive coverage
- Add stronger external validation harnesses for reliable development and demos
- Treat CI/CD as part of the supporting modernization/testing story, not original architecture
- Reflect in backlog items for external test infrastructure, build validation, and eventual pipeline automation

### 2026-05-13T21:07:52.000+02:00: Consolidated workitem backlog
**By:** Ripley (consolidated by Scribe)
**What:** 52 concrete, prioritized workitems organized into 9 phases for building the legacy .NET Framework sample across 14 projects, 3 SQL Server databases, and full legacy enterprise sprawl.
**Why:** The team needed a concrete execution backlog, not additional brainstorming. Workitems are specific, tech-authentic, dependency-aware, database-aligned, and documentation-first. Each workitem targets 1–2 days of work. Phase 1 (Foundation & Topology) is critical path blocker; Phases 2–3 (Data & Business Logic) and Phases 4–7 (Web, Services, Tests, Desktop) can parallelize once dependencies resolve.

**Implementation notes:**
- Phase 1 (11 items) must complete first to unblock Phases 2–9
- Phases 2–3 (14 items) can parallelize; feed Phases 4–7
- Phases 4–7 (21 items) parallelize by subsystem once Phases 1–3 complete
- Phases 8–9 (6 items) finalize documentation
- Tech stack: Enterprise Library 6.0, Autofac 4.9.2, NUnit 3.12, ASP.NET Identity 2.2.3, WCF, ASMX, Web Forms + AJAX Control Toolkit, Windows Forms, SSRS
- Dependency graph enables parallel execution; no blocking dependencies between teams
- Cross-team coordination with Hicks (web branding), Vasquez (database ETL), and Scribe (foundation docs)

### 2026-05-13T21:11:05+02:00: ASP.NET Web Forms Control Vendor Recommendation
**By:** Hicks
**What:** Include AJAX Control Toolkit (ACT) v20.1.0 as the legacy Web Forms control dependency to reinforce the 2008–2015 organic-growth story of the sample.
**Why:** ACT is a perfect fit: MIT open source, archived (Oct 2024) with no ongoing maintenance, peak era 2008–2015, and was the definitive library for AJAX/rich interactions before modern JS frameworks. Including it signals a credible 2010s codebase and creates a natural modernization narrative (ACT was a dead-end; use modern JS instead).

**Implementation notes:**
- Reference ACT v20.1.0 (June 2020) in legacy Fabrikam Pizza Web Forms pages
- Use one or two ACT controls (e.g., ModalPopupExtender, AutoCompleteExtender) on representative pages
- Include in project dependencies to signal organic technical debt and legacy surface area
- MIT License: fully permissive, no restrictions. Code is read-only on GitHub but legally usable.

### 2026-05-13T21:28:59.083+02:00: Repository Naming Guidance
**By:** Ripley
**What:** Keep GitHub repository name as `EnterpriseModernizationSample`. Use **Fabrikam Enterprise Pizza** as the scenario, solution, and business/domain name inside the repository.
**Why:** The repository is a modernization container holding before/after documentation, source layouts, and SQL Server assets; `EnterpriseModernizationSample` says what the repo is for. `Fabrikam Enterprise Pizza` gives the sample its Microsoft-style domain identity, supports the 2005-era story, and aligns with decisions already made for web branding, database names, and scenario expansion.

**Implementation notes:**
- Repository: `EnterpriseModernizationSample`
- Scenario / solution family: `Fabrikam Enterprise Pizza`
- Project/database naming: `FabrikamPizza.*`, `FabrikamPizza_StoreOps`, `FabrikamPizza_CustomerHub`, `FabrikamPizza_Reporting`

### 2026-05-13T21:28:59.083+02:00: Non-Microsoft Web Forms Control Dependency
**By:** User directive (captured by Scribe)
**What:** Include at least one believable third-party legacy Web Forms control/library in the before-state in addition to Microsoft-era controls.
**Why:** Reinforce the organically-grown vendor mix common in older enterprise apps; makes the sample feel like it evolved across multiple technology choices over time.

**Implementation notes:**
- Implemented via AJAX Control Toolkit (ACT) decision
- Use to signal real-world technical debt in modernization story
- Fold into package selection and web-surface planning

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
