---
name: "legacy-webforms-order-binder"
description: "Build internal order-management Web Forms pages with ObjectDataSource, GridView, DetailsView, and ViewState-heavy filter handling."
domain: "web"
confidence: "high"
source: "earned"
---

## Context

Use this when a legacy .NET sample needs an internal operations page that should feel like a mid-2000s supervisor tool instead of a polished customer-facing experience.

## Patterns

- Keep the flow in the Web Forms intranet shell when the users are supervisors, dispatch clerks, or store managers.
- Use `ObjectDataSource` as the binding seam even when the backing data is still sample/business-service driven.
- Store current filter state and sort expressions in ViewState, then feed those values into `ObjectDataSource.Selecting` handlers.
- Pair `GridView` list pages with a `DetailsView` ticket page and a history page so the old binder metaphor feels complete.
- Add obvious utility-nav links and chunky gradient buttons so the pages read as 2005-2008 enterprise data entry surfaces.

## Example Shape

- `OrderLookup.aspx` with filter toolbar, `GridView`, paging, sorting, and object data source.
- `OrderDetails.aspx` with `DetailsView`, status timeline grid, and follow-up notes.
- `OrderHistory.aspx` with date-range filters and a packet-style history grid.
- A small portal-local data source class that adapts service data into UI-specific row shapes.

## Anti-Patterns

- Moving internal order-ops pages into MVC just because the code would look cleaner.
- Hiding all filter state in query strings when the goal is to feel ViewState-heavy.
- Binding `GridView` controls directly to raw service contracts when the page needs UI-specific flags, notes, and packet language.
- Making the intranet shell look too modern or too sarcastically broken.
