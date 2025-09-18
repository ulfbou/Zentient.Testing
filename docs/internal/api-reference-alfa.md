Zentient.Testing — API Reference (alfa)

Overview
This document describes the public API surface for the alfa release of Zentient.Testing. It lists types, methods and short usage examples. This reference corresponds to the package versions published from the v0.1.0-alfa tag.

Namespaces
- Zentient.Testing
- Zentient.Testing.Internal (internal-only; included for reference but not recommended for direct consumption)
- Zentient.Abstractions.Testing

Usage summary
- The library provides lightweight testing harnesses, mock builders, mock verifiers and result assertions targeting .NET 8/9.
- For most scenarios use TestHarnessBuilder -> TestHarness -> TestScenario to create handler tests and verify behavior.

Public types

Zentient.Testing.TestHarnessBuilder
- Methods
  - Build(): TestHarness
  - With(...): TestHarnessBuilder (fluent builder methods to configure DI, fakes and behaviors)

Zentient.Testing.TestHarness
- Methods
  - RunScenario(TestScenario): Task
  - Resolve<T>(): T
  - ForHandler<THandler>(): ITestScenario<TInput, TResult>

Zentient.Testing.TestScenario<TInput,TResult>
- Properties
  - Input: TInput
- Methods
  - Execute(): TResult
  - ExecuteAsync(): Task<TResult>

Zentient.Testing.MockEngine
- Responsible for dynamic proxy generation and behavior matching.
- Methods
  - CreateMock<T>(): IMockBuilder<T>
  - Verify<T>(): IMockVerifier<T>

Zentient.Testing.ResultAssertions
- Provides assertions for verifying function results in a fluent style.
- Methods
  - ShouldBeSuccess()
  - ShouldBeFailure()

Zentient.Abstractions.Testing
- Interfaces
  - IMockBuilder<T>
    - Setup(Expression<Action<T>>): IMockBuilder<T>
    - Build(): T
  - IMockVerifier<T>
    - Verify(Expression<Action<T>>, Times): void
  - ITestHarness
    - CreateScenario<TInput,TResult>(): ITestScenario<TInput,TResult>

Examples
Basic scenario

1) Build harness and run scenario

```csharp
var builder = new TestHarnessBuilder()
  .WithService<MyDependency>(new MyDependencyStub())
  .WithDefaultTimeout(TimeSpan.FromSeconds(5));

var harness = builder.Build();

var scenario = harness.CreateScenario<MyInput, MyResult>(input);
var result = scenario.Execute();
result.ShouldBeSuccess();
```

Mock usage

```csharp
var engine = new MockEngine();
var mock = engine.CreateMock<IMyService>()
    .Setup(s => s.Do(It.IsAny<string>())).Returns(42)
    .Build();

var verifier = engine.Verify<IMyService>();
verifier.Verify(s => s.Do("abc"), Times.Once());
```

Notes
- This is a snapshot of the API for the initial alfa release and may change in subsequent releases. Always refer to PublicAPI.Shipped.txt for the authoritative shipped surface.