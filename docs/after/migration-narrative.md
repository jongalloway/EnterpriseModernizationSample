# Before/After Migration Narrative

**Document date:** 2026-05-14T15:35:48.472+02:00

This document tells the story of Fabrikam Enterprise Pizza's migration from a sprawling 2005-era .NET Framework legacy estate to a modern, decomposed, cloud-ready microservices architecture. It explains why each seam shifts, what stays the same, and how to think about progress as the sample evolves.

## The legacy story

### What legacy built

In 2005, Fabrikam Enterprise Pizza began as a simple POS and franchise accounting system. Over the next 15 years:

1. **A web storefront was bolted on** (ASP.NET MVC 5, circa 2014) to allow online ordering without redesigning the core dispatch and accounting layers.
2. **A franchise portal expanded** (Web Forms, circa 2010) so franchisees could manage store notices, edit menus, and request supplies.
3. **Corporate catering grew** (Web Forms, circa 2012) with its own account management and contract pricing.
4. **Partner sync services appeared** (ASMX, WCF) to allow third-party data providers (POS, delivery networks, payroll vendors) to push data into a central schema.
5. **Dispatch routing became sophisticated** (WinForms desktop app, WCF service layer) as store density increased and delivery times became competitive.
6. **Reporting infrastructure solidified** (SSRS, nightly batch jobs) because finance needed reconciliation and marketing needed KPIs.

By 2025, the system worked—but showed its age:

- Three SQL Server databases held operational, customer/partner, and reporting data separately (a good decision that accidentally became hard to bridge).
- Web Forms, MVC, WCF, and ASMX technologies coexisted without shared patterns; the team needed different domain knowledge for each surface.
- Business logic sprawled across multiple layers: stored procedures, Enterprise Library data gateways, business-logic class libraries, and service DTOs all carried different responsibilities.
- Tests were sparse; regression coverage was hard because dependencies were global (static data access patterns, configuration files, hard-coded connection strings).
- Deployment meant a full solution rebuild, even if only one feature changed.

### Why the legacy works (and should not be discarded)

The legacy system is **architecturally sound** where it matters:

1. **Database seams are explicit.** Reporting stays asynchronous; POS staging data does not pollute transactional tables.
2. **Service contracts are documented.** SOAP contracts and WCF endpoints define clear boundaries.
3. **Business domains are separated.** StoreOps, CustomerHub, and Reporting responsibilities do not blur.
4. **Operational data is consistent.** Nightly batch jobs ensure reporting feeds match transactional reality.

The problem is **technology sprawl**, not domain design. Migrations often fail because teams confuse "replace the monolith with microservices" with "rewrite from scratch." This sample does not make that mistake. The legacy architecture is fundamentally sound; the modernization preserves domain seams while replacing technology.

---

## The migration path

### Phase 1: Foundation and identity boundary

**Legacy state:** Identity is pinned to ASP.NET Identity 2.2.3 (2015-era); the storefront and portal manage users with database-backed claims and roles.

**Modern state:** OAuth 2.0 / OpenID Connect replaces application-level identity; storefront and all services validate tokens at the boundary.

**Why:** OAuth 2.0 is industry standard. Removing identity from application code simplifies services, enables federated sign-in, and allows other systems to integrate without custom authentication logic.

**What stays the same:** User data lives in the same SQL Server database(s); the schema changes only minimally (identity claims move to a separate identity provider, but user profiles stay in `FabrikamPizza_CustomerHub`).

**What changes:** Services do not call `UserManager` or `RoleManager` APIs. Instead, they validate JWT tokens and extract claims. The legacy storefront gains an OAuth 2.0 sign-in endpoint, which redirects to an external identity provider (Azure AD, IdentityServer, or similar).

### Phase 2: Storefront modernization

**Legacy state:** ASP.NET MVC 5 app reading from legacy data access layer; calls to `StoreRepository`, `MenuRepository` etc. which hit stored procedures.

