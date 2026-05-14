# 2026-05-14T09:58:57.628+02:00: Core container bootstrap path

**By:** Ripley

**What:** `CreateConfiguredContainer()` owns the core Unity registrations for fresh containers. `InitializeServiceLocator(IUnityContainer)` only attaches the CommonServiceLocator bridge to an already-configured container and must fail fast if the caller skipped registration.

**Why:** Downstream hosts need one believable composition contract before they start wiring `Global.asax` or desktop startup code. Letting `InitializeServiceLocator(...)` silently re-register services made the seam ambiguous and hid whether callers were passing a fresh container or a prepared one.
