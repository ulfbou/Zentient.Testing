# Zentient.Testing

Lightweight test harness, minimal mock engine, and scenario DSL (Arrange → Act → Assert).

This repository contains two packages:

- Zentient.Abstractions.Testing — lightweight public interfaces used by the testing DSL.
- Zentient.Testing — implementation: harness, mock engine and scenario helpers.

NuGet

```bash
dotnet add package Zentient.Testing --version 0.1.0-alpha.1
```

Quick example (Arrange → Act → Assert)

```csharp
// Arrange: build scenario that resolves a handler and exercise it
var scenario = TestScenario.ForHandler<MyHandler, MyInput, MyResult>(async (handler, input, ct) => await handler.Handle(input, ct));

scenario.Arrange(builder =>
{
    builder.WithDependency<IMyRepository>(new InMemoryRepository());
    builder.WithMock<IMyService>(m => m.Given(s => s.Calculate(It.IsAny<int>())).ThenReturns(42));
});

// Act
var result = await scenario.ActAsync(new MyInput { Value = 1 });

// Assert
scenario.Assert(a => a.NotBeNull());
scenario.Assert(a => a.With(x => x.Value, expectedValue).And);
```

Package signing & release notes

- The CI workflow (.github/workflows/ci-cd.yml) builds, runs tests, generates coverage, packs NuGet packages for projects with `<IsPackable>true</IsPackable>`, and produces artifacts.
- For publishing to NuGet, the workflow expects a repository secret named `NUGET_API_KEY`.
- Symbol signing is optional; provide `SIGNING_KEY` secret and implement signing commands in the `sign-symbols` job if you require signed symbol packages.
- The `publish-prerelease` job pushes prerelease packages when runs are on `release/*` branches (if `NUGET_API_KEY` is configured) and will create a GitHub prerelease with attached artifacts.

Change log

See CHANGELOG.md for release history and notes.

Contributing

We welcome contributions\! If you're interested in contributing to Zentient.Testing, please visit our [GitHub Repository](https://github.com/ulfbou/Zentient.Endpoints) and refer to the [`CONTRIBUTING.md`](https://www.google.com/search?q=https://github.com/ulfbou/Zentient.Endpoints/blob/main/CONTRIBUTING.md) guide.

License

Zentient.Testing is licensed under the MIT License. See the [`LICENSE`](https://www.google.com/search?q=https://github.com/ulfbou/Zentient.Endpoints/blob/main/LICENSE) file for more details.
