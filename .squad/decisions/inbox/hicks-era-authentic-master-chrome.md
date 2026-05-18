# Hicks decision inbox

- **Date:** 2026-05-18T01:39:47.894-07:00
- **By:** Hicks
- **Topic:** Era-authentic master page chrome

## Decision
Center the 2005-2008 portal chrome in `src\before\Fabrikam.EnterprisePizza.Portal\Site.master`, using shared master-page markup for the marquee ticker, promo badge rail, newsletter signup, visitor counter, utility nav, and nostalgic footer details.

## Why
The dated marketing shell is part of the portal's identity rather than page-specific business content. Keeping it in the master page makes the old-web aesthetic consistent across current and future portal pages while avoiding duplicated chrome blocks in each `.aspx` file.

## Impact
- Portal pages inherit the same era-authentic wrapper automatically.
- Future content pages can stay focused on business modules instead of repeating newsletter and badge markup.
- The modernization story stays legible because shared chrome is clearly separated from page-level workflows.