**Modern state:** ASP.NET Core MVC app with direct calls to StoreOps and CustomerHub REST/gRPC services; no direct database access.

**Why:** Decoupling the UI from the database makes both independently deployable. The storefront can ship bug fixes without redeploy of StoreOps services.

**What stays the same:** View models, controller names, route patterns, CSS/JavaScript. The user sees no visual change (until modernization brings UX improvements).

**What changes:** 
- Service layer goes from `StoreRepository` (direct DB gateway) to `IStoreOpsClient` (HTTP client).
- Data fetching becomes async throughout (`GetStoresAsync()` instead of `GetStores()`).
- Error handling shifts from SOAP faults to REST error responses.

**Bridge pattern (dual-write during cutover):**
```
Request → Storefront UI
  ├→ Call modern StoreOps service
  └→ If service unavailable, fall back to legacy database gateway
Response → UI (served, regardless of which path was taken)
```

Once StoreOps service is stable, remove the fallback.

### Phase 3: StoreOps service decomposition

**Legacy state:** `Business.StoreOps` class library holds business logic; `Services.DispatchHost` (WCF) exposes a SOAP contract; `Desktop.DispatchBoard` (WinForms) calls the SOAP service.

**Modern state:** `FabrikamPizza.StoreOps.Service` (ASP.NET Core) holds business logic and exposes REST/gRPC endpoints; desktop clients are replaced by web-based or mobile UIs.

**Why:** 
- ASP.NET Core services are cloud-native (containerizable, horizontally scalable).
- REST and gRPC are simpler than WCF/SOAP and widely understood.
- Stateless services enable zero-downtime deployments.

**What stays the same:**
- Database schema (gradually migrated; legacy schema coexists during transition).
- Business logic (mostly ported 1:1; no algorithmic changes).
- Three-database split; StoreOps owns `FabrikamPizza_StoreOps`.

**What changes:**
- WCF SOAP endpoints → REST endpoints (`GET /api/routes/{id}`, `POST /api/assignments`).
- WinForms desktop app → Replaced by modern web dashboard or retired if no longer needed.
- Synchronous business logic → Async/await throughout.
- Configuration in `app.config` → Centralized configuration in `appsettings.json` or Key Vault.

**Database migration strategy:**
1. **Week 1–2:** Create new EF Core migrations in parallel; populate via ETL from legacy schema.
2. **Week 2–3:** Services read from new schema; write still goes to legacy (dual-write for verification).
3. **Week 3+:** Switch writes to new schema; legacy data archived.

### Phase 4: CustomerHub service decomposition

**Legacy state:** `Business.CustomerHub` logic + Web Forms admin UI + ASMX partner sync service.

**Modern state:** `FabrikamPizza.CustomerHub.Service` (ASP.NET Core) exposes REST for partners; web-based admin replaces Web Forms.

**Why:** Same rationale as StoreOps. Service-oriented architecture enables third-party partners to integrate via standard REST rather than learning ASMX quirks.

**What stays the same:**
- Database schema (migrated via EF Core).
- Partner data ownership; no cross-service foreign keys.
- Nightly synchronization patterns.

**What changes:**
- ASMX XML service → REST API with OpenAPI documentation.
- Web Forms admin UI → ASP.NET Core MVC or Blazor web app.
- Legacy data access → EF Core.

### Phase 5: Reporting and worker services

**Legacy state:** `Reporting.Batch` (console app run via scheduled task) pulls data from StoreOps and CustomerHub, populates SSRS tables in `FabrikamPizza_Reporting`.

**Modern state:** `FabrikamPizza.Reporting.Pipeline` (Worker Service) runs nightly in Kubernetes; exposes `FabrikamPizza.Reporting.Service` REST API for analytics queries.

**Why:** Worker Services are cloud-ready, can be containerized, and integrate cleanly with orchestration platforms (Kubernetes, App Service).

