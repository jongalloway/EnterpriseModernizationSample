---
name: "repo-vs-scenario-naming"
description: "Choose a descriptive repository name while keeping a stronger business/domain name for the sample scenario inside the repo."
domain: "architecture"
confidence: "high"
source: "earned"
---

## Context

Use this pattern when a repository is both a delivery container and a branded sample system. It applies when the repo must explain its modernization or migration purpose, but the system inside it needs a memorable domain identity for solution, project, UI, and database naming.

## Patterns

- Name the **repository** for the artifact's role if it contains more than one layer of concern (for example: before/after docs, source, data, migration assets).
- Name the **scenario/system** for the business domain if that gives the sample a clearer story and more believable internal branding.
- Keep the seam explicit: repo name answers "what is this container for?", while scenario name answers "what business system lives here?"
- Let downstream assets inherit the scenario name: solution names, project prefixes, database names, portal branding, and docs titles.

## Examples

- Repository: `EnterpriseModernizationSample`
- Scenario: `Fabrikam Enterprise Pizza`
- Internal assets: `FabrikamPizza.StoreOps`, `FabrikamPizza.CustomerHub`, `FabrikamPizza_Reporting`

## Anti-Patterns

- Using one catchy domain name for the entire repo when the repo also needs to communicate broader modernization scope.
- Using a generic repo name and then failing to carry a distinct scenario name through the actual solution and docs.
- Renaming the scenario to match the repo when existing architectural decisions already depend on the stronger domain identity.
