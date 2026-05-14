# Project Context

- **Owner:** Scribe
- **Project:** EnterpriseModernizationSample
- **Stack:** C#, .NET Framework, Windows Forms, ASP.NET Web Forms, ASP.NET MVC, WCF, ASMX, Enterprise Library-style patterns, classic jQuery, SQL Server-style data access
- **Description:** Sample legacy line-of-business application built to feel like a real enterprise system started around 2005 and expanded over time.
- **Created:** 2026-05-13

## Core Context

Vasquez owns the service and data layers that make the sample feel like a long-running enterprise system.

## Recent Updates

📌 Team initialized on 2026-05-13
📌 Decision consolidation on 2026-05-13T18:54:45Z: Scribe merged 5 inbox decisions into canonical `decisions.md`. Your "Fabrikam pizza data layout" decision locked: three SQL Server databases (FabrikamPizza_StoreOps, FabrikamPizza_CustomerHub, FabrikamPizza_Reporting) reflecting authentic mid-2000s enterprise patterns.
  - **Cross-team:** Ripley finalized Fabrikam Enterprise Pizza with 2005-era throwbacks and repo framing. Hicks locked web direction to polished 2005 corporate portal (blue gradients, MySpace, promo panels).
📌 Scenario placement on 2026-05-13T20:57:42Z: Your "Scenario placement for delivery, workforce, and partner flows" decision aligned delivery/dispatch, workforce workflows, and HR integration with database seams. Treats payroll/benefits as external nightly integration.
  - **Cross-team:** Ripley's "Scenario expansion priorities" decision confirmed delivery dispatch/routing in StoreOps, B2B partnerships in CustomerHub, cross-system analytics in Reporting.

## Learnings

- WCF, ASMX, and Enterprise Library-style layering are core to the sample's identity.
- Configuration-heavy and SOAP-era integration patterns are desirable, not accidental.
- Fabrikam Enterprise Pizza works better than Enterprize Pizza for the sample's 2005-era Microsoft-demo tone, and the data story should split OLTP, customer/franchise, and reporting concerns across separate SQL Server databases.
- Scenario placement works best when StoreOps owns execution-time workflows, CustomerHub owns relationship/master data, and Reporting stays downstream; payroll-grade HR should remain an external feed rather than a magically unified module.
- 2026-05-14T03:23:27.208+02:00: The DAL lands cleanest as a connection-catalog plus stored-procedure wrapper seam, with business services pulling through repository classes instead of burying fake records in service code.
- 2026-05-14T03:23:27.208+02:00: The services read truer when WCF keeps `basicHttpBinding` plus mex metadata and ASMX exposes explicit XML envelope DTOs from `Shared.Contracts` instead of leaking raw strings across the seam.

📌 Scenario placement locked on 2026-05-13T18:57:42.795Z:
  - **Decision:** Scenario placement for delivery, workforce, and partner flows
  - **StoreOps:** Delivery mapping, routing, store-facing workforce workflows, driver availability, staffing exceptions, dispatch compliance
  - **CustomerHub:** B2B partnership masters, external-account workflows, relationship/contract data
  - **Reporting:** Rollups, settlements, scorecards, route efficiency, labor variance, partner profitability
  - **External:** Payroll and benefits via nightly integration (not first-class domain)
  - **Cross-team:** Ripley confirmed expansion roadmap; intentional legacy seams preserved for modernization exposure

📌 Workitem consolidation on 2026-05-13T21:09:53Z:
- Confirmed data/service layer ownership across 52-item backlog: Phase 2 (schema), Phase 3 (DAL), Phase 5 (services/ETL)
- 3-database split verified: FabrikamPizza_StoreOps, FabrikamPizza_CustomerHub, FabrikamPizza_Reporting
- Cross-database seams defined: StoreOps↔CustomerHub (master sync), StoreOps/CustomerHub→Reporting (nightly rollup)
- Tech stack: Enterprise Library 6.0, DataSets, ad-hoc SQL, stored procedures, WCF + ASMX, batch ETL, legacy messaging
- External payroll feed as intentional brittle boundary (modernization exposure point)
- Phase 2 delivery is critical path for Phases 3, 4, 5; Phase 5 unblocks Phase 8 reporting
- Orchestration log created at `.squad\orchestration-log\2026-05-13T21-09-53Z-vasquez-workitem-planning.md`
- Scribe consolidation ensures Phases 2–3 (data/DAL) parallelize once Ripley Phase 1 completes

📌 Decision archive round-up on 2026-05-14T01:02:28Z (Scribe):
- Canonical decisions.md now locked with 3 new detailed decisions appended
- **Repo naming clarity:** EnterpriseModernizationSample (container) vs Fabrikam Enterprise Pizza (scenario)—clears branding seams for documentation and code
- **Control stack locked:** ACT v20.1.0 for Web Forms (Hicks coordination)
- **Cross-team:** Ripley and Hicks notified of decision consolidation and updated backlog status assessment (5–8% Phase 1 complete).

## 2026-05-14: Ripley Workitem Setup Complete

Your Phases 2-3, 5, and 8 issues (Data/DAL, Services, Reporting) are routed with 'squad:vasquez' label. Phase 2-3 gates downstream; can parallelize after Phase 1 ~50% complete.
## 2026-05-14: Squad orchestration session

- Ripley: Issue #1 solution architecture complete. Created docs\before\solution-architecture.md, recorded modernization-seams decision. Baseline topology established for all downstream work.
- Vasquez: Issue #3 data project foundation complete. Fabrikam.EnterprisePizza.Data project built with connection catalog, stored-procedure gateway, repository plumbing. Recorded data-project-seam decision. Ready for service layer integration.

**Team impact:**
- Both decisions merged into canonical decisions.md
- Orchestration logs written: .squad/orchestration-log/
- Session summary logged: .squad/log/
- Next gate: Monitor Phase 1 ~50% completion before gating Phase 2+

📌 Vasquez Issue #4 completion on 2026-05-14T01:23:27Z:
- **Task:** Issue #4 services project — WCF and ASMX service stubs using shared contracts and metadata
- **Outcome:** Completed and closed
- **Decisions recorded:** 
  - "Service stub contracts" — WCF uses basicHttpBinding + mex, ASMX returns XML envelope with DTOs from Shared.Contracts
  - "Data project stored procedure seam" — connection catalog, stored-procedure gateway, repository classes for DAL seam
- **Cross-team:** Vasquez Phase 2 (database schema) is critical path for Phases 3, 4, 5. Ripley baseline topology ready; services integrate against documented seams.
- **Orchestration log:** .squad/orchestration-log/2026-05-14T01-23-27Z-vasquez.md
