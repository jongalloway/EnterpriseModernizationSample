# Hudson decision: service smoke observability

- **Date:** 2026-05-14T15:35:48.472+02:00
- **Issue:** #44
- **Decision:** Legacy service smoke coverage should validate three layers together: a callable sample payload, the host directive file (`.svc` or `.asmx`), and the transport/protocol wiring in `web.config`.
- **Why:** Directly instantiating the legacy service classes keeps the suite cheap and deterministic, while the file-based checks still catch the embarrassing regressions where the endpoint shell or metadata exposure drifts out of sync with the code.
- **Reviewer guidance:** Treat failures here as service-surface regressions first, not just unit-test noise; broken host directives or transport settings are exactly the kind of legacy outage these smoke checks are meant to spotlight.
