Contributing to Zentient.Testing

Welcome! We appreciate contributions. This document outlines the development workflow and expectations.

Commit conventions
- Use Conventional Commits: type(scope): subject
- Types: feat, fix, docs, style, refactor, perf, test, chore, ci

Branching
- main — default branch for releases
- develop — ongoing work and integration
- feature/*, fix/* — topic branches

PR process
- Open PRs against develop
- Include tests and update docs for user-visible changes
- CI must pass (build + tests) before merging

Running tests locally
- dotnet build
- dotnet test

Code style
- Use consistent formatting (dotnet format)
- Document public APIs with XML comments

Issues & templates
- Use issue templates for bug reports and feature requests

Maintainers
- Mention maintainers in issue comments or ping on PRs for review.