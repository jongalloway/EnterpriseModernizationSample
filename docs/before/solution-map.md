# Legacy Solution Map

The legacy solution is intentionally broad enough to demo modernization seams across UI, services, data access, tests, and batch processing.

| Project | Type | Purpose |
| --- | --- | --- |
| `Fabrikam.EnterprisePizza.Core` | Class library | Shared domain models and low-level business primitives |
| `Fabrikam.EnterprisePizza.Shared.Contracts` | Class library | DTOs used by services and desktop tooling |
| `Fabrikam.EnterprisePizza.Business.StoreOps` | Class library | Store dispatch, labor, and route orchestration |
| `Fabrikam.EnterprisePizza.Business.CustomerHub` | Class library | Corporate accounts and partner workflow logic |
| `Fabrikam.EnterprisePizza.Data` | Class library | Stored-procedure wrappers and Enterprise Library-era data access seams |
| `Fabrikam.EnterprisePizza.Integrations.PosSync` | Class library | Nightly POS and partner synchronization jobs |
| `Fabrikam.EnterprisePizza.Services.DispatchHost` | ASP.NET / WCF | Store dispatch SOAP endpoint |
| `Fabrikam.EnterprisePizza.Services.PartnerSync` | ASP.NET / ASMX | Partner-facing legacy XML service |
| `Fabrikam.EnterprisePizza.Portal` | ASP.NET Web Forms | Main partner portal shell that links the older franchise, customer, and storefront surfaces |
| `Fabrikam.EnterprisePizza.Web.Storefront` | ASP.NET MVC 5 | Online ordering and customer sign-in surface |
| `Fabrikam.EnterprisePizza.Web.FranchisePortal` | ASP.NET Web Forms | Franchise notices, campaign editing, and bulletin tools |
| `Fabrikam.EnterprisePizza.Web.CustomerHub` | ASP.NET Web Forms | Corporate account lookup and catering workflows |
| `Fabrikam.EnterprisePizza.Desktop.DispatchBoard` | Windows Forms | In-store dispatch and driver board with a third-party docking shell |
| `Fabrikam.EnterprisePizza.Reporting.Batch` | Console | Nightly exports and reporting feed generation |
| `Fabrikam.EnterprisePizza.Legacy.Tests` | NUnit test library | Sparse in-solution regression checks |

## Intentional modernization pain points

1. Multiple web stacks live side by side.
2. WCF and ASMX still expose XML/SOAP contracts.
3. Package management remains `packages.config`.
4. Storefront auth is pinned to older ASP.NET Identity packages.
5. Web Forms depends on both Microsoft and third-party legacy controls.
