# Implementation Patterns

**Document date:** 2026-05-14T15:35:48.472+02:00

This document defines the repeatable patterns and code conventions that ensure consistency across both the legacy (before) and modernized (after) solution. Use these patterns to ensure new work remains understandable and maintenance-friendly.

## Before-state patterns (legacy)

### 1. Enterprise Library data access

Legacy data access uses Enterprise Library 6.0 and the stored-procedure gateway pattern.

**Pattern location:** `src\before\Fabrikam.EnterprisePizza.Data`

**Key characteristics:**
- Stored procedures are the primary data interface; rarely any direct SQL.
- `DataSet`-based result handling; typed adapters map rows to objects.
- Connection strings centralized in configuration.
- Database transactions managed at the procedure boundary.

**Example — store lookup:**

```csharp
public class StoreDataGateway
{
    // Enterprise Library-based gateway
    public DataSet GetStoresByRegion(string region)
    {
        Database db = DatabaseFactory.CreateDatabase("FabrikamPizza_StoreOps");
        DbCommand cmd = db.GetStoredProcCommand("sp_GetStoresByRegion");
        db.AddInParameter(cmd, "@Region", DbType.String, region);
        return db.ExecuteDataSet(cmd);
    }
}
```

**When to use:** Implementing new stored-procedure-backed queries in the legacy solution during the reference phase.

**Do not:** Add new DbContext-style async ORM code to the legacy solution. Keep legacy data access synchronous and SP-driven.

### 2. WCF SOAP service endpoints

Legacy services expose WCF service contracts with SOAP/XML transport.

**Pattern location:** `src\before\Fabrikam.EnterprisePizza.Services.DispatchHost`, `Services.PartnerSync`

**Key characteristics:**
- Service contracts defined in `Shared.Contracts` (DTO classes with DataContract attributes).
- Endpoints configured in `web.config` or `app.config`.
- Security via certificate or Windows auth.
- No async methods; all I/O is synchronous.

**Example — dispatch service contract:**

```csharp
[ServiceContract(Namespace = "http://fabrikam.com/2015/05/dispatch")]
public interface IDispatchService
{
    [OperationContract]
    DispatchAssignmentDto GetAssignments(int storeId);
    
    [OperationContract]
    void UpdateRouteStatus(int routeId, string status);
}
```

**When to use:** Maintaining or extending WCF endpoints during the legacy phase.

**Do not:** Add WCF to the modernized solution. Target gRPC or REST instead.

### 3. Web Forms master pages and user controls

Web Forms UI uses master pages for layout and reusable user controls.

**Pattern location:** `src\before\Fabrikam.EnterprisePizza.Web.FranchisePortal`, `Web.CustomerHub`

**Key characteristics:**
- AJAX Control Toolkit for client-side interactivity.
- Server-side event handlers (`Page_Load`, button click events).
- ViewState for state management.
- Code-behind files contain both markup logic and server code.

**Example — franchise notice list:**

```aspx
<%@ Page Language="C#" MasterPageFile="~/Site.Master" CodeBehind="NoticeList.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Web.FranchisePortal.NoticeList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ListView ID="NoticeListView" runat="server">
        <!-- Item template -->
    </asp:ListView>
</asp:Content>
```

**When to use:** Filling out legacy Web Forms UI during the reference phase.

**Do not:** Add Web Forms pages to the modernized solution. Target ASP.NET Core MVC or Razor Pages instead.

### 4. NUnit test structure

Legacy tests use NUnit 3.12 with `TestFixture` and `Test` attributes.

**Pattern location:** `src\before\Fabrikam.EnterprisePizza.Legacy.Tests`, `Tests.Unit`

**Key characteristics:**
- Arrange-Act-Assert pattern.
- Synchronous test methods; no async/await.
- Fluent assertions for readability.
- In-memory stubs for data dependencies.

**Example — store operations test:**

