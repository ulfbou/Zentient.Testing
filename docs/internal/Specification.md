# Zentient.Testing — Staged Implementation Specification (Alpha → Beta → RC → 1.0)

Below is a complete, implementer-focused specification for each staged release of Zentient.Testing. It enables engineers to implement the library end-to-end without waiting for additional design decisions. This document provides precise class/interface names, method signatures (described), constructor responsibilities, invariants, implementation steps, tests to write, CI/packaging steps, and acceptance criteria for each stage.

---

## Overview / Guiding Principles (All Stages)

- **DX-first**: APIs should be terse, discoverable, and read like Arrange → Act → Assert. Defaults must be sensible; advanced options are opt-in.
- **Non-invasive**: Public API should not expose concrete DI/mocking framework types. Use adapter patterns and abstractions.
- **Test-scoped isolation**: Each test gets its own harness instance; resources registered by a harness are disposed when the harness is disposed.
- **Fail-fast diagnostics**: Developer-facing exceptions should be descriptive and actionable.
- **Progressive enhancement**: Start small (alpha), then add DI and adapters (beta), then analyzers and advanced diagnostics (RC), then polish and ecosystem adapters (1.0).

---

## Common Naming & Packaging Conventions (All Stages)

- **Root NuGet**: `Zentient.Testing` (alpha; gradually add modular packages).
- **Recommended Subpackages**:
    - `Zentient.Testing.Abstractions` — all public interfaces and minimal DTOs (always)
    - `Zentient.Testing.Internal` — default implementations (alpha/beta)
    - `Zentient.Testing.Adapters.Moq` — Moq adapter (beta)
    - `Zentient.Testing.Hosting.MicrosoftDi` — Microsoft DI adapter (beta)
    - `Zentient.Testing.Diagnostics` — extended diagnostics (RC)
    - `Zentient.Testing.Analyzers` — Roslyn analyzer pack (RC/1.0)

- Namespaces mirror packages. Interfaces begin with `I`. Keep classes small and single-responsibility.

---

## ALPHA (0.1.x) — Core DSL and Harness WITHOUT Platform DI

**Goal**: Validate the Arrange → Act → Assert flow, runtime ergonomics, and the mocking DSL without depending on Zentient.DependencyInjection.

### Implementer Surface

**Public contracts (in Zentient.Testing.Abstractions):**

- `ITestScenario<TInput, TResult>`
    - `Arrange(Action<ITestHarnessBuilder> configure)` — zero or more configure actions (idempotent: multiple calls append)
    - `ActAsync(TInput input, CancellationToken ct = default)` — invokes SUT using harness resolution rules; returns `Task<TResult>`
    - `Assert(Action<IResultAssertions<TResult>> assertions)` — runs sync assertions on the `TResult`
- `ITestHarnessBuilder`
    - `WithDependency<TService>(TService instance)` — register concrete instance local to harness
    - `WithMock<TService>(Action<IMockBuilder<TService>> configure)` — configure a mock with the built-in mock engine (alpha: minimal stub)
    - `Replace<TService>(TService instance)` — explicit replace semantics
    - `Build()` — returns `ITestHarness` (private/impl type or small resolver; for alpha, a lightweight resolver local to harness)
- `IMockBuilder<TService>` — DSL:
    - `Given(Expression<Func<TService, object>> call)` — identify method/property call to stub
    - `ThenReturns(object result)` — return specified value
    - `ThenThrows(Exception ex)` — throw when called
- `IResultAssertions<TResult>`
    - `NotBeNull()`
    - `HaveValue(TResult expected)`
    - `With<TProperty>(Expression<Func<TResult, TProperty>> selector, TProperty expected)`
    - `And` chaining

**Internal type `ITestHarness` (impl-only):**
- Small, per-test service registry that maps service type → instance (singleton-in-test). Not a full DI container.

### Default Behaviors / Conventions

- **SUT resolution**: Prefer harness-registered instance. Otherwise, construct via reflection if constructor parameters are all registered. Otherwise, throw descriptive exception.
- **Mocks**: Tiny, internal mock engine: maps expressions to behaviors, returns configured values or throws. Provide simple `Verify` method on mock builder (not enforced).
- **Contexts**: Contextual data via `WithDependency<TContext>(contextInstance)` (no typed IContext in alpha).

### Implementation Tasks (Alpha)

1. **Create abstractions project.**  
   - Add interfaces above, with XML docs.

2. **Implement harness (Impl project).**  
   - Implement `TestScenario<THandler, TInput, TResult>` factory.
   - Implement `TestHarnessBuilder` with internal dictionary `Type → object`.
   - `Build()` returns `TestHarness` (internal), which provides `Resolve<T>() : T`.

