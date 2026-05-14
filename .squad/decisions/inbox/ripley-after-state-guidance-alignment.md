# After-State Guidance Alignment

**Date:** 2026-05-14T22:58:44.433+02:00  
**Decision Owner:** Ripley (Lead)  

## Context

Copilot review on PR #57 flagged several places where the after-state guidance drifted internally: route text did not match the controller example, worker scheduling guidance conflicted with the Kubernetes CronJob roadmap, and the cutover narrative mixed fallback/replication language with true dual-write terminology.

Because these documents are now the architectural baseline for downstream implementation, the wording must describe one believable operating model instead of a grab bag of patterns.

## Decision

- Treat modern REST routes as service-local resource paths (for example, `/api/stores/{id}`), not domain-prefixed URLs.
- Treat reporting and batch workers as containerized jobs scheduled by Kubernetes CronJob/Job resources; do not document self-scheduling `BackgroundService` loops as the recommended model.
- Reserve **dual-write** for short-lived, explicit reconciliation windows. The normal migration sequence is legacy-write plus ETL/replication verification until cutover.
- Mark `src\after\...` references in implementation guidance as planned locations until those projects exist in the repo.

## Rationale

- Reviewers need enforceable guidance. Ambiguous wording invites implementations that technically "fit the doc" while violating the intended seam.
- A nightly reporting pipeline belongs to the orchestrator schedule, not to a process that sleeps for 24 hours and hopes the host stays stable.
- Migration terminology matters: fallback, replication, and dual-write are three different risk profiles and should not be collapsed into one phrase.

## Consequences

- PR #57 documentation becomes authoritative enough for downstream implementers to copy patterns without inventing missing assumptions.
- Future after-state project scaffolding should follow the planned paths already named in the documents.
- Reviewers can reject any new guidance that reintroduces route-prefix ambiguity, self-scheduling workers, or sloppy dual-write language.
