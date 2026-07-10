# Copilot Instructions — Infinity Toolkit Messaging

This repository ships .NET messaging packages centered around a shared `IMessageBus` abstraction, with in-memory and Azure Service Bus broker implementations.

## Build, test, and lint

```powershell
# Restore/build
dotnet restore
dotnet build
dotnet build --configuration Release --no-restore

# Test (full suite / single test project)
dotnet test
dotnet test tests/Infinity.Toolkit.Messaging.Tests/Infinity.Toolkit.Messaging.Tests.csproj

# Alternative test execution (Microsoft Testing Platform executable test project)
dotnet run --project tests/Infinity.Toolkit.Messaging.Tests/Infinity.Toolkit.Messaging.Tests.csproj -- --disable-logo
dotnet run --project tests/Infinity.Toolkit.Messaging.Tests/Infinity.Toolkit.Messaging.Tests.csproj --configuration Release -- --disable-logo

# Run a single test (TUnit UID-based filter)
dotnet test --project tests/Infinity.Toolkit.Messaging.Tests/Infinity.Toolkit.Messaging.Tests.csproj -- --filter-uid "Infinity.Toolkit.Messaging.Tests.MessageBusBuilderTests.1.1.AddBroker_Should_Succeed.1.1.0"

# Discover test names/UIDs
dotnet test --project tests/Infinity.Toolkit.Messaging.Tests/Infinity.Toolkit.Messaging.Tests.csproj -- --list-tests --diagnostic

# Pack NuGet packages (output from Directory.Build.props: ./artifacts)
dotnet pack --configuration Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
```

- Solution file: `Infinity.Toolkit.Messaging.slnx`
- CI baseline (see `.github/workflows/nuget-publish.yml`): `dotnet build --configuration Release --no-restore` and `dotnet test --configuration Release --no-build`
- There is no separate lint command; analyzer/style enforcement happens through `dotnet build` with warnings treated as errors (`Directory.Build.props`).
- Because tests run on Microsoft Testing Platform, `dotnet run` from the test project is also a valid way to execute tests.

## High-level architecture

### Package layout

| Package | Role |
|---|---|
| `src/Infinity.Toolkit.Messaging` | Core abstractions, DI setup, hosted lifecycle, in-memory broker, diagnostics/OpenTelemetry integration |
| `src/Infinity.Toolkit.Messaging.AzureServiceBus` | Azure Service Bus broker + producers/consumers implementing the same core abstractions |
| `tests/Infinity.Toolkit.Messaging.Tests` | Single consolidated test project covering core and in-memory behavior |
| `samples/MessagingSample` | Reference wiring for `AddInfinityMessaging().ConfigureInMemoryBus(...).MapMessageHandler(...)` |

### Runtime flow (cross-file)

1. `AddInfinityMessaging(...)` registers defaults (`IMessageBus`, `MessagingExceptionHandler`, metrics, and `MessageBusBackgroundService`) in `ServiceCollectionExtensions.cs`.
2. Broker setup happens through builder extensions:
   - `ConfigureInMemoryBus(...)` (`InMemory/MessageBusBuilderExtensions.cs`)
   - `ConfigureAzureServiceBus(...)` (`AzureServiceBus/MessageBusBuilderExtensions.cs`)
3. Channel producers/consumers are registered on broker-specific builders (`InMemoryBusBuilderExtensions.cs`, `AzureServiceBusBuilderExtensions.cs`) and indexed in each broker’s `ChannelConsumerRegistry`.
4. `MessageBusBackgroundService` initializes brokers and starts listening after host startup, honoring `MessageBusOptions.AutoStartListening` and `AutoStartDelay`.
5. `MessageBus` orchestrates all registered `IBroker` instances and can start/stop all brokers or a single broker.
6. Broker implementations (`InMemoryBus`, `AzureServiceBusBroker`) create channel processors, resolve `IMessageHandler<TMessage>` from DI, deserialize payloads, invoke handlers, and record diagnostics/metrics.

## Key conventions

### Configuration and option binding

- Core options bind from `Infinity:Messaging` (`ServiceCollectionExtensions.cs`).
- Broker option roots:
  - `Infinity:Messaging:InMemoryBus`
  - `Infinity:Messaging:AzureServiceBus`
- Consumer/producer options are keyed by message type `AssemblyQualifiedName` (or an explicit service key for keyed/raw registrations).

### Message handling model

- `MapMessageHandler<TMessage, TMessageHandler>` registers handlers as transient and rejects duplicate handler-type registration.
- A message can fan out to multiple handlers: brokers resolve `IEnumerable<IMessageHandler<TMessage>>` and invoke all.
- For keyed/raw consumers, `RequireCloudEventsTypeProperty` is explicitly disabled; typed consumers rely on CloudEvents type matching and registered event type metadata.

### Diagnostics and telemetry

- OpenTelemetry extensions are in `src/Infinity.Toolkit.Messaging/OpenTelemetry/`:
  - `AddMessagingInstrumentation(this TracerProviderBuilder ...)`
  - `AddMessagingInstrumentation(this MeterProviderBuilder ...)`
- Activity/meter naming is centralized in `Diagnostics/DiagnosticProperty.cs` (`ActivitySourceName = "Infinity.Toolkit.Messaging"`).

### Testing and code style

- Tests use TUnit + Shouldly + NSubstitute (`tests/.../Infinity.Toolkit.Messaging.Tests.csproj`).
- Test naming follows `Method_Scenario_Should_ExpectedBehavior`.
- File-scoped namespaces are mandatory (`csharp_style_namespace_declarations = file_scoped:error`).
- Global build settings from `Directory.Build.props`: `TreatWarningsAsErrors=true`, `Nullable=enable`, package output path `artifacts`, and non-packable auto-rules for projects containing `Sample` or `Test`.

### Commit messages

- Follow Conventional Commits (`feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`).
- Use imperative subject lines without trailing period.
