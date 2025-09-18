# Overview — Zentient.Testing

## Mission

Zentient.Testing is a compact testing harness and scenario DSL that enables developers to write clear Arrange ? Act ? Assert tests for handler-style code. The project emphasizes developer ergonomics, small public APIs, and straightforward local validation.

## Audience

- Library authors and application developers who write small to medium-sized handler-style units (message handlers, command handlers, small services).
- Teams that prefer minimal friction and readable test code over heavyweight testing frameworks.

## What the alpha provides

- A small set of public abstractions for composing test scenarios.
- A lightweight mock engine for common stubbing scenarios.
- Assertion helpers that make test checks concise and readable.
- Local scripts to validate build/test/pack locally.

## Roadmap summary

- **Alpha (0.1.x)**: Core DSL, TestHarness, simple mock engine, and documentation.
- **Beta (0.2.x)**: Adapters for Microsoft DI and Moq, diagnostics, and richer verification.
- **Release Candidate / 1.0**: Roslyn analyzers, snapshot/diff features, final API stabilization and polished docs.

## Getting help

- Open an issue or discussion on the repository for questions, feature requests, or bug reports.
- Use the Code of Conduct when interacting with maintainers and contributors.