3. **Implement small mock engine.**  
   - Minimal expression parser, store behaviors in a dictionary.

4. **Assertions implementation.**  
   - Implement `ResultAssertions<TResult>` using test-runner-friendly approach.

5. **Example tests and docs.**  
   - Show Arrange/Act/Assert flows with dependencies and mocks.

6. **Unit tests for the library itself.**  
   - Test harness build/dispose, SUT resolution, mock DSL, assertions.

### CI / Packaging (Alpha)

- Single library package `Zentient.Testing` (Abstractions + Impl).
- CI steps: run unit tests, build NuGet package (`0.1.0-alpha.*`), push to feed.

### Acceptance Criteria (Alpha)

- `ITestScenario` flows compile/run without external DI dependencies.
- Mocks can be configured to return/throw.
- SUT resolution works for simple constructors; failure case includes actionable error.

---

## BETA (0.2.x) — Add Zentient.DependencyInjection Integration, Adapters & Basic Diagnostics

**Goal**: Introduce the real DI surface (via adapters), enable DI diagnostics in tests.

**Precondition**: Zentient.DependencyInjection available as an upstream package (preferred) or create an internal shim.

### Implementer Surface

**Abstractions (augmented):**
- Keep `Zentient.Testing.Abstractions` stable from alpha.
- Add small interfaces/types referencing Zentient DI contracts (do not duplicate types; reference them).
- `ITestHarnessBuilder` now gains:
    - `ConfigureServices(Action<IServiceRegistrationBuilder> configure)`
    - `WithContext<TContextDefinition>(IContext<TContextDefinition> context)`
- `ITestHarnessBuilder.Build()` returns `IServiceResolver` or a thin wrapper.

**Add `IMockAdapter` abstraction:**
- `CreateMock<TService>() : IMockBuilder<TService>`
- `GetInstance<TService>(IMockBuilder<TService> builder) : object`

**Hosting adapters:**
- Implement `Zentient.Testing.Hosting.MicrosoftDi`:
    - Maps harness builder calls to `IServiceRegistrationBuilder`.
    - `Build()` materializes an `IServiceResolver` that wraps the container.
- Optionally implement a simple container for minimal DI semantics.

**Mock adapters:**
- `Zentient.Testing.Adapters.Moq`:
    - Implements `IMockAdapter` using Moq.
    - Provide `UseMoq()` extension method.

**Diagnostics & validation:**
- Integrate `IServiceValidator` from Zentient.DependencyInjection.
- Expose `IDiagnosticsAssertions` to allow test assertions on DI graph.
- Harness can run validation automatically during `Build()` if enabled.

### Implementation Tasks (Beta)

1. Reference Zentient.DependencyInjection contracts or provide a temporary shim.
2. Extend `HarnessBuilder` to use `IContainerBuilder`.
3. Implement Microsoft DI adapter.
4. Implement Moq adapter.
5. Add diagnostics layer (`ITestHarnessBuilder.Diagnostics`, `IDiagnosticsAssertions`).
6. Create integration tests (harness + adapters + diagnostics).

### CI / Packaging (Beta)

- Split packages: `Zentient.Testing`, `Zentient.Testing.Adapters.Moq`, `Zentient.Testing.Hosting.MicrosoftDi`.
- CI: unit/integration tests, build sample projects, package artifacts, publish to preview feed.

### Acceptance Criteria (Beta)

- `ITestScenario` builds/uses real DI via adapter.
- Mocks created by adapter are injected.
- Diagnostics endpoints provide registration maps and validations.
- Example repo demonstrates typed context, mock, and DI-hosted SUT.

---

## RELEASE CANDIDATE / STABILIZATION (0.9.x) — Analyzers, Strong Diagnostics, Ergonomics

**Goal**: Stabilize API, add Roslyn analyzers, advanced diagnostics, and developer ergonomics.

### Implementer Surface

- **API finalization**: Freeze public API, document compatibility rules.
- **Advanced diagnostics & statistics**:
    - `IRegistrationStatistics` (counts by lifetime/type).
    - `IDependencyGraphAnalysis` (topological ordering, cycles, depth).
    - Harness `Snapshot()` and `Diff(snapshot)` features.
- **Harness options**: `AutoValidateOnBuild`, `ValidateOnAct`, configurable via static config or builder.
- **Roslyn Analyzers**: (in `Zentient.Testing.Analyzers`)
    - Detect missing await, missing context, mocks after Act, DI lifetime mismatches.
    - Provide code fixes.
