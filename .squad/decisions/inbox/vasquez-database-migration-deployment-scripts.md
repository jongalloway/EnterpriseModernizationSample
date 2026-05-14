# Vasquez decision inbox

- **Date:** 2026-05-14T15:35:48.472+02:00
- **Agent:** Vasquez
- **Issue:** #51
- **Decision:** Package the legacy database estate as per-database numbered SQLCMD scripts plus shared cross-database orchestration scripts, with migration work expressed as stored procedures that leave CustomerHub-to-StoreOps sync and Reporting rollups visibly batch-driven.
- **Why:** That shape stays era-authentic for a DBA-run SQL Server deployment, aligns with the three-database ownership split, and gives later modernization work a concrete deployment seam instead of one opaque bootstrap script.
- **Impact:** Service and reporting code can now point at believable stored-procedure names, and future after-state work has a documented deployment story to replace.
- **Addendum (2026-05-14T22:58:44.433+02:00):** The nightly bridge now treats CustomerHub extracts as rolling staged batches with retention, lets StoreOps delete cache rows that disappear from the active source set, and requires an explicit deploy-root variable when SQLCMD includes need to survive arbitrary launch directories.
