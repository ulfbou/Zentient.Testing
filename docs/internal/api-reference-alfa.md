# Zentient.Testing — API Reference (alfa)

Version: v0.1.0-alfa
Scope: public surface shipped with the alfa pre-release. This reference documents the primary types, interfaces and typical usage patterns. It is intended as a developer-facing wiki page (authoritative shipped surface: PublicAPI.Shipped.txt).

Table of contents
- Overview
- Key concepts
- Public namespaces
- Detailed API reference
  - TestHarnessBuilder
  - TestHarness
  - TestScenario<TInput,TResult>
  - MockEngine, IMockBuilder<T>, IMockVerifier<T>
  - ResultAssertions
- Examples
- Compatibility & notes

## Overview
Zentient.Testing is a compact testing library focused on scenario-driven handler tests and a small mock/verification engine. It aims to be lightweight and explicitly targeted at .NET 8/9.

## Key concepts
- Test harness: an execution container that wires DI, configuration and fake implementations for reproducible scenarios.
- Scenario: a single, focused test run described by input and expected result (sync or async).
- Mock engine: lightweight dynamic proxy factory that supports setup/verify patterns.
- Result assertions: fluent helpers to verify success / failure shapes and extract diagnostics.

## Public namespaces
- Zentient.Testing — core harnesses, scenarios, mocks and assertions.
- Zentient.Abstractions.Testing — minimal interfaces for DI-friendly mocking and harness composition.
- Zentient.Testing.Internal — internal helpers (not for public consumption; subject to change).

---

## Detailed API reference

### class: TestHarnessBuilder
Namespace: Zentient.Testing

Purpose
Build and configure a TestHarness instance. The builder provides methods to register services, configure timeouts and enable specific test behaviors.

Constructors
- TestHarnessBuilder()

Fluent methods
- WithService<TService>(TService instance) : TestHarnessBuilder
  - Registers a singleton instance overriding the production registration.
- WithTransient<TService, TImplementation>() : TestHarnessBuilder
  - Registers a transient mapping for the harness service collection.
- WithConfiguration(Action<IServiceCollection> configure) : TestHarnessBuilder
  - Allows arbitrary DI configuration for advanced scenarios.
- WithDefaultTimeout(TimeSpan timeout) : TestHarnessBuilder
  - Sets a harness-level timeout used by scenario execution.

Terminal
- Build() : TestHarness
  - Returns an immutable TestHarness configured with the specified services.

Remarks
The builder is safe to use in parallel for independent harnesses.

---

### class: TestHarness
Namespace: Zentient.Testing

Purpose
Runtime container for executing scenarios and resolving services inside a controlled scope.

Methods
- Resolve<T>() : T
  - Resolve a service from the harness service provider (scoped to each scenario where appropriate).
- CreateScenario<TInput,TResult>(TInput input) : TestScenario<TInput,TResult>
  - Create a scenario object ready to execute with the harness' configuration.
- RunScenarioAsync<TInput,TResult>(TestScenario<TInput,TResult> scenario) : Task<TResult>
  - Execute a scenario using harness policies (timeouts, retries if configured).

Disposal
- Implements IDisposable / IAsyncDisposable where underlying DI scopes or resources must be released.

---

### class: TestScenario<TInput,TResult>
Namespace: Zentient.Testing

Purpose
Represents one test invocation including input, optional preconditions, and execution logic.

Properties
- Input : TInput
- Metadata : IDictionary<string,string> (optional)

Methods
- Execute() : TResult
- ExecuteAsync() : Task<TResult>

Usage
Construct via TestHarness.CreateScenario(...). Scenarios capture context and can expose hooks for assertions or result transformations.

---

### class: MockEngine
Namespace: Zentient.Testing

Purpose
Provides creation of mock instances and verification helpers. It is intentionally small and designed for common mocking tasks without the complexity of a full mocking framework.

Methods
- CreateMock<T>() : IMockBuilder<T>
- Verify<T>(T instance) : IMockVerifier<T>

---

### interface: IMockBuilder<T>
Namespace: Zentient.Abstractions.Testing

Purpose
Fluent builder used to configure behaviors on a mock before materialization.

Methods
- Setup(Expression<Action<T>> expression) : IMockBuilder<T>
  - Configure a void method expectation or behavior.
- Setup<TResult>(Expression<Func<T, TResult>> expression, TResult returnValue) : IMockBuilder<T>
  - Configure a function to return a value when invoked with matching arguments.
- Build() : T
  - Produce the mock instance usable by consumers.

---

### interface: IMockVerifier<T>
Namespace: Zentient.Abstractions.Testing

Purpose
Verification API to assert that configured mocks were exercised as expected.

Methods
- Verify(Expression<Action<T>> expression, Times times)
  - Ensure the expression was invoked the expected number of times. Times is a small struct with helpers: Once(), Times(int), Never().

---

### class: ResultAssertions
Namespace: Zentient.Testing

Purpose
Fluent result assertions to interpret success/failure shapes from scenario execution.

Methods
- ShouldBeSuccess(this TResult) : TResult
  - Throws an assertion exception if the result indicates failure; returns the result for chaining.
- ShouldBeFailure(this TResult) : TResult
  - Asserts the result represents a failure and returns failure details for inspection.

---

## Examples

### Synchronous scenario example
```csharp
var harness = new TestHarnessBuilder()
    .WithService<IMyDependency>(new MyStub())
    .Build();

var scenario = harness.CreateScenario<MyInput, MyResult>(new MyInput { Value = 42 });
var result = scenario.Execute();
result.ShouldBeSuccess();
```

### Mocking example
```csharp
var engine = new MockEngine();
var builder = engine.CreateMock<IMyService>();
builder.Setup(s => s.Do(It.IsAny<string>()), 42);
var mock = builder.Build();

// Exercise mock
var v = mock.Do("abc"); // returns 42

var verifier = engine.Verify<IMyService>(mock);
verifier.Verify(s => s.Do("abc"), Times.Once());
```

---

## Compatibility & notes
- The alfa release aims for .NET 8/9 compatibility. Some APIs may change in the beta release.
- Public API is documented here for convenience; the authoritative shipped API is the PublicAPI.Shipped.txt file used by the project.

If you need more detail for a particular type or module, request it and a dedicated section will be added.