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

## Learnings

- 2026-05-14T15:35:48.472+02:00: Package the SQL estate as numbered per-database SQLCMD scripts plus shared orchestration files so deployment order and cross-database seams stay visible.
- 2026-05-14T15:35:48.472+02:00: Keep service-facing procedure names aligned with StoreOps dispatch reads, CustomerHub partner reads, and Reporting dashboard reads so later DAL work has a believable contract to target.
- 2026-05-14T15:35:48.472+02:00: Key database deployment files now live under `data\sqlserver\before\Deploy\00-deploy-all.sql`, `data\sqlserver\shared\Migration\01-run-nightly-sync.sql`, and `data\sqlserver\before\deployment-guide.md`.
- 2026-05-14T15:35:48.472+02:00: The deployment story lands better when the SQLCMD stack is paired with thin `.cmd` wrappers, a copied environment template, and a separate audit script instead of telling operators to edit SQL headers by hand every time.
- 2026-05-14T15:35:48.472+02:00: Key issue #50 files are `data\sqlserver\before\Deploy\01-deploy-all.cmd`, `data\sqlserver\before\Deploy\00-set-environment.sample.cmd`, and `data\sqlserver\shared\Migration\03-deployment-audit.sql`.