- **Performance & resiliency**: Resolver performance for many tests, harden disposal semantics.
- **Documentation & examples**: Cookbook, migration notes, best practices, sample repo (xUnit & NUnit).

### Implementation Tasks (RC)

1. Finalize API, XML docs.
2. Implement statistics and snapshot/diff.
3. Build/test analyzers and code-fix providers.
4. Cross-platform CI.
5. Performance benchmarks.

### CI / Packaging (RC)

- Pack/sign packages.
- Staged release channel.
- Publish analyzers separately.

### Acceptance Criteria (RC)

- Public API is frozen.
- Analyzers detect/fix patterns.
- Diagnostics reliable; snapshot/diff accurate.
- Cross-platform CI green.

---

## STABLE (1.0.0) — Production-Ready, Full Alignment with Zentient.DependencyInjection

**Goal**: Deliver a robust, mature, and extensible testing ecosystem.

### Implementer Surface

- **Final features**:
    - Full set of adapters: Moq, NSubstitute, FakeItEasy, etc.
    - Hosting adapters: Microsoft DI, Autofac, etc.
    - Decorators/interceptors: `WithDecorator<TService, TDecorator>()`, `WithInterceptor<TService, TInterceptor>()`.
    - Advanced metadata assertions.
    - Optional runtime UI/console diagnostics for failing tests.
- **Security & maintenance**: Harden dependencies, provide LTS policy, SemVer commitment.
- **Community & extensibility**: Contributor guide, extension API docs, template repo.

### Implementation Tasks (1.0)

1. Implement/publish all hosting and mocking adapters.
2. Add decorator/interceptor APIs.
3. Polish docs/changelogs/guides.
4. Coordinate release of Zentient.DependencyInjection stable.

### CI / Packaging (1.0)

- Semantic versioning, release notes.
- Multi-targeting to .NET LTS.
- Single-using experience: recommend `Zentient.Testing` and `TestScenario.For<...>()`.

### Acceptance Criteria (1.0)

- Consumers can write tests for complex DI graphs, decorators, interceptors with minimal ceremony.
- All analyzers/diagnostics are stable.
- Clean package graph for enterprise.

---

## Per-Stage Testing Matrix

- **Alpha**: Harness dictionary semantics, SUT resolution, mock engine, integration flows.
- **Beta**: All alpha tests, container-backed harness, diagnostics, contract tests.
- **RC**: Analyzer unit tests, snapshot/diff, performance.
- **1.0**: Multi-target integration, interoperability, security/fuzz.

---

## Developer Ergonomics & Examples

Implement and test these example patterns:

1. **Simple unit handler (alpha)**: WithDependency for repository, WithMock for service, ActAsync, assert result.
2. **DI-backed handler with context (beta)**: Typed context registration, `UseMicrosoftDi`, assert context/repository in handler.
3. **Mock injection via adapter (beta)**: WithMock using adapter DSL, build harness, resolve SUT, run assert.
4. **DI validation use-case (beta/RC)**: Create circular dependency, call ValidateDi, assert failure.

---

## Backwards Compatibility & Migration Strategy

- **Alpha → Beta**: Keep alpha APIs. Add new methods as optional, provide compatibility shims.
- **Beta → RC**: Freeze API, communicate deprecation, provide codemods.
- **RC → 1.0**: Only opt-in enhancements or critical fixes.

---

## Documentation and Onboarding

- **Each stage**: Publish quickstart, cookbook, adapter guide, diagnostics reference, migration guide.
- **Example projects**: xUnit & NUnit, aligned with docs.

---

## CI/CD & Release Process

- **Alpha**: Unit tests → package → preview feed.
- **Beta**: Unit + integration tests, semantic pre-release, preview feed, RC artifacts.
- **RC**: Full test matrix, analyzer tests, performance, sign/tag.
- **Release**: Coordinate with Zentient.DependencyInjection, publish 1.0.0.

---

## Security, Licensing, and Contribution

- MIT license (or agreed OSS); document CLA if needed.
- Pin third-party dependencies, scan with SCA tools.
- Accept adapter contributions, template PR checks.

---

## Roadmap Summary

- **0.1.x (Alpha)**: Core DSL, harness, tiny mock engine, example repo.
- **0.2.x (Beta)**: DI integration, Moq adapter, diagnostics, validation.
- **0.9.x (RC)**: Analyzers, snapshots, stats, docs.
- **1.0.0**: Multiple adapters, decorators/interceptors, polished diagnostics, full alignment with Zentient.DependencyInjection stable.

---