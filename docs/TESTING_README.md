# Zentient.Testing — Alpha Quickstart

This document shows how to use the lightweight Zentient.Testing harness and mock APIs in the alpha release.

## Arrange → Act → Assert

Example:

```csharp
var scenario = TestScenario.ForHandler<MyHandler, (int a, int b), int>((h, input, ct) => Task.FromResult(h.Handle(input)));

scenario.Arrange(builder =>
{
    builder.WithMock<ICalculator>(mb => mb.Given(x => x.Add(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(42));
});

int result = await scenario.ActAsync((1, 2));
scenario.Assert(a => a.HaveValue(42));
```

## FluentAssertions license note

This repository includes tests that use FluentAssertions. FluentAssertions is published under the Xceed Fluent Assertions Community License. The project is free for non-commercial use. If you plan to use these tests in a commercial product or CI environment for a proprietary code base, please review the FluentAssertions licensing terms and obtain a commercial license if required: https://xceed.com/products/unit-testing/fluent-assertions/

## Nullability

The alpha APIs aim to provide nullable-aware signatures for common operations. `ThenReturns(object? result)` accepts null when configuring void-returning methods in the minimal mock DSL.

## Recommended test additions for alpha

- Add tests for behaviour priority and multiple matching rules.
- Test exception throwing from ThenThrows.
- Test async handlers and Task-returning methods.

These are already partly implemented in the example tests included in `tests/`.
