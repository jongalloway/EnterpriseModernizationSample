---
name: "legacy-unity-core-seam"
description: "Seed a legacy .NET Framework core library with Unity registrations, a CommonServiceLocator bridge, and small cross-domain business policies before the host projects wire the container."
domain: "architecture"
confidence: "high"
source: "earned"
---

## Context

Use this when a legacy sample needs an authentic Enterprise Library-era DI seam, but the web/service/desktop hosts are not ready to own full composition roots yet.

## Patterns

- Put the first Unity registrations in the shared core library, not in one arbitrary host.
- Register only genuinely cross-cutting services there: clocks, policy objects, low-level domain helpers.
- Add a CommonServiceLocator adapter beside the Unity registrations so later WCF/Web Forms/WinForms work can opt into the same seam without re-inventing it.
- Keep business policy code small and believable; the core library should seed downstream behavior, not swallow the whole business layer.
- Leave host-specific bootstrapping for later workitems so the architecture still shows its historical layering.

## Anti-Patterns

- Hiding the first DI seam inside a web project because it happens to compile first.
- Filling the core library with domain-specific repository code that belongs in StoreOps or CustomerHub.
- Waiting for every host to exist before introducing any shared DI surface; that just encourages ad hoc `new` calls everywhere.