**What stays the same:**
- Three-database split; Reporting owns `FabrikamPizza_Reporting`.
- Nightly ETL cadence; reporting lags transactional data by 0–24 hours (acceptable for business KPIs).
- SSRS/Power BI downstream; no breaking changes to report definitions.

**What changes:**
- Console app + Windows Task Scheduler → Worker Service + Kubernetes CronJob.
- Direct database queries via `SqlConnection` + `SqlCommand` → EF Core DbContext.
- Configuration in `app.config` → Environment variables + Key Vault.

### Phase 6: Data ownership and API contracts

**Legacy state:** Cross-database foreign keys and shared stored procedures blur ownership. Storefront can query any database; business layers depend on specific gateways.

**Modern state:** Each service owns its database. Cross-domain queries go through explicit APIs (REST, gRPC, or async messaging).

**Why:** This prevents accidental dependencies and makes service boundaries easier to enforce during future refactoring.

**What stays the same:**
- Three-database split is preserved.
- No shared transactional tables across services.

**What changes:**
- `SELECT * FROM FabrikamPizza_CustomerHub.dbo.Customers` directly from StoreOps → Call `CustomerHub.Service/api/customers/{id}` instead.
- Synchronous stored-procedure calls → Async service calls (retry logic, circuit breakers added automatically).

---

## The business timeline

| Period | Focus | Business value | Technical milestones |
| --- | --- | --- | --- |
| **Months 1–3** | Foundation + Storefront | Zero customer impact; team gains velocity with modern tooling | OAuth 2.0 boundary, Core MVC storefront, CI/CD automation |
| **Months 3–5** | StoreOps decomposition | Dispatch routing, workforce scheduling faster to update | StoreOps Service live; WCF retired |
| **Months 5–7** | CustomerHub + Partners | Partner integrations simpler; faster catering contract changes | CustomerHub Service live; ASMX retired; partner docs in OpenAPI |
| **Months 7–9** | Reporting + Workers | KPI dashboards more responsive; fewer manual reconciliations | Reporting pipeline in Kubernetes; SSRS migrated or replaced |
| **Months 9+** | Legacy retirement | Full cloud deployment; zero tech debt; automatic scaling | Legacy .NET Framework solution archived; all services containerized |

---

## Mapping legacy concepts to modern equivalents