```csharp
[TestFixture]
public class DispatchOrchestrationTests
{
    private Mock<IStoreRepository> _mockRepo;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IStoreRepository>();
    }

    [Test]
    public void GetAssignments_WithValidStoreId_ReturnsAssignments()
    {
        // Arrange
        var storeId = 12;
        _mockRepo.Setup(x => x.GetStore(storeId)).Returns(new Store { Id = storeId });

        // Act
        var assignments = new DispatchOrchestration(_mockRepo.Object).GetAssignments(storeId);

        // Assert
        Assert.That(assignments, Is.Not.Empty);
    }
}
```

**When to use:** Adding sparse regression tests to legacy code.

**Do not:** Expect exhaustive test coverage on legacy code. Focus on critical seams and refactoring protection.

---

## After-state patterns (modernized)

### 1. ASP.NET Core service architecture

Modernized services use ASP.NET Core with dependency injection and repository patterns.

**Planned pattern location:** `src\after\FabrikamPizza.StoreOps.Service`, `src\after\FabrikamPizza.CustomerHub.Service`

**Key characteristics:**
- Dependency injection via `IServiceProvider` (built-in to Core).
- Entity Framework Core for ORM and migrations.
- Async/await throughout; all I/O is non-blocking.
- Minimal APIs or controller-based REST endpoints.
- OpenTelemetry for distributed tracing.

**Example — StoreOps service with EF Core:**

```csharp
public class StoreOpsService
{
    private readonly StoreOpsContext _context;
    private readonly ILogger<StoreOpsService> _logger;

    public StoreOpsService(StoreOpsContext context, ILogger<StoreOpsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Store>> GetStoresByRegionAsync(string region)
    {
        _logger.LogInformation("Fetching stores for region: {Region}", region);
        return await _context.Stores
            .Where(s => s.Region == region)
            .ToListAsync();
    }
}
```

**When to use:** Building new business logic in the modernized services.

**Do not:** Use `DataSet`s or synchronous database I/O. All queries must be async and LINQ-to-SQL.

### 2. REST API conventions

Modernized APIs follow RESTful conventions with consistent error handling and pagination.

**Planned pattern location:** `src\after\FabrikamPizza.StoreOps.Service\Controllers`, `src\after\FabrikamPizza.CustomerHub.Service\Controllers`

**Key characteristics:**
- HTTP verbs (GET, POST, PUT, DELETE) map to CRUD operations.
- Resource paths follow `/api/{resource}/{id}` within each domain service (for example, `/api/stores/{id}` inside StoreOps).
- Standardized error responses with `ProblemDetails`.
- Pagination via `?page=1&pageSize=20` query parameters.

**Example — StoreOps API controller:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class StoresController : ControllerBase
{
    private readonly StoreOpsService _service;

    public StoresController(StoreOpsService service) => _service = service;

    [HttpGet("{id}")]
    public async Task<ActionResult<StoreDto>> GetStoreAsync(int id)
    {
        var store = await _service.GetStoreAsync(id);
        if (store == null)
            return NotFound();
        return Ok(store);
    }

    [HttpPost]
    public async Task<ActionResult<StoreDto>> CreateStoreAsync(CreateStoreDto dto)
    {
        var store = await _service.CreateStoreAsync(dto);
        return CreatedAtAction(nameof(GetStoreAsync), new { id = store.Id }, store);
    }
}
```

**When to use:** Exposing domain operations via REST.

**Do not:** Return `DataSet` or `DataTable` from API endpoints. Use DTOs.

### 3. Entity Framework Core data access

All database queries go through EF Core `DbContext` with async methods and migrations.

**Planned pattern location:** `src\after\FabrikamPizza.StoreOps.Data`, `src\after\FabrikamPizza.CustomerHub.Data`

**Key characteristics:**
- One `DbContext` per domain (e.g., `StoreOpsContext`, `CustomerHubContext`).
- Schema migration using EF Core migrations.
- Explicit navigation properties for relationships.
- LINQ queries; no raw SQL unless unavoidable.

**Example — StoreOps context:**

```csharp
public class StoreOpsContext : DbContext
{
    public StoreOpsContext(DbContextOptions<StoreOpsContext> options) : base(options) { }

