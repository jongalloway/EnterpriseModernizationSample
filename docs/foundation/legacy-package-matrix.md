# Legacy Package Matrix

This matrix locks the intentionally uneven package profile for the **before** solution.

| Package | Version | Legacy use |
| --- | --- | --- |
| EnterpriseLibrary.Data | 6.0.1304 | Data access helpers and stored procedure wrappers in `Fabrikam.EnterprisePizza.Data` |
| EnterpriseLibrary.Common | 6.0.1304 | Supporting Enterprise Library abstractions |
| Autofac | 4.9.2 | Ad hoc DI in customer and storefront components |
| Microsoft.AspNet.Mvc | 5.2.3 | MVC storefront |
| Microsoft.AspNet.Identity.Core | 2.2.3 | Legacy storefront auth surface |
| Microsoft.AspNet.Identity.EntityFramework | 2.2.3 | Identity-backed membership store |
| EntityFramework | 6.1.3 | Identity and selected repository classes |
| AjaxControlToolkit | 15.1.4 | Web Forms extenders, accordions, and modal behaviors |
| FreeTextBox | 3.3.1 | Third-party rich text editing on Web Forms admin pages |
| DockPanelSuite | 3.1.1 | Non-Microsoft docking shell for the WinForms dispatch desktop |
| DockPanelSuite.ThemeVS2005 | 3.1.1 | Visual Studio 2005-style chrome for the WinForms desktop shell |
| jQuery | 1.8.3 | Older client behavior shared across MVC and Web Forms |
| Modernizr | 2.6.2 | Era-authentic storefront front-end dependency |
| NUnit | 3.12.0 | In-solution legacy test project |

## Notes

- **FreeTextBox** is the selected non-Microsoft legacy Web Forms control dependency. It is a believable third-party rich text control for franchise bulletin and promo editing, and it reinforces the uneven vendor story beside AJAX Control Toolkit.
- **DockPanelSuite** is the selected non-Microsoft Windows Forms control dependency. It gives the dispatch desktop a vendor docking shell instead of a pure stock WinForms layout, which makes the sample feel more organically assembled over time.
- The package set is intentionally mixed across eras. The app started in the mid-2000s, but some dependencies reflect later incremental upgrades rather than a clean platform refresh.
- Keep package management on `packages.config` for the legacy projects unless a later work item intentionally modernizes it.
