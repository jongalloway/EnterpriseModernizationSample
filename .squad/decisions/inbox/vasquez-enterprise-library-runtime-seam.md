# Vasquez decision inbox

- **Date:** 2026-05-18T01:39:47.894-07:00
- **Agent:** Vasquez
- **Issue:** #24
- **Decision:** Bootstrap the legacy service hosts from `enterpriseLibrary:*` configuration keys so Unity registrations, log-writer defaults, exception policy behavior, and database aliases all come through one Enterprise Library-flavored runtime seam instead of hand-built `Global.asax` code.
- **Why:** That keeps the sample honestly mid-2000s: the service locator is still there, but the messy composition and policy wiring lives in config where operators and architects would expect it. It also gives downstream services a reusable pattern without pretending the repo has already modernized to clean constructor-injected startup code everywhere.
- **Impact:** DispatchHost and PartnerSync now resolve services from the same configurable bootstrap path, and later WCF/ASMX/desktop hosts can reuse the same reader, exception policy, and log writer behavior without another ad hoc container story.
