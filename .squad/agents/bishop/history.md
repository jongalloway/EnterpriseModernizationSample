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
- 2026-05-14T15:35:48.472+02:00: Added OrderLookupForm and StoreManagementForm under src/before/Fabrikam.EnterprisePizza.Desktop.DispatchBoard as dense modal workbenches off the dispatch shell, backed by StoreOperationsWorkbenchService.
- 2026-05-14T15:35:48.472+02:00: Shared.Contracts StoreOps records now carry order/store desk data, while the desktop layer derives workflow and terminal detail grids locally to keep WinForms behavior understandable.
- 2026-05-14T15:35:48.472+02:00: Validation path for legacy desktop work is dotnet restore + dotnet msbuild on src/before/Fabrikam.EnterprisePizza.Legacy.sln, plus Windows PowerShell form instantiation against the built net48 assemblies when no direct UI test harness exists.
