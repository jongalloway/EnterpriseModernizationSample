# Modernized Solution Architecture

**Document date:** 2026-05-14T15:35:48.472+02:00

This document describes the target state of **Fabrikam Enterprise Pizza** after migration from the legacy 2005-era .NET Framework estate. It maps legacy seams to modern components, documents boundary decisions that preserve the three-database model, and establishes patterns for phased cutover.

## 1. Architectural posture

The modernization intent is not to rewrite; it is to **decompose along business seams while retiring technology sprawl**.

- One Visual Studio solution for each business domain (StoreOps, CustomerHub, Reporting) replaces the monolithic legacy solution.
- Modern ASP.NET Core and .NET services replace Web Forms, MVC, and WCF/ASMX.
- Windows background workers replace console batch jobs.
- SQL Server remains the primary data store; per-domain schemas live in separate databases (`FabrikamPizza_StoreOps`, `FabrikamPizza_CustomerHub`, `FabrikamPizza_Reporting`).
- The three-database split is preserved intentionally; it prevents operational data from polluting reporting tables and keeps domain ownership explicit.

The first migration target is **domain independence**. Each domain ships as a loosely-coupled service with its own API, persistence, and deployment pipeline, while sharing cross-domain contracts through explicit service boundaries.

## 2. Domain decomposition at a glance

| Domain | Responsibility | Modern primary assets | Database | Deployment unit |
| --- | --- | --- | --- | --- |
| **StoreOps** | Store execution, dispatch, routes, workforce operations, POS staging | `FabrikamPizza.StoreOps.Service` (ASP.NET Core), `FabrikamPizza.StoreOps.Worker` (background jobs), `FabrikamPizza.StoreOps.Data` | `FabrikamPizza_StoreOps` | Single containerized service |
| **CustomerHub** | Corporate accounts, catering, franchise/partner relationships, referral channels | `FabrikamPizza.CustomerHub.Service` (ASP.NET Core), `FabrikamPizza.CustomerHub.Data` | `FabrikamPizza_CustomerHub` | Single containerized service |
| **Commerce** | Public ordering, promotions, sign-in | `FabrikamPizza.Storefront` (ASP.NET Core MVC), modern identity boundary | Reads from StoreOps, anchored auth | Single containerized web app |
| **Reporting** | Nightly rollups, delivery KPIs, labor summaries, partner profitability | `FabrikamPizza.Reporting.Service` (ASP.NET Core), `FabrikamPizza.Reporting.Pipeline` (worker), SSRS/BI integration | `FabrikamPizza_Reporting` | Single containerized service |
| **Shared platform** | Cross-domain contracts, identity, configuration, shared libraries | `FabrikamPizza.Shared` (contracts and DTOs), `FabrikamPizza.Common` (logging, tracing, DI) | n/a (NuGet package) | Versioned package |

## 3. Modern solution structure

### Repository structure

| Path | Purpose |
| --- | --- |
| `docs\foundation\` | Scenario framing, naming, package direction |
| `docs\before\` | Legacy-state topology, boundary rules, and migration reference |
| `docs\after\` | Target-state architecture, migration decisions, and before/after mapping |
| `src\before\` | Legacy .NET Framework solution (preserved for reference and phased cutover) |
| `src\after\` | Modernized ASP.NET Core services, web apps, and workers |
| `data\sqlserver\before\` | Legacy schemas and seed scripts |
| `data\sqlserver\after\` | Modernized schemas with explicit domain boundaries |
| `data\sqlserver\shared\` | Migration helpers, reconciliation queries, and shared test fixtures |

### After-state solution inventory

| Project | Type | Layer | Dependency direction | Architectural role |
| --- | --- | --- | --- | --- |
| `FabrikamPizza.Shared` | NuGet package | Foundation | none | Cross-domain contracts, DTOs, and shared types |
| `FabrikamPizza.Common` | Class library | Foundation | `Shared` | Logging, tracing, DI setup, configuration helpers |
| `FabrikamPizza.Storefront` | ASP.NET Core MVC web app | Presentation | `Shared`, `Common` | Public ordering and customer sign-in |
| `FabrikamPizza.StoreOps.Service` | ASP.NET Core service | Business + API | `Shared`, `Common` | Dispatch, routes, and workforce API |
| `FabrikamPizza.StoreOps.Worker` | Worker service | Background | `Shared`, `Common` | POS sync jobs, route optimization batches |
| `FabrikamPizza.StoreOps.Data` | Class library | Data | `Shared` | SQL Server data access layer for StoreOps |
| `FabrikamPizza.CustomerHub.Service` | ASP.NET Core service | Business + API | `Shared`, `Common` | Partner accounts and catering workflows API |
| `FabrikamPizza.CustomerHub.Data` | Class library | Data | `Shared` | SQL Server data access layer for CustomerHub |
| `FabrikamPizza.Reporting.Service` | ASP.NET Core service | API + analytics | `Shared`, `Common` | KPI and analytics queries |
| `FabrikamPizza.Reporting.Pipeline` | Worker service | Background | `Shared`, `Common` | Nightly ETL and reporting data feeds |
| `FabrikamPizza.Reporting.Data` | Class library | Data | `Shared` | Reporting warehouse schema and queries |
| `FabrikamPizza.Tests` | xUnit library | Validation | `Shared`, per-domain services | Unit and integration tests |

## 4. Runtime shape

```text
Users / franchise staff / store staff
    |
    +-- Storefront (ASP.NET Core MVC) [public web]
    +-- StoreOps Service (ASP.NET Core gRPC/REST) [internal API]
    +-- CustomerHub Service (ASP.NET Core gRPC/REST) [internal API]
    +-- Reporting Service (ASP.NET Core REST) [read-only analytics]
    |
    +-- StoreOps.Worker (background jobs)
    +-- Reporting.Pipeline (nightly ETL)
    |
    +-- FabrikamPizza_StoreOps (SQL Server)
    +-- FabrikamPizza_CustomerHub (SQL Server)
    +-- FabrikamPizza_Reporting (SQL Server)
