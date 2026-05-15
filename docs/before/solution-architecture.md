# Legacy Solution Architecture

**Document date:** 2026-05-15T04:37:01.332+02:00

This document is the working architecture baseline for the **before** state of **Fabrikam Enterprise Pizza**. It records the current solution topology, the intended project seams inside the legacy estate, and the modernization roadmap that should guide downstream implementation work.

## 1. Architectural posture

The sample is intentionally broad, uneven, and believable:

- One Visual Studio solution holds desktop, web, service, batch, business, data, and test projects.
- Three SQL Server databases split operational writes, customer/partner data, and reporting rollups.
- Web Forms, MVC 5, WCF, ASMX, WinForms, Enterprise Library-style data access, and `packages.config` coexist by design.
- The current repository contains mostly scaffolding. The shape is useful already; the implementation depth is not there yet.

That means the first architectural job is not polishing internals. It is protecting seams so later work does not collapse into one generic “legacy app” blob.

## 2. System topology at a glance

### Business domains

| Domain | Responsibility | Primary legacy assets | Database |
| --- | --- | --- | --- |
| **StoreOps** | Store execution, dispatch, routes, workforce operations, POS staging | `Business.StoreOps`, `Services.DispatchHost`, `Desktop.DispatchBoard`, `StoreOps.Portal`, `Integrations.PosSync` | `FabrikamPizza_StoreOps` |
| **CustomerHub** | Corporate accounts, catering, franchise/partner relationships, referral channels | `Business.CustomerHub`, `Services.PartnerSync`, `Portal`, `Web.CustomerHub`, `Web.FranchisePortal` | `FabrikamPizza_CustomerHub` |
| **Reporting** | Nightly rollups, delivery KPIs, labor summaries, partner profitability | `Reporting.Batch` plus future SSRS assets | `FabrikamPizza_Reporting` |
| **Commerce edge** | Public ordering, promotions, sign-in, coupons | `Web.Storefront` | Mixed reads, anchored on StoreOps and CustomerHub |
| **Shared platform** | Domain primitives and service DTOs | `Core`, `Shared.Contracts`, selected `Data` seams | Cross-cutting |

### Runtime shape

```text
Customers / franchise staff / store staff
    |
    +-- Portal (ASP.NET Web Forms)
    +-- StoreOps.Portal (ASP.NET Web Forms)
    +-- Web.Storefront (ASP.NET MVC 5)
    +-- Web.FranchisePortal (ASP.NET Web Forms)
    +-- Web.CustomerHub (ASP.NET Web Forms)
    +-- Desktop.DispatchBoard (WinForms)
    |
    +-- Services.DispatchHost (WCF / SOAP)
    +-- Services.PartnerSync (ASMX / XML)
    |
Business.* + Data + Integrations.PosSync + Reporting.Batch
    |
    +-- FabrikamPizza_StoreOps
    +-- FabrikamPizza_CustomerHub
    +-- FabrikamPizza_Reporting
```

## 3. Solution structure

### Repository structure

