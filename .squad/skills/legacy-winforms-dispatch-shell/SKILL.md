---
name: "legacy-winforms-dispatch-shell"
description: "Build a dense WinForms dispatch workstation with settings-backed terminal behavior and related routing desks."
domain: "desktop-ui"
confidence: "high"
source: "observed"
---

## Context

Use this skill when a legacy desktop surface needs to feel like an in-store operations workstation instead of a demo UI.

## Patterns

1. Keep the main dispatch form dense: toolbar up top, ticket grid in the center, summary/details in a right-side panel, status strip at the bottom.
2. Read per-terminal defaults from `App.config` and save operator tweaks through a small modal options dialog instead of a modern settings experience.
3. Launch related route planning and driver assignment desks from the main dispatch shell so the operator workflow stays in one old-client hub.
4. Bind grids to DTOs or simple view models from the business/service seam, then derive waves, ETAs, mileage, reminders, and queue cues in the desktop layer.
5. Use refresh buttons and auto-refresh together so each desk feels like a workstation board rather than a static report.

## Examples

- `src\before\Fabrikam.EnterprisePizza.Desktop.DispatchBoard\DispatchBoardForm.cs`
- `src\before\Fabrikam.EnterprisePizza.Desktop.DispatchBoard\RoutePlanningForm.cs`
- `src\before\Fabrikam.EnterprisePizza.Desktop.DispatchBoard\DriverAssignmentForm.cs`
- `src\before\Fabrikam.EnterprisePizza.Desktop.DispatchBoard\DispatchBoardSettingsForm.cs`

## Anti-Patterns

- Spinning up a separate shell executable for each dispatch sub-workflow.
- Widening shared service contracts just to carry UI-only columns or reminders.
- Replacing dense operator screens with sparse modern layouts that hide active work.
