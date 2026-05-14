# Issue 52 interop baseline

- **Date:** 2026-05-14T15:35:48.472+02:00
- **Context:** `main` does not yet carry the Phase 7.6 interop implementation, but the legacy seams that will absorb it already exist in the gateway, batch job, WCF host, and ASMX host.
- **Decision:** Hudson is locking the reviewer baseline in `src\before\Fabrikam.EnterprisePizza.Legacy.Tests` instead of guessing at future container APIs. Review issue #52 against preserved observable behavior: `LegacyDbGateway.GetConnectionName`, `NightlyPosImportJob.GetTargetDatabase`, `StoreDispatchService.GetDispatchBoard`, and `PartnerSyncService.GetPreferredPartners` must keep returning the same seeded outputs after the interop layer lands.
- **Why:** This gives the implementation specialist freedom to change wiring patterns while preserving the legacy sample's believable outputs and host-level seams.
