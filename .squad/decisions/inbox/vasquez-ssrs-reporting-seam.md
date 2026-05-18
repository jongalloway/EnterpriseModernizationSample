# Decision inbox: SSRS reporting seam

- **Date:** 2026-05-18T01:39:47.894-07:00
- **By:** Vasquez
- **Topic:** SSRS reporting seam

## Proposed decision
Keep the SSRS framework in `src\before\Fabrikam.EnterprisePizza.Reporting.Batch` with checked-in `.rdl` definitions, a shared `FabrikamPizza_Reporting` data source, and a `ReportExecution2005.asmx` wrapper that builds SOAP envelopes from batch-owned parameter models.

## Why
This keeps reporting downstream and deployment-friendly: DBAs can publish shared data sources and reports together, while batch jobs keep the legacy SSRS execution pattern isolated from StoreOps and CustomerHub runtime services.

## Impact
- Reporting stays the owner of scorecards, settlements, and scheduled render orchestration.
- SSRS assets deploy alongside the batch executable instead of leaking into transactional web/service projects.
- The reporting schema now explicitly includes `dbo.PartnerProfitabilitySummary` so partner profitability reports match the nightly ETL contract.
