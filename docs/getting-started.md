# Getting Started — Zentient.Testing (Alpha)

This quick guide helps you add Zentient.Testing to a project, write a simple scenario-based test, and run local validation for build/test/pack.

## Prerequisites

- .NET 8 SDK or later (the project also targets .NET 9).
- Optional: a unit test runner (xUnit is used in this repository for discovery).

## Install

Add the package from your NuGet feed (replace version as needed):

```bash
dotnet add package Zentient.Testing --version 0.1.0-alpha
```

Or add the project reference if working from source:

```bash
dotnet add <your-test-project> reference <path-to>/src/Zentient.Testing/Zentient.Testing.csproj
```

## Write a simple test

Example using a handler-style SUT:

```csharp
[Fact]
public async Task AdderHandler_ReturnsConfiguredMockValue()
{
    var scenario = TestScenario.ForHandler<AdderHandler,(int a,int b),int>((h,input,ct) => Task.FromResult(h.Handle(input)));

    scenario.Arrange(builder => {
        builder.WithMock<ICalculator>(mb => mb.Given(x => x.Add(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(100));
    });

    var result = await scenario.ActAsync((1,2));

    scenario.Assert(a => a.With(r => r, 100));
}
```

## Run tests

Run tests using your preferred test runner. Example with `dotnet test`:

```bash
dotnet test
```

Note: This repository uses xUnit for test discovery. The final product may ship adapters that remove the direct need for xUnit in the test project in a later release.

## Local CI-style validation (build → test → pack)

A convenience script is provided to validate the repository locally, performing build, test and pack steps.

```powershell
..\ulfbou\scripts\validate.ps1
```

The script will:

- Load environment variables from `../.env` if present.
- Build all `src/` projects.
- Build and run tests in `tests/`.
- Pack any project with `<IsPackable>true</IsPackable>` into `artifacts/`.
- Attempt to sign packages if `SIGNING_KEY` is present in the environment.

## Troubleshooting

- If dotnet test cannot discover tests, ensure the test project references an appropriate test SDK (e.g., Microsoft.NET.Test.Sdk & xUnit runner) or use the provided validate.ps1 which builds projects before invoking tests.
- If packaging fails due to missing README.md, ensure `README.md` exists at the repository root; packaging expects this file to be included in nupkg.

## Next steps

- Read the API Reference: `docs/api.md`.
- Explore example scenarios: `docs/usage-examples.md`.
- Review packaging guidance: `docs/packaging.md`.