| Path | Purpose |
| --- | --- |
| `docs\foundation\` | Scenario framing, naming, package direction |
| `docs\before\` | Legacy-state architecture, topology, operating context |
| `docs\after\` | Target-state migration outputs |
| `src\before\` | Legacy .NET Framework solution |
| `src\after\` | Modernized solution space, not yet populated |
| `data\sqlserver\before\` | Legacy schema, seed, deploy, and operational SQL assets |
| `data\sqlserver\after\` | Target-state database assets |
| `data\sqlserver\shared\` | Shared fixtures and migration helpers |

### Legacy solution inventory

| Project | Type | Layer | Current dependency direction | Architectural role |
| --- | --- | --- | --- | --- |
| `Fabrikam.EnterprisePizza.Core` | Class library | Foundation | none | Shared domain primitives and low-level business types |
| `Fabrikam.EnterprisePizza.Shared.Contracts` | Class library | Foundation | none | DTO/contracts for WCF, ASMX, and desktop integration |
| `Fabrikam.EnterprisePizza.Business.StoreOps` | Class library | Business | `Core`, `Shared.Contracts` | Dispatch, route, and workforce orchestration |
| `Fabrikam.EnterprisePizza.Business.CustomerHub` | Class library | Business | `Core` | Partner and account workflow logic |
| `Fabrikam.EnterprisePizza.Data` | Class library | Data | `Core` | Enterprise Library-era gateway and stored-procedure seam |
| `Fabrikam.EnterprisePizza.Integrations.PosSync` | Class library | Integration | `Data` | Nightly POS/import job surface |
| `Fabrikam.EnterprisePizza.Services.DispatchHost` | ASP.NET / WCF | Service | `Business.StoreOps`, `Shared.Contracts` | Store dispatch SOAP host |
| `Fabrikam.EnterprisePizza.Services.PartnerSync` | ASP.NET / ASMX | Service | `Business.CustomerHub` | Partner-facing XML web service |
| `Fabrikam.EnterprisePizza.Portal` | ASP.NET Web Forms | Web | package refs only today | Main partner portal shell that links older franchise, customer, and storefront surfaces |
| `Fabrikam.EnterprisePizza.StoreOps.Portal` | ASP.NET Web Forms | Web | package refs only today | Back-office dispatch, labor watch, and zone bulletin portal for store supervisors |
| `Fabrikam.EnterprisePizza.Web.Storefront` | ASP.NET MVC 5 | Web | package refs only today | Public ordering and legacy identity edge |
| `Fabrikam.EnterprisePizza.Web.FranchisePortal` | ASP.NET Web Forms | Web | package refs only today | Franchise notices, promo editing, bulletins |
| `Fabrikam.EnterprisePizza.Web.CustomerHub` | ASP.NET Web Forms | Web | package refs only today | Corporate account and catering workflows |
| `Fabrikam.EnterprisePizza.Desktop.DispatchBoard` | Windows Forms | Desktop | isolated today | In-store dispatch and route board |
| `Fabrikam.EnterprisePizza.Reporting.Batch` | Console | Reporting | `Business.StoreOps` today | Nightly extracts and reporting feed jobs |
| `Fabrikam.EnterprisePizza.Legacy.Tests` | NUnit library | Validation | `Business.StoreOps` | Sparse regression coverage, intentionally thin |
| `Fabrikam.EnterprisePizza.Tests.Unit` | NUnit library | Validation | `Business.StoreOps`, `Business.CustomerHub`, `Data` | Focused seam-level coverage for legacy stubs and services |

## 4. Boundary rules

These rules keep the sample survivable as it fills out:

1. **Database ownership stays explicit.**
   - Store execution data belongs in `StoreOps`.
   - Customer, franchise, and partner master data belongs in `CustomerHub`.
   - Finance, KPI, and scorecard rollups belong in `Reporting`.
2. **Reporting stays batch-fed.** Do not let UI projects start querying reporting tables directly.
3. **Service contracts sit at the seam, not in UI projects.** SOAP/XML DTOs belong in `Shared.Contracts`, not copied per host.
4. **Web stacks stay side-by-side, not blended.**
   - MVC owns public storefront concerns.
   - Web Forms owns franchise/admin/corporate workflows.
   - Do not “simplify” by moving everything into a single web project.
5. **Legacy inconsistency is intentional.** Uneven DI, package age, and service patterns are part of the modernization story.
6. **`src\after\` must decompose by business seam, not by legacy project name.** The modernization target should preserve domain boundaries while deleting technology sprawl.

## 5. Known modernization pain points

| Pain point | Why it matters |
| --- | --- |
| Multiple UI stacks | Demonstrates channel sprawl and duplicated web concerns |
| WCF + ASMX endpoints | Creates concrete SOAP/XML retirement work |
| `packages.config` and older packages | Forces package-management and dependency uplift work |
| Legacy ASP.NET Identity in storefront | Makes auth modernization visible instead of hidden |
| Enterprise Library-style data access | Exposes data access refactoring and configuration seams |
| Three-database estate | Prevents fake simplicity; requires data ownership decisions |
| Thin in-solution tests | Keeps the legacy baseline believable while justifying external validation |

## 6. Modernization roadmap

The roadmap is a sequence, not a wish list. Foundation and data seams land first; fan-out comes later.

### Workstream 1 — Foundation and topology

- Lock project boundaries, package direction, and folder ownership.
- Finish missing composition work (`#9`) and keep scaffolding aligned with the documented topology.
- Use this issue (`#1`) as the baseline architecture read; use `#49` to finalize architecture documentation once more implementation exists.

### Workstream 2 — Data estate

- Build the three database schemas (`#12`-`#14`).
- Add seed/population scripts (`#15`-`#17`).
- Implement legacy DAL seams with stored-procedure/DataSet bias (`#18`-`#20`).

**Gate:** no broad UI expansion before the three-database shape is real.

### Workstream 3 — Business logic

- Fill in order/POS, dispatch, and partner/account logic (`#21`-`#24`).
- Keep StoreOps and CustomerHub separate even when both need shared customer/order identifiers.

**Gate:** shared contracts may cross domains; business services should not collapse into one omnibus project.

### Workstream 4 — Presentation surfaces

- Flesh out storefront, franchise, and customer portal experiences (`#25`-`#31`).
- Keep public commerce in MVC and operational/admin workflows in Web Forms.
- Apply the dated web chrome and control stack intentionally, not everywhere.

### Workstream 5 — Services and integration

- Build WCF/ASMX/service surfaces and batch connectors (`#32`-`#36`, `#52`).
- Preserve the ugly seams now so the after-state has something honest to remove.

### Workstream 6 — Desktop and validation

- Build the WinForms dispatch shell and operations screens (`#37`-`#39`).
- Add NUnit-based legacy tests and smoke coverage (`#40`-`#44`), but keep expectations realistic.

### Workstream 7 — Reporting and migration closeout

- Add reporting assets and SQL deployment documentation (`#45`-`#51`).
- Use `src\after\` and `docs\after\` to map legacy seams into modern services, workers, and web apps once the before-state is credible.

## 7. Recommended after-state seams

Do not migrate project-for-project. Migrate seam-for-seam:

| Legacy seam | Likely after-state target |
| --- | --- |
| `Web.Storefront` + legacy Identity | Customer web app + modern identity boundary |
| `Business.StoreOps` + `StoreOps.Portal` + `Services.DispatchHost` + `Desktop.DispatchBoard` | StoreOps service/API + operations UI |
| `Portal` + `Web.FranchisePortal` + `Web.CustomerHub` + `Business.CustomerHub` + `Services.PartnerSync` | Customer/Partner service + back-office UI |
| `Integrations.PosSync` + reporting batch feeds | Worker services / scheduled integration jobs |
| `Reporting.Batch` + SSRS assets | Reporting pipeline, warehouse feeds, or BI boundary |
| `Data` omnibus gateway | Per-domain persistence adapters |

That is the point of this sample: the legacy solution is allowed to be sprawling, but the modernization path should cut along business ownership seams, not preserve every historical project.

## 8. Reviewer guidance

Reject downstream work when it does any of the following:

- merges StoreOps and CustomerHub “for convenience”
- lets reporting become a live operational dependency
- moves Web Forms and MVC into one presentation blob
- hides SOAP/XML seams instead of documenting and containing them
- adds implementation detail that contradicts the three-database ownership model

If the structure starts drifting, stop the work and reset the seam. A cleaner wrong shape is still wrong.
