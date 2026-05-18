# Vasquez decision inbox

- **Date:** 2026-05-15T08:40:39.286+02:00
- **Agent:** Vasquez
- **Issue:** #18
- **Decision:** Treat StoreOps DAL completion as two believable seams: dispatch-board reads stay in their own repository, and POS import monitoring gets a separate repository that maps the latest batch row out of the shared `DataSet` gateway for integration jobs.
- **Why:** That keeps the transport/data plumbing honestly ugly without pretending a nightly import job would live forever as a connection-name helper or reach straight into tables. It also gives later business/service work a real batch seam to modernize instead of another fake inline stub.
- **Impact:** `NightlyPosImportJob` now consumes StoreOps DAL output instead of only exposing the database alias, and future order/POS work can extend StoreOps repositories without collapsing dispatch and batch concerns together.
