---
name: "legacy-web-portal-shell"
description: "Build a believable mid-2000s Web Forms portal shell that fronts older sidecar web apps without collapsing them into one surface."
domain: "web-forms"
confidence: "medium"
source: "earned"
---

## Context

Use this when a legacy sample needs a main intranet or partner portal, but the repo already contains more specialized Web Forms or MVC applications. The portal should feel like a polished front door layered on top of an already messy estate.

## Pattern

- Use a **Web Forms master page** with glossy gradient chrome, utility nav, and footer-era cues like MySpace links or “best viewed” hints.
- Keep the portal focused on **dashboard and routing** responsibilities: quick links, alerts, promo/status cards, and a district snapshot page are usually enough.
- Link out to specialized surfaces using plausible virtual-directory style URLs (`/CustomerHub/...`, `/FranchisePortal/...`, `/Storefront/`) instead of pretending everything lives in one clean application.
- Add light **jQuery-era behavior** as progressive enhancement only; the page should still read clearly if the old script does nothing.
- Prefer hardcoded sample data or simple `DataTable` binding in the before-state unless a work item explicitly asks for deeper service/data integration.

## Anti-Patterns

- Turning the portal into the new single source of truth for every workflow.
- Making the UI too modern or too ironic; it should look aged, not self-aware.
- Blocking the page on JavaScript when a classic server-rendered fallback would feel more authentic.
