# SQL Server Data Assets

Use this folder for SQL Server scripts, schema notes, deployment helpers, and sample data assets.

## Current legacy framing

- `data\sqlserver\before\` - legacy schemas, deploy scripts, seed data, and migration procedures
- `data\sqlserver\after\` - target-state database assets
- `data\sqlserver\shared\` - common fixtures, validation helpers, and cross-database orchestration scripts

Start with `data\sqlserver\before\deployment-guide.md` if you need the DBA-facing deployment order, wrapper scripts, and audit checkpoints.

## Legacy database split

- `FabrikamPizza_StoreOps`
- `FabrikamPizza_CustomerHub`
- `FabrikamPizza_Reporting`

The split is intentionally uneven and batch-heavy: transactional store activity stays in StoreOps, customer and partner data drifts into CustomerHub, and finance and operational rollups land in Reporting after nightly ETL.
