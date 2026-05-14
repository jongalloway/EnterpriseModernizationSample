# Web Forms Binding Safety Defaults

**By:** Hicks  
**Date:** 2026-05-14T22:58:44.433+02:00  
**Status:** Inbox (awaiting Scribe merge)

## Decision

On legacy Web Forms surfaces, data-bound content should be encoded by default when it lands in page markup, especially for link attributes and repeated portal content. Use small code-behind helpers or encoded binding syntax so future contributors do not inherit raw `Eval(...)` patterns as the default.

## Rationale

- Partner- and operator-facing portals are believable places for user-supplied notes, promo text, and external URLs to creep in over time.
- Encoding in the page layer preserves the legacy Web Forms feel without forcing a larger architecture cleanup.
- This keeps old markup maintainable while avoiding brittle "it was hardcoded when we wrote it" assumptions.

## Guidance

- Use `HttpUtility.HtmlEncode(...)` for visible text.
- Use `HttpUtility.HtmlAttributeEncode(...)` for attribute values such as `href`.
- Leave generated designer seams alone unless regeneration or build validation proves they are wrong; hand-editing those files is riskier than fixing the surrounding page behavior.