| Legacy concept | Modern equivalent | Why it changed |
| --- | --- | --- |
| `StoredProcedureGateway` class | EF Core `DbContext` + LINQ queries | ORM handles query optimization and schema evolution; no hand-written SQL |
| `ServiceContract` (WCF SOAP) | `ApiController` (REST) or gRPC service | REST is simpler; OpenAPI auto-documentation; gRPC for internal service-to-service |
| Enterprise Library `Database` API | `IDbConnection` / EF Core | Dependency injection replaces static factory methods; connection pooling handled by runtime |
| `DataSet` result handling | DTOs (plain C# classes) | Type-safe, JSON-serializable, testable without data-layer coupling |
| ASP.NET Identity (table-driven) | OAuth 2.0 / OpenID Connect | Federated, industry-standard, separates identity from application code |
| Web Forms server controls + ViewState | HTML + JavaScript / Razor Pages | Lighter payloads, browser standards, easier testing |
| Console app run by Task Scheduler | Worker Service + Kubernetes CronJob | Cloud-ready, self-healing, no machine dependency |
| SSRS reports (server-deployed) | Power BI / Tableau + REST API | Cloud-hosted, self-service, easier sharing with external partners |
| NUnit test (2.x-era) | xUnit + TestContainers | Async support, better test isolation, real database per test |

---

## How to measure progress

### Early wins (Months 1–3)
- [ ] OAuth 2.0 identity boundary in place; legacy storefront can sign in without `UserManager` calls.
- [ ] Storefront runs on ASP.NET Core; build time under 60 seconds.
- [ ] CI/CD pipeline auto-deploys storefront on every commit.
- [ ] First service (e.g., StoreOps.Service) deployed to container registry and running in dev environment.

### Midpoint (Months 4–6)
- [ ] Two services live (StoreOps, CustomerHub); legacy service layers (WCF, ASMX) coexist but not called by new code.
- [ ] 80% of business logic ported to modern services.
- [ ] No new features added to legacy .NET Framework solution; all feature work goes to modern side.
- [ ] Reporting pipeline runs nightly on schedule; dashboards available 24 hours behind reality.

### Endgame (Months 7–9+)
- [ ] All three services deployed and load-balanced.
- [ ] Legacy services retired; no incoming calls.
- [ ] All tests run in xUnit with TestContainers; no legacy NUnit regression tests remain in active rotation.
- [ ] Every service automatically scales up during peak hours; gracefully scales down off-hours.
- [ ] Legacy .NET Framework solution archived; no new commits.

---

## Risks and mitigations

| Risk | Consequence | Mitigation |
| --- | --- | --- |
| Service boundaries blur during implementation | Services become entangled; future refactoring impossible | **Gate:** Code review rejects cross-service direct DB queries. Architecture review before adding new service dependencies. |
| Reporting falls behind; becomes stale | Business teams report inaccurate KPIs | **Design:** Reporting is async batch-fed; set expectations that data lags by 24h. Provide live dashboards for operational metrics only. |
| Migration takes longer than planned | Business loses confidence; pressure to "just keep the old system" | **Plan:** Release early and often. Celebrate Phase 1 completion (OAuth boundary) even if backend work is incomplete. |
| Team unfamiliar with modern tooling (ASP.NET Core, Docker, Kubernetes) | Slow velocity; mistakes during deployment | **Invest:** Pair legacy team members with modern tech leads. Automate deployment with scripts and CI/CD. |
| Data consistency issues during dual-write period | Stale reads; confusion about source of truth | **Validate:** Write reconciliation queries that compare legacy and modern schemas nightly. Alert if drift exceeds threshold. |

---

## Decision points that define success

### Decision 1: Do not rewrite business logic.
**Right:** Port logic from legacy class libraries to modern services; run tests against both to verify equivalence.
**Wrong:** Rewrite from spec; assume legacy business logic is "wrong" and needs fixing.

### Decision 2: Preserve database seams; do not merge.
**Right:** Keep three databases; add cross-service APIs if data needs flow across boundaries.
**Wrong:** Collapse `FabrikamPizza_CustomerHub` and `FabrikamPizza_Reporting` into one database "for simplicity."

### Decision 3: Retire old technology; do not refactor it.
**Right:** ASMX service is done; move partners to REST. Do not add new ASMX features.
**Wrong:** "We'll refactor ASMX to modern WCF later." (Later never comes.)

### Decision 4: Measure success by operational velocity, not lines of code.
**Right:** Storefront ship time drops from 4 hours to 20 minutes after modernization.
**Wrong:** "We rewrote 50,000 lines of code." (That is not a win; it is a cost.)

---

## The after-state vision

Once migration completes:

1. **Developers can ship features independently.** Storefront changes do not require redeploying StoreOps. Partners can integrate via documented REST APIs without understanding internal architecture.

2. **Operations can scale on demand.** During peak holiday seasons, Kubernetes spins up extra StoreOps and Storefront replicas; Reporting pipeline still runs nightly on a single node (it does not need scaling).

3. **New business requirements fit cleanly.** A new "franchise financial reporting" requirement becomes a new service; it does not require adding 200 lines of stored-procedure SQL to the existing Reporting database.

4. **Developers onboard faster.** One modern ASP.NET Core service is easier to understand than legacy WCF + ASMX + Web Forms all at once.

5. **Incidents are faster to diagnose.** OpenTelemetry traces follow a user request through Storefront → StoreOps → Database. Correlation IDs make it obvious where the latency is.

This sample exists to show that migration is possible, seams matter, and technology choices follow architecture—not the reverse.
