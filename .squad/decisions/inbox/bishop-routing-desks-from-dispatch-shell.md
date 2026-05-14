### 2026-05-14T15:35:48.472+02:00: Routing desks from dispatch shell
**By:** Bishop
**What:** Route planning and driver assignment should live as separate dense WinForms desks launched from the dispatch board, while desktop-only planning columns stay derived in the client from `DispatchTicket` data.
**Why:** That preserves the lived-in workstation flow operators expect and keeps UI-only heuristics out of shared contracts or service boundaries.
**Impact:** Later desktop work can add more operator desks without turning each one into a separate executable or widening backend contracts just to satisfy screen chrome.
