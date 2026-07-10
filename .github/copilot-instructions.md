# Copilot Instructions — Infinity Toolkit

A collection of NuGet packages for building modular and vertical-slice architecture applications on .NET.

## Build & Test

```powershell
# Build
dotnet build
dotnet build --configuration Release

# Test
dotnet test                                         # all tests
dotnet test tests/Infinity.Toolkit.Messaging.Tests/Infinity.Toolkit.Messaging.Tests.csproj  # single project

# Run a single test class or method
dotnet test --filter "FullyQualifiedName~ResultTests"
dotnet test --filter "DisplayName~Result_Success_Should_Be_Successful"

# Pack NuGet packages (output: ./artifacts)
dotnet pack --configuration Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
```

The solution file is `Infinity.Toolkit.Messaging.slnx` (`.slnx` format).

## Architecture

The repository is structured as independent, composable packages under `src/`:

| Package | Purpose |
|---|---|
| `Infinity.Toolkit.Messaging` | In-memory message bus, OpenTelemetry, diagnostics |
| `Infinity.Toolkit.Messaging.AzureServiceBus` | Azure Service Bus implementation of the messaging abstraction |

`tests/Infinity.Toolkit.Messaging.Tests` is the single main test project. `samples/` contains standalone runnable examples for each feature area.

## Key Conventions

### Tests

- **Framework**: xUnit v3 — use `[Fact]` and `[Theory]`
- **Assertions**: Shouldly — `result.Succeeded.ShouldBeTrue()`, `value.ShouldBe(expected)`
- **Mocking**: NSubstitute
- **Naming**: `ClassName_Scenario_Should_ExpectedBehavior`
- Global usings are declared in `Usings.cs` (`global using Xunit;`)

### Code Style (enforced via `.editorconfig`)

- **File-scoped namespaces are required** (`namespace Foo.Bar;`) — nesting is an error
- **`TreatWarningsAsErrors=true`** globally; XML doc warnings (1591) and NU1505 are suppressed
- `Nullable=enable` everywhere — no nullable-suppression without justification
- Async methods must end with `Async`
- `ImplicitUsings=enable` — no need to add `using System;` etc. manually
- Max line length: 100 characters
- Expression-bodied members: properties and accessors only (not methods)

### NuGet Packaging

`Directory.Build.props` auto-sets `IsPackable=false` for any project whose name contains "Sample" or "Test". Packable projects must have their own `<VersionPrefix>` set in the `.csproj`. Packages are output to `./artifacts/`.

When adding a new packable project, include in the `.csproj`:
```xml
<PropertyGroup>
  <VersionPrefix>0.1.0</VersionPrefix>
  <Description>One-line description for NuGet.org</Description>
</PropertyGroup>
```

## Commit messages
- Follow [Conventional Commits](https://www.conventionalcommits.org) standard.
- The commit message should be structured as follows:
  ```
  <type>[optional scope]: <description>

  [optional body]

  [optional footer(s)]
  ```
- Valid types include: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`.
- Do not end subject line with a period. Use the imperative mood ("Add feature" not "Added feature").
- Describe what and why, not how. Avoid unnecessary details about implementation.