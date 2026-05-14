# SQL Server Data Assets

Use this folder for SQL Server scripts, schema notes, deployment helpers, and sample data assets.

## Current legacy framing

- `data\sqlserver\before\` - legacy schemas, deploy scripts, and seed data
- `data\sqlserver\after\` - target-state database assets
- `data\sqlserver\shared\` - common fixtures, lookup data, and migration helpers

## Legacy database split

- `FabrikamPizza_StoreOps`
- `FabrikamPizza_CustomerHub`
- `FabrikamPizza_Reporting`

The split is intentionally uneven and batch-heavy: transactional store activity stays in StoreOps, customer and partner data drifts into CustomerHub, and finance and operational rollups land in Reporting after nightly ETL.
