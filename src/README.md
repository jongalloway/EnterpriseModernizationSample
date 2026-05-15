# Source Layout

Keep executable code split by migration state so the legacy solution and the modernization target do not blur together.

- `src\before\` — the intentionally overgrown .NET Framework estate used as the baseline for demos and migration work.
- `src\after\` — the replacement services, apps, workers, and shared components that emerge from the modernization path.

Treat `before` as the system we inherited and `after` as the system we are shaping, not as parallel copies of the same project tree.