    public DbSet<Store> Stores { get; set; }
    public DbSet<DispatchRoute> Routes { get; set; }
    public DbSet<Driver> Drivers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Store>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasMany(x => x.Routes).WithOne(r => r.Store);
        });
    }
}
```

**When to use:** All database I/O in modernized services.

**Do not:** Use multiple DbContexts in a single query path. Keep data ownership explicit and per-domain.

### 4. Shared contracts library (NuGet package)

Cross-domain DTOs and enums live in a versioned NuGet package.

**Planned pattern location:** `src\after\FabrikamPizza.Shared`

**Key characteristics:**
- Plain classes with auto-properties; no business logic.
- Serializable (JSON, gRPC); no circular references.
- Versioned via `<Version>` in `.csproj`.
- Published to internal NuGet feed.

**Example — shared DTOs:**

```csharp
namespace FabrikamPizza.Shared;

public class StoreDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Region { get; set; }
}

public class DispatchAssignmentDto
{
    public int RouteId { get; set; }
    public int DriverId { get; set; }
    public List<int> StoreIds { get; set; }
}
```

**When to use:** Defining cross-domain contract types.

**Do not:** Add business logic, dependencies, or domain-specific repositories to the Shared package.

### 5. Worker service for background jobs

Nightly batch jobs run as containerized .NET Worker Services that execute once per invocation; Kubernetes CronJobs own the schedule.

**Planned pattern location:** `src\after\FabrikamPizza.StoreOps.Worker`, `src\after\FabrikamPizza.Reporting.Pipeline`

**Key characteristics:**
- Host services inherit `BackgroundService` or equivalent hosted-service entry points.
- Packaged as container images and scheduled by Kubernetes CronJob/Job resources, not by in-process `Task.Delay` loops.
- One run performs one ETL/sync pass, then exits cleanly so the orchestrator can track success/failure.
- Logging via Serilog.

**Example — POS sync worker entry point (CronJob-invoked):**

```csharp
public class PosSyncWorker : BackgroundService
{
    private readonly ILogger<PosSyncWorker> _logger;
    private readonly IStoreOpsService _storeOps;

    public PosSyncWorker(ILogger<PosSyncWorker> logger, IStoreOpsService storeOps)
    {
        _logger = logger;
        _storeOps = storeOps;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting nightly POS sync");
            await _storeOps.SyncPosDataAsync(stoppingToken);
            _logger.LogInformation("POS sync completed");
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "POS sync failed");
            throw;
        }
    }
}
```

**When to use:** Running scheduled jobs and background ETL in the modernized solution.

**Do not:** Use Windows Task Scheduler, SQL Agent jobs, or self-scheduling `Task.Delay(TimeSpan.FromHours(24))` loops. Keep jobs containerized and let the orchestrator own the schedule.

### 6. xUnit test structure

Modernized tests use xUnit with Moq and TestContainers.

**Planned pattern location:** `src\after\FabrikamPizza.Tests`

**Key characteristics:**
- Fact and Theory attributes.
- Async test methods with async Task return.
- TestContainers for database isolation per test.
- AAA pattern (Arrange-Act-Assert).

**Example — StoreOps service test:**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.PostgreSql;
using Xunit;

public sealed class StoreOpsServiceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .Build();

    private StoreOpsContext? _context;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<StoreOpsContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _context = new StoreOpsContext(options);
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (_context is not null)
        {
            await _context.DisposeAsync();
        }

        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task GetStoresByRegion_WithValidRegion_ReturnsStores()
    {
        var context = Assert.IsType<StoreOpsContext>(_context);
        var service = new StoreOpsService(context, NullLogger<StoreOpsService>.Instance);
        context.Stores.Add(new Store { Id = 1, Name = "Downtown", Region = "East" });
        await context.SaveChangesAsync();

        var stores = await service.GetStoresByRegionAsync("East");

        Assert.Single(stores);
        Assert.Equal("Downtown", stores[0].Name);
    }
}
```

