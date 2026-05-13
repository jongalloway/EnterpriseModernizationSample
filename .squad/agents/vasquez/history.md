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

## Learnings

- WCF, ASMX, and Enterprise Library-style layering are core to the sample's identity.
- Configuration-heavy and SOAP-era integration patterns are desirable, not accidental.
- Fabrikam Enterprise Pizza works better than Enterprize Pizza for the sample's 2005-era Microsoft-demo tone, and the data story should split OLTP, customer/franchise, and reporting concerns across separate SQL Server databases.
