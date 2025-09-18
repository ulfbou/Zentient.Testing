# Alpha Release Notes — Zentient.Testing (0.1.0-alpha)

This document summarizes the scope and limitations of the alpha release.

Scope
- Minimal public abstractions (ITestScenario, ITestHarnessBuilder, IResultAssertions, IMockBuilder).
- Lightweight TestHarness implementation with per-test registrations and simple constructor resolution.
- Built-in mock engine providing Given(...).ThenReturns(...) and ThenThrows(...), basic verifier semantics.
- Result assertion helpers for common checks.

Limitations
- No platform DI integration; adapters planned for beta.
- No VSTest test adapter — tests are executed using existing test frameworks (xUnit) in CI.
- Diagnostics and analyzers are not yet implemented.

Migration notes
- Alpha API is intentionally small and may evolve into beta. Keep tests and usage patterns minimal and prefer the public abstractions.
