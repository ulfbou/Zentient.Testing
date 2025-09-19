# CHANGELOG — Zentient.Testing

All notable changes to this project are recorded in this file. This project follows Semantic Versioning.

## Unreleased

- Working toward 0.1.0 (alpha) — internal validation, tests and packaging.

## [0.1.0-alpha.1] - 2025-09-18

Initial public alpha release of Zentient.Testing. This lightweight package provides a small, ergonomic testing DSL and harness for Arrange → Act → Assert flows without depending on a platform DI container.

### Highlights
- Added public abstractions in Zentient.Abstractions.Testing: ITestScenario, ITestHarnessBuilder, IMockBuilder, IResultAssertions.
- Implemented a minimal TestHarness and TestHarnessBuilder that support WithDependency, WithMock and Replace semantics.
- Small built-in mock engine with Given(...).ThenReturns(...) and ThenThrows(...), plus a simple verifier API.
- Result assertions: NotBeNull, HaveValue, With(selector, expected) and chaining.
- Expression utilities (Zentient.Expressions) exposed via ExpressionRegistry.DefaultParser and DefaultEvaluator and validated by unit tests.
- CI and packaging: projects multi-target net8.0/net9.0, packable metadata added and a validated pack pipeline (local validation).
- Validation scripts: scripts to load .env and validate build/test/pack locally.

### Notes
- This alpha is intentionally small and non-invasive: it avoids exposing concrete DI or mocking frameworks in public API. Adapters (Moq, Microsoft DI) and broader diagnostics are planned for beta.
- Tests currently run using xUnit + FluentAssertions in the test project; a VSTest adapter for running tests that only reference Zentient.Testing is a future task (beta/RC).
