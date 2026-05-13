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

📌 Scenario expansion locked on 2026-05-13T18:57:42.795Z:
  - **Decision:** Scenario expansion priorities — delivery dispatch/routing, workforce ops, B2B partnerships, delivery/labor/partner analytics
  - **StoreOps:** Route planning, dispatch boards, driver zones, mileage/reimbursement
  - **CustomerHub:** Partner account setup, contract pricing, referral channels, shared order/catering
  - **Reporting:** On-time delivery, route efficiency, labor overtime/turnover, partner profitability
  - **Cross-team:** Vasquez confirmed placement across databases with intentional legacy seams
