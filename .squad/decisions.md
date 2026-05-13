# Squad Decisions

## Active Decisions

### 2026-05-13T20:38:19.745+02:00: User directive
**By:** Scribe (via Copilot)
**What:** Use Enterprise Library, Patterns & Practices guidance, Stock Trader, and the Blue Yonder CAB sample as reference points for the legacy .NET sample's architecture and feel.
**Why:** User request — captured for team memory

### 2026-05-13T20:42:26.144+02:00: User directive
**By:** Scribe (via Copilot)
**What:** Structure the repository so it can hold docs and source folders for both before and after migration, and model the system with one or more SQL Server databases.
**Why:** User request — captured for team memory

### 2026-05-13T20:51:37.753+02:00: User directive
**By:** Scribe (via Copilot)
**What:** Use a Microsoft-style business name in the scenario, with Fabrikam Enterprise Pizza as a strong candidate, and carry 2005-era throwbacks throughout the design and business domain, including dated trends such as a MySpace footer link.
**Why:** User request — captured for team memory

### 2026-05-13T20:54:45.785+02:00: User directive
**By:** Scribe (via Copilot)
**What:** The app should feel organically grown over time, include some older NuGet packages that were popular in the early 2010s, and include business logic projects in the solution.
**Why:** User request — captured for team memory

### 2026-05-13T20:52:10.614+02:00: Hicks web direction
**By:** Hicks
**What:** Push the public-facing web surfaces toward a clear 2005 corporate-web identity under **Fabrikam Enterprise Pizza**, using slightly over-polished portal chrome, upbeat enterprise marketing copy, and era-authentic social/web references.
**Why:** This gives Web Forms, MVC, and portal content a shared visual story without making the sample feel parody-heavy or historically incoherent.

**Implementation notes:**
- Treat **Fabrikam Enterprise Pizza** as the polished marketing-facing name, while older internal wording can still drift between portal areas.
- Favor blue gradients, beveled boxes, badge promos, utility nav, and "trusted partner" language over modern minimalism.
- Use dated-but-believable web culture touches such as a MySpace footer link, newsletter signup copy, "best viewed" hints, and limited-time promo badges.

### 2026-05-13T20:52:10.614+02:00: Fabrikam Enterprise Pizza direction
**By:** Ripley
**What:** Use **Fabrikam Enterprise Pizza** as the baseline business name, carry explicit 2005-era throwbacks throughout the domain and presentation, and frame SQL Server assets under `data\sqlserver\before`, `data\sqlserver\after`, and `data\sqlserver\shared`.
**Why:** The Microsoft-style name matches the user's direction without weakening credibility, the throwbacks make the legacy sample legible at a glance, and the database split gives the before/after migration story a clear seam instead of one catch-all data folder.

### 2026-05-13T20:52:10.614+02:00: Fabrikam pizza data layout
**By:** Vasquez
**What:** Use **Fabrikam Enterprise Pizza** as the business name and model the legacy SQL Server estate as three databases: `FabrikamPizza_StoreOps`, `FabrikamPizza_CustomerHub`, and `FabrikamPizza_Reporting`.
**Why:** That split feels authentically mid-2000s: core order/store writes stay isolated, customer/franchise data grows into its own integration-heavy database, and reporting is peeled off for nightly ETL so operations do not trust live transactional queries.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