Add the `Testcontainers.PostgreSql` package so contributors get `PostgreSqlBuilder` and `PostgreSqlContainer` from the supported .NET API surface.

**When to use:** Testing modernized services with real database isolation.

**Do not:** Mock the database in integration tests. Use TestContainers to spin up real PostgreSQL or SQL Server instances.

### 7. OpenTelemetry tracing

All services emit structured logs and distributed traces via OpenTelemetry.

**Planned pattern location:** All modernized services; configured in `Program.cs`

**Key characteristics:**
- Automatic instrumentation for HTTP, database, and message queues.
- Correlation IDs propagated across service boundaries.
- Exporters to Jaeger, Application Insights, or Datadog.
- Activity names follow OpenTelemetry conventions.

**Example — tracing setup in Program.cs:**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracingBuilder =>
    {
        tracingBuilder
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddConsoleExporter();
    });

builder.Services.AddSerilog((context, config) =>
{
    config
        .WriteTo.Console(new JsonFormatter())
        .Enrich.WithCorrelationId();
});
```

**When to use:** Instrumenting all service endpoints and data access.

**Do not:** Log raw exceptions without context. Always include correlation IDs and structured fields.

---

## Shared principles across before and after

### 1. Boundary enforcement

- **Legacy:** Stored procedures and SOAP contracts create hard boundaries.
- **Modern:** REST/gRPC contracts and `DbContext` ownership create hard boundaries.
- **Pattern:** Never bypass the boundary. If querying another domain's data, go through its API.

### 2. Explicit dependencies

- **Legacy:** Enterprise Library configuration centralizes data source info.
- **Modern:** Dependency injection centralizes service registration.
- **Pattern:** Avoid static singletons; wire dependencies through constructors or DI containers.

### 3. Error handling

- **Legacy:** Exceptions bubble up to service boundaries; SOAP faults carry error info.
- **Modern:** Exceptions caught at controller boundary; converted to `ProblemDetails` responses.
- **Pattern:** Do not let database errors leak into API responses; wrap in domain-specific exceptions first.

### 4. Testing discipline

- **Legacy:** Mock stored-procedure calls; test gateway code in isolation.
- **Modern:** Use TestContainers for realistic database testing; mock only external dependencies.
- **Pattern:** Prioritize end-to-end seam tests over isolated unit tests for data and API boundaries.

### 5. Operational observability

- **Legacy:** Event Viewer and SQL Server logs; limited correlation.
- **Modern:** Structured logs and distributed traces; correlation IDs across services.
- **Pattern:** Every request and job should carry a correlation ID from entry to database.

---

## Decision points for implementers

| Question | Legacy answer | Modern answer | Enforcement point |
| --- | --- | --- | --- |
| How do I query data? | Stored procedure → Enterprise Library gateway | LINQ → EF Core DbContext | Code review: reject direct DB I/O outside DbContext |
| How do I call another domain? | SOAP service contract | REST API or gRPC | Code review: reject cross-domain DbContext usage |
| How do I handle errors? | Exception → SOAP fault | Exception → 400/500 ProblemDetails | Code review: ensure all exceptions caught at controller boundary |
| How do I test this? | Mock gateways; in-memory stubs | TestContainers; real database per test | Code review: reject tests that mock the database |
| How do I trace a user request? | Event ID in event log | Correlation ID in structured logs and trace spans | Code review: ensure all log statements include correlation ID |

These patterns ensure that **legacy work stays contained within its bounds** while **modern work follows cloud-native conventions**. Both states maintain clear boundaries; neither should leak operational details across domain seams.
