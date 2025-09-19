# Zentient.Testing — Internal Documentation Landing

Welcome to the internal documentation for the alfa release of Zentient.Testing. This landing page provides quick links to internal reference material, release notes and contributor guidance intended for maintainers and early adopters.

> Note: files in docs/internal are intended for maintainers and may contain design notes or unstable API details. Do not publish these pages to public sites without reviewing sensitive content.

---

## Quick links

- [API Reference (alfa)](api-reference-alfa.md) — full surface-area snapshot for v0.1.0-alfa
- [Roadmap](roadmap.md) — short- and medium-term roadmap and milestones
- [Release notes — alfa](release-notes-alfa.md) — what's included and migration guidance
- [Contributing](contributing.md) — contributor workflow, commit conventions and branch guidance

---

## Aside / Navigation

<aside>
- **Documentation**
  - [Landing](index.md)
  - [API Reference (alfa)](api-reference-alfa.md)
  - [Release notes — alfa](release-notes-alfa.md)
  - [Roadmap](roadmap.md)
  - [Contributing](contributing.md)

- **Repository**
  - [Code (src/)](../../src/)
  - [Tests (tests/)](../../tests/)
</aside>

---

## Getting started (for maintainers)

1. Read the API reference to understand exported interfaces and expectations for consumers.
2. Review the release notes for known issues, migration advice, and packaging notes.
3. Follow the contributing guide for branch naming, commit messages and CI expectations.

---

<footer>
**Maintainers:** please keep this directory up to date. For questions or to propose changes to the internal docs, open an issue with the `docs/internal` label.

**License:** content in this repository follows the repository license (see LICENSE at the root).
</footer>

---

# Zentient.Testing — Internal Documentation

This directory contains the internal wiki pages and documentation intended for maintainers, contributors and early adopters of the alfa release of Zentient.Testing.

## Files
- [api-reference-alfa.md](api-reference-alfa.md) — full API reference for the alfa release (surface area & examples)
- [roadmap.md](roadmap.md) — short- and medium-term roadmap and release plan
- [release-notes-alfa.md](release-notes-alfa.md) — alfa release notes and migration guidance
- [contributing.md](contributing.md) — contributor guidance, development workflow and commit conventions

## Internal wiki (maintainer-facing)
- [index.md](wiki/index.md) — Overview & introduction
- [wiki/getting-started.md](wiki/getting-started.md) — Quick start and running tests locally
- [wiki/concepts.md](wiki/concepts.md) — Core concepts and mental model
- [wiki/architecture.md](wiki/architecture.md) — High level architecture and extension points
- [wiki/usage-examples.md](wiki/usage-examples.md) — Recipes and example patterns

## How to use
- These pages are written for Markdown-driven hosting (GitHub Pages / Docs). Keep them up to date when you change public APIs.
- API reference is intentionally compact for the alfa; expand signatures and examples as APIs stabilize.

## Contact
- For questions about the design or release process open an issue or ping the maintainers in the repository.

## Notes
- These files are internal and should not be published publicly without review. They remain local to maintainers and are intentionally kept out of public releases.
