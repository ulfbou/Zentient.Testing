# API Reference — Zentient.Testing (Alpha)

This document merges the API overview and reference for the alpha release. It describes the public abstractions provided by **Zentient.Abstractions.Testing**, their responsibilities, and short usage examples.

## Overview

The alpha API is deliberately small. Target interfaces:

- **ITestScenario<TInput, TResult>** — orchestrates Arrange ? Act ? Assert for a handler or SUT.
- **ITestHarnessBuilder** — builder API used during Arrange to register dependencies and mocks.
- **IMockBuilder<TService>** — minimal mock DSL for configuring behaviors.
- **IResultAssertions<TResult>** — assertion helpers used by scenario.Assert.

## Interfaces

### ITestScenario<TInput, TResult>

Purpose: Encapsulate a unit test scenario for a handler or SUT and provide the fluent test lifecycle.

Members:

- Arrange(Action<ITestHarnessBuilder> configure)
- Task<TResult> ActAsync(TInput input, CancellationToken ct = default)
- Assert(Action<IResultAssertions<TResult>> assertions)

Example:

```csharp
var scenario = TestScenario.ForHandler<MyHandler, MyInput, MyResult>((h,input,ct) => Task.FromResult(h.Handle(input)));
scenario.Arrange(builder => builder.WithDependency(...));
var result = await scenario.ActAsync(...);
scenario.Assert(a => a.NotBeNull());
```

---

### ITestHarnessBuilder

Purpose: Provide test-scoped registration APIs for dependencies and mocks.

Common methods:

- WithDependency<TService>(TService instance)
- WithMock<TService>(Action<IMockBuilder<TService>> configure)
- Replace<TService>(TService instance)
- Build()

Notes: For the alpha, Build returns an internal TestHarness that resolves types either from registrations or by construction when possible.

---

### IMockBuilder<TService>

Purpose: Configure simple stubs for a service type.

API surface (alpha):

- Given(Expression<Func<TService, object>> call)
- ThenReturns(object result)
- ThenThrows(Exception ex)

Example:

```csharp
builder.WithMock<ICalc>(m => m.Given(x => x.Add(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(42));
```

---

### IResultAssertions<TResult>

Purpose: Provide a focused set of assertion helpers used inside scenario.Assert.

Helpers (alpha):

- NotBeNull()
- HaveValue(TResult expected)
- With<TProperty>(Expression<Func<TResult, TProperty>> selector, TProperty expected)
- And (chaining)

Example:

```csharp
scenario.Assert(a => a.With(r => r.Value, 42).And.NotBeNull());
```

## Guidance & Migration Notes

- Code against the abstractions, not concrete implementations, to simplify future adapter work.
- Expect small API additions in beta; core shapes should remain stable for adapter integration.

## Where to find more

- See docs/usage-examples.md for copy-pasteable examples.
- For packaging and CI, refer to docs/packaging.md and the GitHub Actions workflow.
