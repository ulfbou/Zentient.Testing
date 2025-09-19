Zentient.Testing — Alfa Release (v0.1.0-alfa)

Overview
This is the initial alfa release of Zentient.Testing. It provides a testing harness, lightweight mock engine and a set of assertions to support testing of handler-style code and expression evaluation.

Highlights
- TestHarnessBuilder and TestHarness for orchestrating scenarios
- MockEngine for dynamic mock/proxy generation
- ResultAssertions for clearer failure messages
- CI/CD pipeline for packaging and GitHub Releases

Breaking changes / migration
- API is unstable and expected to change; expect breaking changes between alfa -> beta.
- If you depend on public interfaces, add integration tests and pin the package version.

Known issues
- Documentation generation is limited in this release. See [DocFX workflow](../../.github/workflows/docfx.yml).
- Some advanced mocking scenarios may not be supported yet. See [API Reference](api-reference-alfa.md) for current capabilities.

Installation
- Install the package from the pre-release NuGet feed or from the GitHub release assets.

Contact
- File issues on the repository for bugs or feature requests.

---

For detailed API surface and examples see the internal [API Reference](api-reference-alfa.md) or the public [api.md](/docs/api.md).