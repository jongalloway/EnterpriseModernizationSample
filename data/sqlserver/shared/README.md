# Shared SQL Assets

Use this folder for lookup data, reference fixtures, and migration helpers used by both the before and after states.

## Shared migration helpers

- `Migration\01-run-nightly-sync.sql` - drives the CustomerHub extract, StoreOps cache refresh, and Reporting rollup procedures in sequence
- `Migration\02-smoke-test.sql` - runs post-deployment checks against the legacy service-facing procedures and seeded tables

Keep the shared area orchestration-focused. Schema ownership still belongs to the individual database folders.
