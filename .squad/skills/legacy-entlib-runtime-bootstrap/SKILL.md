---
name: "legacy-entlib-runtime-bootstrap"
description: "Wire legacy .NET Framework service hosts through Enterprise Library-flavored appSettings so Unity, service location, exception policies, logging, and database aliases all hang off one configuration seam."
domain: "services"
confidence: "high"
source: "earned"
---

## Context

Use this when a WCF or ASMX host needs to feel like Enterprise Library/Unity-era infrastructure without dragging in a full modern DI stack or inventing a new infrastructure project.

## Patterns

- Read composition data from `enterpriseLibrary:*` keys so hosts can swap registrations without reopening `Global.asax` every time.
- Keep type aliases, container registrations, log-writer defaults, exception policies, and database aliases in the same configuration reader; legacy operators expected one ugly seam, not five competing ones.
- Bootstrap Unity from shared core code, then let the service locator expose resolved services to WCF/ASMX surfaces.
- Treat logging and exception policy as first-class bootstrap outputs, not afterthoughts; service methods should route failures through the configured policy before throwing back across the SOAP boundary.
- Keep the database factory thin and configuration-driven so connection aliases can drift per host while repository/gateway code still talks in old connection-name terms.

## Anti-Patterns

- Hardcoding repository/service registrations directly in each service host after the repo already has a shared composition seam.
- Building a sprawling fake EntLib clone with custom XML sections before the sample proves it needs them.
- Letting the gateway resolve raw connection strings everywhere; the connection-name seam is part of the modernization story.
