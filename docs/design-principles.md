# Design Principles — Zentient.Testing

- Developer Experience first: prioritize terse, discoverable APIs.
- Non-invasive: do not expose concrete DI/mocking framework types in public interfaces.
- Test-scoped isolation: each harness instance represents a test scope and disposes local resources.
- Fail-fast diagnostics: descriptive, actionable exceptions for misconfiguration.
- Progressive enhancement: small alpha core, then adapters and diagnostics in beta.