```

## 5. Domain boundary rules

1. **Services own their data.** StoreOps, CustomerHub, and Reporting each own their respective database schema. No shared transactional tables.
2. **Cross-domain calls go through explicit APIs** (REST, gRPC, or async messaging). No direct database queries across service boundaries.
3. **Reporting stays asynchronous.** Storefront and operational services do not call the Reporting Service for transactional decisions. Reporting feeds into read-only dashboards and bulk exports.
4. **Identity is centralized.** OAuth 2.0 / OpenID Connect at a shared boundary; each service validates tokens locally but does not manage identity state.
5. **Shared contracts** in `FabrikamPizza.Shared` (DTOs, enums, domain primitives). Business logic stays in the owning service.
6. **No omnibus gateway.** Each public interface (Storefront, partner APIs, analytics portal) speaks to its upstream services directly or through a lightweight facade. Avoid BFF sprawl; favor clear domain APIs.

## 6. Before-to-after seam mapping

| Legacy seam | After-state target | Notes |
| --- | --- | --- |
| `Web.Storefront` (MVC 5) + legacy Identity | `Storefront` (Core MVC) + OAuth 2.0 boundary | UI layer stays MVC; identity layer lifted to OIDC boundary |
| `Business.StoreOps` + `Services.DispatchHost` (WCF) + `Desktop.DispatchBoard` (WinForms) | `StoreOps.Service` (Core API) + modern desktop/mobile clients | Business logic centralized in service; desktop replaced by modern UX or retired |
| `Business.CustomerHub` + `Services.PartnerSync` (ASMX) + admin Web Forms | `CustomerHub.Service` (Core API) + modern back-office web | Business logic centralized; XML service retired; Web Forms UI replaced or embedded in back-office app |
| `Integrations.PosSync` | `StoreOps.Worker` | Nightly batch jobs → background worker service |
| `Reporting.Batch` + SSRS assets | `Reporting.Pipeline` + modern BI layer | ETL and pipeline logic → worker service; SSRS sheets → modern reporting platform or dashboards |
| `Data` omnibus gateway | Per-domain data adapters (`StoreOps.Data`, `CustomerHub.Data`, `Reporting.Data`) | Decentralized data access; no shared stored-procedure library |
| `Core`, `Shared.Contracts` | `Shared` (NuGet package) | Shared domain primitives and DTOs versioned and published |

## 7. Technology stack

| Layer | Technology | Rationale |
| --- | --- | --- |
| Web frameworks | ASP.NET Core MVC, ASP.NET Core gRPC, ASP.NET Core minimal APIs | Modern, cross-platform, unified runtime |
| Identity | OAuth 2.0 / OpenID Connect (e.g., Azure AD, IdentityServer) | Standard, industry-proven, removes dependency on ASP.NET Identity |
| Data access | Entity Framework Core | Fully supported, enables per-domain migrations and schema management |
| Background jobs | .NET Worker Service or Azure Functions | Modern alternative to console and scheduled tasks |
| Inter-service communication | gRPC for internal, REST for external APIs | Performance, schema evolution, clear contracts |
| Testing | xUnit, Moq, TestContainers | Modern .NET testing stack; TestContainers for database isolation |
| Deployment | Docker containers orchestrated by Kubernetes or App Service | Cloud-native, repeatable, scalable |
| Logging / tracing | Serilog + OpenTelemetry | Structured logging, correlation IDs, distributed tracing |
| Configuration | .NET configuration providers (JSON files, environment variables, Key Vault) | Centralized, environment-aware, secret-safe |

## 8. Data migration strategy

### Database seams are preserved

- `FabrikamPizza_StoreOps` remains the authority for store execution, labor, and dispatch data.
- `FabrikamPizza_CustomerHub` remains the authority for account, partner, and referral data.
- `FabrikamPizza_Reporting` remains read-only; filled nightly by `Reporting.Pipeline` ETL from upstream databases.

### Schema evolution

1. **Phase 1:** Modernized schema lives alongside legacy schema (side-by-side deployment).
2. **Phase 2:** Application layer switches reads/writes to new schema; legacy data remains for audit/reconciliation.
3. **Phase 3:** Legacy data archived or deleted; modern schema becomes sole source of truth.

### Cross-domain identifiers

Legacy systems often copy foreign keys across databases; the modern approach uses explicit **correlation IDs** and API contracts:

- StoreOps and CustomerHub do not share tables; they share data via APIs that reference entities by ID + domain prefix.
- Example: `store-op-12345` (a StoreOps entity ID) and `customer-hub-67890` (a CustomerHub entity ID) are linked in the Storefront application layer, not the database.

## 9. Modernization roadmap

### Phase A — Foundation and service skeleton (weeks 1–2)

- Set up ASP.NET Core web app and service project templates.
- Define shared contract library (`Shared` NuGet package).
- Configure Serilog, OpenTelemetry, and centralized logging.
- Establish Docker build and push pipeline.
- **Gate:** Services build and deploy; shared contracts versioned and published.

### Phase B — Storefront and identity boundary (weeks 2–4)

- Migrate `Web.Storefront` to ASP.NET Core MVC.
- Integrate OAuth 2.0 / OpenID Connect identity boundary.
- Implement legacy data facade (reading from legacy schema to unblock frontend).
- **Gate:** Storefront runs, signs in users, and reads legacy order data.

### Phase C — StoreOps service and data layer (weeks 3–6)

- Build `StoreOps.Service` with REST/gRPC endpoints.
- Create modernized `FabrikamPizza_StoreOps` schema (EF Core migrations).
- Implement `StoreOps.Data` layer and populate via ETL from legacy database.
- **Gate:** StoreOps API serves reads and writes; legacy batch jobs fully replaced.

### Phase D — CustomerHub service and partnerships (weeks 5–8)

- Build `CustomerHub.Service` with partner and account endpoints.
- Create modernized `FabrikamPizza_CustomerHub` schema.
- Implement `CustomerHub.Data` layer and ETL.
- **Gate:** CustomerHub API live; partner sync jobs replaced.

### Phase E — Reporting and analytics (weeks 6–10)

- Build `Reporting.Service` and `Reporting.Pipeline`.
- Create modernized `FabrikamPizza_Reporting` schema and nightly ETL.
- Integrate with modern BI platform (Power BI, Tableau, or similar).
- **Gate:** Reporting pipelines run nightly; dashboards available.

### Phase F — Cutover and legacy retirement (weeks 9+)

- Dual-write data for final reconciliation.
- Cut over traffic from legacy to modern services.
- Retire legacy WCF/ASMX endpoints, Web Forms, and desktop tooling.
- Archive or delete legacy .NET Framework solution.

## 10. Known trade-offs and risks

| Issue | Modern approach | Trade-off |
| --- | --- | --- |
| Service boundaries not yet hardened by traffic | Explicit API contracts and data ownership | Risk: Early cross-domain calls may look "cheaper" than service calls; discipline required |
| Reporting latency (nightly batches) | Asynchronous ETL keeps transactional databases clean | Trade-off: Analytics 0–24 hours behind; not suitable for real-time operational dashboards |
| Multiple deployments vs. monolith simplicity | Each domain independent; separate deployment pipelines | Risk: Operational overhead; mitigation = CI/CD automation, Infrastructure as Code |
| OAuth 2.0 token overhead | Standard identity boundary with cached token validation | Trade-off: Per-request token validation overhead vs. per-session legacy cookies |

## 11. Reviewer guidance for after-state work

Reject downstream work that:

- Adds synchronous calls between domain services outside of the contracted API boundary.
- Violates the three-database ownership model by adding cross-database foreign keys or shared tables.
- Implements new business features in the legacy solution instead of the modern service.
- Skips OpenTelemetry tracing, making distributed debugging impossible.
- Adds dependencies between `Storefront` and `CustomerHub.Service` outside of the defined REST/gRPC contract.

If service boundaries start drifting, stop work and reset the seam. A slower start with clear boundaries beats a fast sprint into entanglement.
