# CustomerHub contract and franchise seams

- **Date:** 2026-05-15T08:40:39.286+02:00
- **By:** Vasquez
- **Related issue:** #13

## Decision

Model `FabrikamPizza_CustomerHub` around four explicit legacy tables: `PartnerAccount`, `CorporateAccount`, `FranchiseLocation`, and `PartnerContract`. Keep contracts as their own rows that point at a partner and optionally a corporate account or franchise location, then surface that catalog through CustomerHub stored procedures.

## Why

That shape keeps catering pricing and franchise relationships out of StoreOps, preserves the crooked legacy seam between partner masters and pricing agreements, and gives later DAL/service work something honest to wrap instead of hard-coded composite columns.

## Files

- `data\sqlserver\before\CustomerHub\02-schema.sql`
- `data\sqlserver\before\CustomerHub\03-seed-data.sql`
- `data\sqlserver\before\CustomerHub\04-service-procedures.sql`
