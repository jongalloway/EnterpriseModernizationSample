# Legacy Source

This folder holds the deliberately overgrown **before** solution for **Fabrikam Enterprise Pizza**.

## Planned legacy solution

- Canonical entry point: `Fabrikam.EnterprisePizza.Legacy.sln`
- `Fabrikam.EnterprisePizza.Core`
- `Fabrikam.EnterprisePizza.Shared.Contracts`
- `Fabrikam.EnterprisePizza.Business.StoreOps`
- `Fabrikam.EnterprisePizza.Business.CustomerHub`
- `Fabrikam.EnterprisePizza.Data`
- `Fabrikam.EnterprisePizza.Integrations.PosSync`
- `Fabrikam.EnterprisePizza.Services.DispatchHost` (WCF)
- `Fabrikam.EnterprisePizza.Services.PartnerSync` (ASMX)
- `Fabrikam.EnterprisePizza.Portal` (ASP.NET Web Forms main partner portal)
- `Fabrikam.EnterprisePizza.StoreOps.Portal` (ASP.NET Web Forms dispatch and labor intranet shell)
- `Fabrikam.EnterprisePizza.Web.Storefront` (ASP.NET MVC 5)
- `Fabrikam.EnterprisePizza.Web.FranchisePortal` (ASP.NET Web Forms + AJAX Control Toolkit + FreeTextBox)
- `Fabrikam.EnterprisePizza.Web.CustomerHub` (ASP.NET Web Forms)
- `Fabrikam.EnterprisePizza.Desktop.DispatchBoard` (Windows Forms + DockPanelSuite)
- `Fabrikam.EnterprisePizza.Reporting.Batch`
- `Fabrikam.EnterprisePizza.Legacy.Tests` (NUnit)
- `Fabrikam.EnterprisePizza.Tests.Unit` (NUnit)

The scaffold intentionally mixes patterns and eras so the modernization story has visible seams: Web Forms and MVC side by side, WCF and ASMX still active, packages.config usage, Unity/CommonServiceLocator composition in `Fabrikam.EnterprisePizza.Core`, Enterprise Library-era data access, and older ASP.NET Identity packages in the MVC storefront.

## Smoke coverage notes

- `Fabrikam.EnterprisePizza.Legacy.Tests` keeps the WCF `StoreDispatchService.svc` host file, metadata endpoint wiring, and sample dispatch payload observable.
- The same test project checks the ASMX `PartnerSync.asmx` directive, basic-profile attributes, and sample partner responses so reviewers can catch broken endpoint shells before deeper modernization work starts.

## Running the NUnit backlog locally

Restore the legacy packages first so the NUnit adapter lands in `src\before\packages`:

```powershell
nuget restore src\before\Fabrikam.EnterprisePizza.Legacy.sln
dotnet msbuild src\before\Fabrikam.EnterprisePizza.Tests.Unit\Fabrikam.EnterprisePizza.Tests.Unit.csproj /t:Build /p:Configuration=Debug
powershell -ExecutionPolicy Bypass -File .\scripts\Invoke-LegacyNUnit.ps1 -AssemblyPath .\src\before\Fabrikam.EnterprisePizza.Tests.Unit\bin\Debug\Fabrikam.EnterprisePizza.Tests.Unit.dll
```
