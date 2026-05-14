---
name: "legacy-webforms-portal-shell"
description: "Pattern for a believable back-office ASP.NET Web Forms portal shell"
domain: "web-forms"
confidence: "medium"
source: "earned"
---

## Context
Use this when the sample needs an internal portal that feels like it grew up beside WinForms tools and MVC sites instead of replacing them. It works well for supervisor dashboards, bulletin centers, and admin shells that should look obviously older than the public storefront.

## Pattern

- Create a dedicated Web Forms project instead of burying back-office pages inside the MVC storefront.
- Give it a master page with blue-gradient corporate chrome, utility nav, terminal hints, and footer copy that feels operational instead of marketing-led.
- Bind one representative page directly to existing business-layer services so the portal feels server-heavy and believable.
- Add modest AJAX-era behavior such as UpdatePanel refreshes, ACT tabs/modals, and a small legacy script file that can use jQuery when present.
- Keep the page practical: dispatch grids, labor watch tables, zone notes, and supervisor reminder panels tell the modernization story faster than generic lorem ipsum admin pages.

## Notes

- If the page uses ScriptManager or UpdatePanel, add `System.Web.Extensions` explicitly.
- If the page uses classic MVC `ViewBag` or older Web Forms AJAX packages, expect to restore packages and add missing framework references before validating the full solution.
