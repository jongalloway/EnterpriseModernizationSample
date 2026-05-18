# Vasquez decision inbox

- **Date:** 2026-05-18T01:39:47.894-07:00
- **Agent:** Vasquez
- **Issue:** #33
- **Decision:** Co-host a dedicated `DispatchService` in `Fabrikam.EnterprisePizza.Services.DispatchHost` and keep the contract on existing `Shared.Contracts.Routing` DTOs (`RoutePlan`, `DriverAssignmentRecommendation`, `DispatchQueueEntry`, `DeliveryTrackingRecord`, `DriverAvailability`, `DispatchTicket`, `StoreDispatchRequest`) instead of inventing host-local wrappers.
- **Why:** That keeps the WCF seam additive beside the already-hosted StoreOps services, preserves the SOAP-era “shared contract assembly” feel, and lets the service boundary expose route planning, dispatch queue, driver assignment, and delivery status behavior directly from `DispatchCoordinator` without another fake integration layer.
- **Impact:** Future service, desktop, and modernization work can point at one StoreOps WCF host with shared routing DTOs and a stable EntLib-configured composition story, while status updates still allow the first SOAP call to bootstrap tracking from a dispatch ticket.
