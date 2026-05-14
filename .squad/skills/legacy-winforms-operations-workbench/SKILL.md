---
name: "legacy-winforms-operations-workbench"
description: "Build dense order/store desk WinForms screens that feel like extensions of a dispatch workstation"
domain: "desktop-ui"
confidence: "high"
source: "earned"
---

## Context
Use this when a legacy desktop shell needs additional operational workbenches without drifting into a cleaner, more modern UI pattern. It fits order lookup, store oversight, counter follow-up, and similar workstation tasks.

## Patterns
- Launch secondary desks as modal forms from the main shell so the application still feels like a single workstation utility.
- Keep the main work area grid-heavy: one primary grid for the operator list, one secondary grid for the selected row's detail trail, plus a right sidebar for summary metrics and reminders.
- Pull list data from a business/service seam, but derive screen-specific detail rows inside the WinForms form so desktop behavior stays obvious.
- Reuse the same toolbar, group box, and status strip structure across desks so operators do not have to relearn the shell.

## Examples
- src/before/Fabrikam.EnterprisePizza.Desktop.DispatchBoard/OrderLookupForm.cs
- src/before/Fabrikam.EnterprisePizza.Desktop.DispatchBoard/StoreManagementForm.cs
- src/before/Fabrikam.EnterprisePizza.Business.StoreOps/Services/StoreOperationsWorkbenchService.cs

## Anti-Patterns
- Do not replace dense operator grids with card layouts, wizard flows, or web-style navigation.
- Do not push every detail row into shared contracts when the detail only exists to support a single desktop screen.
