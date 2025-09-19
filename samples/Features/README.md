# Samples.Features.Tests — README

This folder contains a set of small sample tests that demonstrate the usage of the lightweight `Zentient.Testing` harness and mock DSL. The tests are organized by feature so each file focuses on one area of the API.

The sample project targeting `net8.0` and `net9.0` is `Features/Samples.Features.Tests.csproj` and is the test project you can run with `dotnet test`.

---

## How the samples are organized

- Tests are grouped by feature. Each file contains one or more related tests and, when appropriate, private helper types used only by those tests.
- Shared test utilities and simple DTOs used by multiple tests live in `Features/UtilityTypes.cs`.
- Naming convention: tests have descriptive names indicating the behaviour or expectation, e.g. `Mock_ItIsAny_WithNumerics_ReturnsConfiguredValue`.

---

## Files and purpose

- `Samples.Features.Tests.csproj`
  - The test project file. Multi-targets `net8.0` and `net9.0`. References `xunit`, `Microsoft.NET.Test.Sdk` and the `Zentient.Testing` projects.

- `UtilityTypes.cs`
  - Shared, small helper types used across multiple tests:
    - `ISimpleService`, `SimpleServiceImpl`, `SimpleHandler` — used by many mocking/dependency tests.
    - `ResultDto`, `IResultProvider`, `ResultProvider`, `ResultHandler` — used by result/assertion examples.
    - `ICalculator`, `AdderHandler` — used by numeric/ad hoc mocking examples.
  - Keep utilities here if they are referenced by more than one feature test file.

- `DependencyTests.cs`
  - Demonstrates registration of concrete dependencies and replacing registrations in the test harness.
  - Tests:
    - `Arrange_WithDependency_UsesConcreteImplementation` — shows `WithDependency<T>` wiring a concrete implementation for the handler under test.
    - `Replace_Registration_ReplacesDependency` — demonstrates `Replace<T>` to override a previously registered dependency.
  - Includes a private nested `OverrideService` used only by the replacement test.

- `MockingTests.cs`
  - Exercises the mock DSL, `Given(...).ThenReturns(...)`, `ThenThrows(...)`, and mock verifiers.
  - Tests:
    - `Arrange_WithMock_ReturnsConfiguredValue` — configures a mock to return a value for matching arguments.
    - `Arrange_WithMock_ThenThrowsException` — configures a mock to throw an exception for a specific call.
    - `Mock_Verifier_Records_Calls_OutParam` — shows building a mock with an out `IMockVerifier<T>` to inspect recorded calls.
    - `Mock_MultipleBehaviors_LaterSpecificOverridesEarlier` — behavior ordering and priority.
    - `Mock_Void_Method_VerifiedCallCount` — mocking a void-returning method and asserting call counts.
  - Contains private `IVoidService`/`VoidCaller` utilities used solely by the void mock test.

- `AsyncAndCallerTests.cs`
  - Shows usage of asynchronous mocks and a simple caller/spy example.
  - Tests:
    - `Async_Mock_Returns_TaskResult` — demonstrates configuring a `Task` returning mock for async handlers.
    - `AsyncHandler_Uses_WorkerToProduceResult` — async handler wired to a worker mock.
    - `Caller_Calls_Service_And_SpyRecordsCall` — a synchronous example of a caller using a spy to validate call details.
  - Contains nested private utilities used by those tests (e.g. `IAsyncService`, `AsyncHandlerDemo`, `IWorker`, `AsyncHandler`, `IService`, `Caller`, `ServiceSpy`). These are intentionally private and nested so they don't pollute the shared namespace.

- `CallerTests.cs`
  - Historical rename: a focused test for the caller/spy scenario. If both `CallerTests.cs` and `AsyncAndCallerTests.cs` contain caller samples, prefer one for the canonical example. The current codebase keeps the consolidated `CallerFeatureTests` in `CallerTests.cs`.

- `AdderTests.cs`
  - Numeric/mock examples for value-returning calls.
  - Tests:
    - `UnmatchedMock_Returns_DefaultValue_ForValueType` — if no mock behavior matches, value types return `default` (e.g., `0`).
    - `Mock_ItIsAny_WithNumerics_ReturnsConfiguredValue` — demonstrates `It.IsAny<T>()` wildcard matching for numeric arguments.
    - `AdderHandler_Returns_MockedResult_WhenUsingStub` — uses a local `CalculatorStub` to demonstrate substituting a concrete stub for the interface.
  - Contains `CalculatorStub` as a private helper for the stub-based test.

- `AssertionTests.cs`
  - Demonstrates the `IResultAssertions<TResult>` APIs provided by the test harness.
  - Tests:
    - `ResultAssertion_HaveValue_Works` — uses `HaveValue` to assert the result matches an expected `ResultDto` instance.
    - `ResultAssertion_AndAlso_Chaining_Works` — demonstrates chaining assertions via `AndAlso` and `WithProperty`.

---

## Conventions and best practices used in the sample tests

- Files are grouped by feature to make it easy to find focused examples.
- Helpers that are required by only a single test are declared as private nested types inside the same test file so they remain local and don't pollute the shared test namespace. Shared helpers remain in `UtilityTypes.cs`.
- Tests use `xUnit` and the sample harness `TestScenario.ForHandler<,>` which provides `Arrange(...)`, `ActAsync(...)` and `Assert(...)` helpers.
- Use descriptive test method names to explain the expected behaviour.

---

## How to run the samples

1. From the repository root in this samples folder run:

   - `dotnet restore Features/Samples.Features.Tests.csproj`
   - `dotnet test Features/Samples.Features.Tests.csproj`

   This runs the xUnit tests for the sample project across the targeted frameworks.

2. To run a single test, use `dotnet test --filter FullyQualifiedName~Namespace.ClassName.MethodName` or an IDE test runner.

---

## Extending the samples

- To add a new feature demonstration:
  1. Create a new test file named after the feature (for example `FeatureXTests.cs`).
  2. Put tests related to that feature into the file and declare private helper types there when the helpers are used only by those tests.
  3. Add shared helper types to `UtilityTypes.cs` only when they are used by two or more files.

- If you add new tests that require additional NuGet packages, update the `Samples.Features.Tests.csproj` accordingly.

---

## Notes

- The samples are intentionally small and focused on illustrating how to use `Zentient.Testing` APIs in isolation. They are not intended to be complete integration tests.
- The mock DSL in `Zentient.Testing` supports the `Given(...).ThenReturns(...)` and `ThenThrows(...)` primitives along with the `It.IsAny<T>()` wildcard helper.

If you'd like, I can also generate a short QuickStart snippet or split the README into a top-level `samples/README.md` and per-feature READMEs. Which would you prefer?