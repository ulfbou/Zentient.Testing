# Usage Examples — Zentient.Testing (Alpha)

This page collects short, copy?pasteable examples that demonstrate common usage patterns for the alpha release of Zentient.Testing. These examples are minimal by design and intended to illustrate the Arrange ? Act ? Assert flow.

## Handler with dependency and mock

The following example shows a simple handler that depends on an ICalculator service. The test registers a mock for the calculator and asserts the result.

```csharp
public class AdderHandler
{
    private readonly ICalculator _calc;
    public AdderHandler(ICalculator calc) => _calc = calc;
    public int Handle((int a, int b) input) => _calc.Add(input.a, input.b);
}

// Arrange: create a scenario that resolves AdderHandler from the harness
var scenario = TestScenario.ForHandler<AdderHandler,(int a,int b),int>((h, input, ct) => Task.FromResult(h.Handle(input)));

scenario.Arrange(builder => {
    // Register a mock implementation that always returns 100
    builder.WithMock<ICalculator>(mb => mb.Given(x => x.Add(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(100));
});

// Act
var result = await scenario.ActAsync((1, 2));

// Assert
Assert.Equal(100, result);
```

## Assertions examples

Use the ResultAssertions API to make focused assertions about the result. The API supports chaining for readable assertions.

```csharp
// Ensure result is not null
scenario.Assert(a => a.NotBeNull());

// Assert on a selected property and expected value
scenario.Assert(a => a.With(r => r.Value, 42));
```

## Notes

- These examples show the alpha surface. Adapter-backed mocking (Moq) and DI hosting are planned for the beta release and will provide richer integration points.
- Prefer the public abstractions (ITestScenario, ITestHarnessBuilder, IMockBuilder, IResultAssertions) when writing tests so future adapter work remains compatible.
