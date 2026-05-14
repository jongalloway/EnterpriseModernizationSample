# Vasquez decision inbox

- **Date:** 2026-05-14T15:35:48.472+02:00
- **Agent:** Vasquez
- **Issue:** #50
- **Decision:** Package the legacy database deployment runbook with checked-in SQLCMD command wrappers, an operator-editable environment file template, and a post-run audit script instead of assuming developers will execute loose `.sql` files by memory.
- **Why:** That keeps the mid-2000s DBA deployment posture believable, builds directly on the per-database script stack from #51, and exposes the deployment seam in a way later modernization demos can replace without pretending the old estate was cleaner than it was.
- **Impact:** Anyone wiring services, ETL, or modernization automation now has a canonical legacy deployment path with explicit validation and audit steps.
