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
