# Zentient.Testing

**Lightweight test harness, minimal mock engine, and scenario DSL (Arrange → Act → Assert).**

Zentient.Testing provides a compact, dependency-light testing framework intended for fast, readable tests of handler-style components. The alpha release focuses on ergonomics and a small, stable public surface; advanced DI and mocking adapters are planned for future releases.

## Highlights (Alpha)
- Small public surface in **Zentient.Abstractions.Testing**: ITestScenario, ITestHarnessBuilder, IMockBuilder, IResultAssertions.
- Lightweight implementation in **Zentient.Testing**: TestHarness, MockBuilder, ResultAssertions and a tiny mock engine.
- Multi-targeted to **.NET 8** and **.NET 9**.

## Quickstart
Install the alpha package from your feed (or local artifacts):

```bash
dotnet add package Zentient.Testing --version 0.1.0-alpha.1
```

Minimal example

```csharp
// Arrange: create scenario that constructs/resolves a handler from the harness
var scenario = TestScenario.ForHandler<MyHandler, MyInput, MyResult>((h, input, ct) => Task.FromResult(h.Handle(input)));

scenario.Arrange(builder =>
{
    builder.WithDependency<IMyRepository>(new InMemoryRepository());
    builder.WithMock<IMyService>(m => m.Given(x => x.Calculate(It.IsAny<int>())).ThenReturns(42));
});

// Act
var result = await scenario.ActAsync(new MyInput());

// Assert
scenario.Assert(a => a.NotBeNull());
scenario.Assert(a => a.With(r => r.Value, 42));
```

## Documentation
See the `docs/` folder for detailed information:
- Architecture and design principles
- API reference and usage examples
- Packaging and release notes

## Running tests & local validation
- Tests in this repository use **xUnit** for discovery. Run them with:
  ```bash
  dotnet test
  ```
- For local CI-style validation (build → test → pack) use the included script:
  ```powershell
  ..\ulfbou\scripts\validate.ps1
  ```

## Contributing
Contributions are welcome. Please respect the Code of Conduct and follow contribution guidelines in the repository.

## License
MIT — see the `LICENSE` file for details.
