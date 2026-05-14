# Enterprise Scenario and Repository Framing

## Selected business

**Fabrikam Enterprise Pizza**

- **Tagline:** Hot slices. Corporate precision.
- **What the company does:** Runs franchise ordering, corporate catering, store dispatch, coupon campaigns, franchise accounting, and nightly POS synchronization for a regional pizza chain that grew fast during the mid-2000s.
- **Why this name wins:** It keeps the Microsoft-style fake-company language the user asked for, while the straight-faced “Enterprise Pizza” phrasing lands better than a joke spelling once it is repeated across solutions, databases, portals, and docs.

## 2005-era details that should show up across the sample

1. The public site should feel like a late-added “online ordering” front end on top of older store systems.
2. The footer should include a MySpace link beside the usual contact, privacy, and careers links.
3. Marketing should lean on printable coupon PDFs, “email this deal,” and family-combo names like *Extreme Value Tuesday*.
4. Corporate catering should still accept faxed order forms that get re-keyed by call-center staff.
5. Store and menu data should synchronize nightly through scheduled POS/VPN imports with exception reports.
6. Call-center reps should use scripted upsell prompts for breadsticks, 2-liter drinks, and dessert add-ons.
7. The franchise portal should publish weekly Excel packs, PDF bulletins, and store-compliance notices.
8. The store locator should offer MapQuest-style driving directions and neighborhood delivery-zone language.
9. Reporting should revolve around SSRS exports, emailed CSVs, and finance-ready end-of-day summaries.
10. Design language should favor glossy gradients, bevel-heavy buttons, stock-photo hero banners, and dated promo copy rather than clean modern minimalism.

## Recommended top-level repository framing

```text
docs\
  foundation\
  before\
  after\
src\
  before\
  after\
data\
  sqlserver\
    before\
    after\
    shared\
```

### Framing guidance

- `docs\foundation\` holds scenario, naming, repository-shaping decisions, and migration framing.
- `docs\before\` holds the legacy solution map, user workflows, screenshots, support notes, and operational constraints.
- `docs\after\` holds target architecture, decomposition decisions, migration sequencing, and modernization trade-offs.
- `src\before\` holds the legacy .NET Framework estate with all of its historical layering intact.
- `src\after\` holds the migrated services, web apps, workers, and shared components.
- `data\sqlserver\before\` holds legacy schemas, seed data, and operational scripts.
- `data\sqlserver\after\` holds target-state schemas, migration scripts, and cutover support assets.
- `data\sqlserver\shared\` holds reference data, shared test fixtures, and neutral notes used by both sides of the migration.

### Suggested SQL Server database shape

- `FabrikamPizza_StoreOps` — store dispatch, workforce workflows, route boards, staging, and nightly POS synchronization
- `FabrikamPizza_CustomerHub` — customer profiles, corporate catering accounts, franchise partner contacts, and referral/program data
- `FabrikamPizza_Reporting` — finance rollups, labor snapshots, delivery KPIs, and downstream SSRS/export tables
