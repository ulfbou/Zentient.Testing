# 🧾 CHANGELOG — Zentient.Testing

> All notable changes to this package will be documented in this file.  
> This project adheres to [Semantic Versioning](https://semver.org).

---

## 📦 [2.0.0] — 2025-07-17

💥 **Major Overhaul & API Refinement**

> This version introduces a significantly expanded and refined set of core abstractions, enhancing flexibility, consistency, and support for advanced patterns like hierarchical contexts, structured errors, and composable policies. It includes breaking changes to core envelope contracts to improve protocol-agnosticism.

---

### ✨ Added — Core Contracts & Factories

| Interface           | Description                                                                                             |
|---------------------|---------------------------------------------------------------------------------------------------------|
| `ICode`             | Universal, protocol-agnostic code for operation outcomes with symbolic name and optional numeric value. |
| `ICodeFactory`      | Factory for creating `ICode` instances.                                                                 |
| `IContextFactory`   | Factory for creating root `IContext` instances and managing ambient context.                            |
| `IErrorInfo`        | Detailed, immutable information about an error, including category, code, message, and nested errors.   |
| `IErrorInfoFactory` | Factory for creating `IErrorInfo` instances, including from exceptions.                                 |
| `IEnvelopeFactory`  | Factory for creating various `IEnvelope` types (generic, headered, streamable).                         |
| `IHeaderedEnvelope` | Base contract for envelopes supporting arbitrary key-value headers (protocol-agnostic).                 |
| `IHeaderedEnvelope<TValue>` | Strongly-typed headered envelope for carrying operation result payloads.                        |
| `IMetadataFactory`  | Factory for creating `IMetadata` instances.                                                             |
| `IPolicyFactory`    | Factory for creating common policy instances (retry, circuit breaker, fallback).                        |
| `IStreamableEnvelope<TStream>` | Base contract for envelopes whose payload can be streamed.                                      |
| `IStreamableEnvelope<TStream, TValue>` | Strongly-typed streamable envelope containing both a stream payload and a value.        |

---

### ♻️ Changed — Core Contracts & Refinements

* **`IContext`**:
    * Added `Id`, `Parent`, `CorrelationId` properties for enhanced traceability and hierarchy.
    * `Metadata` property changed from nullable (`IMetadata?`) to non-nullable (`IMetadata`).
    * Introduced `CreateChild`, `WithMetadata`, `WithType`, and `Merge` methods for immutable context manipulation.
* **`IEnvelope`**:
    * `Code` property type changed from `IEndpointCode?` to the more general `ICode?`.
    * `Messages` property changed from nullable (`IReadOnlyCollection<string>?`) to non-nullable (`IReadOnlyCollection<string>`).
    * `Metadata` property changed from nullable (`IMetadata?`) to non-nullable (`IMetadata`).
    * Added new `Errors` property (`IReadOnlyCollection<IErrorInfo>`) for structured error reporting.
* **`IEnvelopeFactory`**:
    * Removed `isSuccess` parameter from `CreateEnvelope` methods, as success is now implicitly determined by the absence of errors.
* **`IPolicy<T>`**:
    * Renamed the `With` method to `CombineWith` for clearer semantic meaning when chaining policies.
    * Added `Fallback` method for defining fallback execution paths.
* **`ErrorCategory`**:
    * Relocated from `Zentient.Results` to `Zentient.Testing` for broader applicability.
    * Updated XML documentation to remove specific references to `ErrorInfo` properties, maintaining abstraction.
* **`IEnvelope<out TValue>`**:
    * Renamed from `IEnvelope2<out TValue>` to simplify the interface name.

---

### 🗑️ Removed — Deprecated Contracts

* `IEndpointCode`: Replaced by the more general and flexible `ICode` interface.

---

### 🛠️ Build & CI/CD

* **Build Properties (`Directory.Build.props`)**:
    * Enabled documentation file generation conditionally for non-test projects.
    * Removed redundant analyzer and StyleCop configurations.
    * Cleaned up unnecessary comments and simplified `SolutionDir` definition.
* **Package Targets (`Directory.Pack.targets`)**:
    * Removed duplicate package metadata properties, centralizing configuration.
* **Project File (`Zentient.Testing.csproj`)**:
    * Configured inclusion of `README.md` in the NuGet package.
    * Corrected the path for `AssemblyOriginatorKeyFile`.
* **Strong-Name Key (`Zentient.snk`)**: Added the strong-name key file to the repository.
* **CI/CD Workflow (`.github/workflows/release.yml`)**:
    * Implemented a comprehensive GitHub Actions pipeline for automated build, pack, and release.
    * Includes semantic versioning via GitVersion, artifact management, and automated publishing to NuGet.org and GitHub Releases.
    * Features robust release notes extraction from `CHANGELOG.md`.

---

### 📘 Documentation

* Extensive updates and additions to XML documentation for all new and modified interfaces, properties, and methods, ensuring clarity and adherence to the Zentient Framework's documentation standards.

---

> 🔖 This major release significantly enhances the foundational abstraction layer of the Zentient ecosystem, providing more powerful, consistent, and protocol-agnostic building blocks for modern distributed applications.

---

## 📦 [1.0.0] — 2025-07-15

🎉 **Initial Public Release**

> This version introduces the foundational, protocol-agnostic abstractions that underpin the Zentient Framework’s execution, context, envelope, and policy models.

---

### ✨ Added — Core Contracts

| Interface        | Description                                                                 |
|------------------|-----------------------------------------------------------------------------|
| `IContext`       | Immutable hierarchical runtime context with identity, correlation, and metadata. |
| `IMetadata`      | Immutable, strongly-typed key-value container for contextual metadata.      |
| `IEnvelope`      | Base contract for operation outcome encapsulation (success, error, metadata). |
| `IEnvelope<T>`   | Strongly-typed envelope for carrying operation result payloads.             |
| `IEndpointCode`  | Protocol-aware symbolic outcome code with name, numeric value, and protocol. |
| `IFormatter<TIn, TOut>` | Transform abstraction for value formatting across layers and transports. |
| `IPolicy<T>`     | Composable policy abstraction for encapsulating cross-cutting operational logic (e.g., retry, fallback). |

---

### 🧱 Added — Supporting Types

- **`Unit`**  
  Functional unit type representing an intentional absence of value, suitable for generic APIs and void-like operations.

- **`UnitJsonConverter`**  
  Custom System.Text.Json converter for proper serialization and deserialization of `Unit`.

---

### 📘 Documentation

- Full XML documentation added for all public contracts, properties, and methods.
- Follows the Zentient Framework documentation structure and developer-first principles.

---

### 🛡 Licensing

Released under copyright © 2025  
**Zentient Framework Team**

---

> 🔖 This release establishes the foundational abstraction layer for the Zentient ecosystem.  
> Designed for maximum composability, immutability, protocol-agnosticism, and testability across distributed applications.
