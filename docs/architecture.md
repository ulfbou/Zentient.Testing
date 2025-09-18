# Architecture — Zentient.Testing (Alpha)

This document provides a concise, high-level architecture overview for the alpha release of Zentient.Testing. It is intended for contributors and maintainers who implement or integrate the library.

## Goals

- Provide a compact, test-first DSL for Arrange ? Act ? Assert flows.
- Avoid leaking concrete DI or mocking framework types in the public API for alpha.
- Keep the implementation small, well-delineated, and easy to reason about.

## High-level Components

- **Zentient.Abstractions.Testing** (public)
  - Interfaces and small DTOs consumed by tests: ITestScenario, ITestHarnessBuilder, IMockBuilder, IResultAssertions.

- **Zentient.Testing** (implementation)
  - Test harness (TestHarness, TestHarnessBuilder): per-test service registry and simple resolver.
  - Mock engine (MockBuilder, MockEngine): expression-driven stubs with ThenReturns/ThenThrows semantics.
  - Assertions (ResultAssertions): helper methods for concise assertions and chaining.

- **Tests**
  - Unit tests and integration smoke tests exercising the harness, mock engine and expression evaluator.

## Component Responsibilities

- TestHarnessBuilder
  - Collect registrations (WithDependency, WithMock, Replace).
  - Materialize a TestHarness via Build().

- TestHarness
  - Provide Resolve<T>() with the following priority:
    1. Return registered instance if present.
    2. Attempt to construct via public constructor when all parameter types are registered.
    3. Throw a descriptive invalid operation exception when resolution fails.
  - Dispose any IDisposable registrations when disposed.

- MockBuilder / MockEngine
  - Allow mapping a captured call expression to a behavior (return value or exception).
  - Provide a verifier that can assert invocation counts (basic semantics in alpha).

- ResultAssertions
  - Expose small set of assertion helpers: NotBeNull, HaveValue, With(selector, expected), and chaining via And.

## Data flow (ASCII)

```
Test code -> TestScenario -> TestHarnessBuilder -> TestHarness -> Resolve SUT
                                 -> MockBuilder -> Mock instances injected into harness
```

## Future evolution (beta/RC)

- Adapter layering for DI (Microsoft DI) and mocking frameworks (Moq) to avoid reimplementation and provide richer mocking features.
- Diagnostics and validation hooks during Build() to surface registration issues earlier.
- Roslyn analyzers and performance tuning in later stages.

## Non-goals for alpha

- A full DI container implementation.
- Advanced mock verification features (spies, complex matchers beyond It.IsAny in alpha).

## Notes

- The architecture favors clarity over completeness. Keep public contracts minimal and stable for the alpha.
