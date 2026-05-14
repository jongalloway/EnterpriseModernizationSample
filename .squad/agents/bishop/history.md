# Project Context

- **Owner:** Scribe
- **Project:** EnterpriseModernizationSample
- **Stack:** C#, .NET Framework, Windows Forms, ASP.NET Web Forms, ASP.NET MVC, WCF, ASMX, Enterprise Library-style patterns, classic jQuery, SQL Server-style data access
- **Description:** Sample legacy line-of-business application built to feel like a real enterprise system started around 2005 and expanded over time.
- **Created:** 2026-05-13

## Core Context

Bishop owns the legacy desktop shell, utility screens, and thick-client workflows.

## Recent Updates

📌 Team initialized on 2026-05-13

## Learnings

- The desktop app should feel like a long-lived internal tool, not a modern showcase UI.
- Old patterns such as settings dialogs, grid-heavy forms, and service-backed desktop actions fit the brief.
- 2026-05-14T15:35:48.472+02:00 — Route planning and driver assignment belong as modal desks off the dispatch shell in `src\before\Fabrikam.EnterprisePizza.Desktop.DispatchBoard\DispatchBoardForm.cs`, not as separate shell apps.
- 2026-05-14T15:35:48.472+02:00 — Desktop-only cues such as waves, mileage, ETAs, and queue status stay derived inside `RoutePlanningForm.cs`, `DriverAssignmentForm.cs`, and `DispatchBoardForm.cs` so the shared `DispatchTicket` contract stays narrow.
- 2026-05-14T15:35:48.472+02:00 — Terminal defaults now live in `src\before\Fabrikam.EnterprisePizza.Desktop.DispatchBoard\App.config`, `DispatchBoardAppSettings.cs`, and `DispatchBoardSettingsForm.cs` for legacy workstation-style setup.
