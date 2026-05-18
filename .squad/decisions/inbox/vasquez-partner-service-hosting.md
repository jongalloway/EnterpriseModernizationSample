# Vasquez decision inbox

- **Date:** 2026-05-18T01:39:47.894-07:00
- **Agent:** Vasquez
- **Issue:** #34, #35
- **Decision:** Host the new partner workflow service and the backward-compat SOAP endpoint side-by-side in `Fabrikam.EnterprisePizza.Services.PartnerSync`, with WCF carrying the primary `basicHttpBinding` contract and `PartnerLookup.asmx` acting as the legacy compatibility facade over the same CustomerHub business/repository seam.
- **Why:** That keeps the sample's SOA history believable: newer clients get a metadata-friendly WCF surface, but older franchise and partner integrations still talk to an ASMX endpoint that feels pre-WCF without forking the underlying business rules.
- **Impact:** Future modernization work can swap service edges independently while preserving one Enterprise Library-style CustomerHub seam for partner lookup, contract maintenance, referrals, and commission processing.
