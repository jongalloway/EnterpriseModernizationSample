# 2026-05-18T01:39:47.894-07:00 — DAL tests use recording gateways and Enterprise Library factory resolution

**By:** Hudson  
**Issue:** #42

**What:** Keep DAL coverage in `src\before\Fabrikam.EnterprisePizza.Tests.Unit` by making `LegacyDbGateway` overridable for recording test doubles and by adding a small `LegacyDatabaseFactory` that resolves Enterprise Library `Database` instances from `LegacyConnectionCatalog` names.

**Why:** Repository fixtures need deterministic assertions on stored procedure names, parameter payloads, and mapped `DataSet` rows without a live SQL Server. The factory seam keeps Enterprise Library wiring visible to reviewers while the recording gateway keeps repros local and fast.

**Impact:** Future DAL and service tests can capture command construction directly, and reviewers can validate connection-name-to-database resolution without standing up any database infrastructure.
