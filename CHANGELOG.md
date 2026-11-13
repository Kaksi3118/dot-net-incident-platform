# Changelog

All notable changes to OpsRelay will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

_No unreleased changes yet._

---

## [0.3.0] - 2026-11-12

SLA calculation engine, incident state machine and transactional outbox.

### Added
- Domain-level `ISlaCalculator` Strategy interface and `SlaCalculationResult`
  record with response/resolution deadlines and strategy metadata.
- `StandardBusinessHoursSlaCalculator` for Low/Medium priorities
  (Monday-Friday, 09:00-17:00 UTC) with night and weekend rollover.
- `Critical24x7SlaCalculator` for High/Critical priorities with continuous
  calendar-time calculation.
- `SlaCalculatorFactory` resolving the correct strategy from DI.
- `IncidentStateMachine` with a 36-transition lifecycle matrix.
- `InvalidStatusTransitionException` for guarded status changes.
- `IDomainEventDispatcher` and `MediatRDomainEventDispatcher` using typed
  MediatR notifications.
- Transactional outbox integration in `UnitOfWork`: domain events are persisted
  as JSON `OutboxMessages` in the same transaction, cleared, then dispatched.
- 99 xUnit tests covering SLA strategies, the full state matrix, aggregate
  invariants, handlers, validators, behaviors, mappings and the dispatcher.
- `docs/coverage-report.md` with class-level coverage evidence.

### Changed
- `CreateIncidentCommandHandler` now selects a strategy through
  `ISlaCalculatorFactory` and stores the calculated resolution deadline.
- `Incident.ChangeStatus` enforces the state machine and uses `TimeProvider`;
  reopening a resolved incident clears `ResolvedAt`.
- `UnitOfWork.CommitAsync` writes outbox messages and dispatches domain events
  after the transaction commits.

### Fixed
- Invalid lifecycle transitions now surface as RFC 7807 `400` responses instead
  of being silently accepted.

### Performance
- Unit suite runs in under one second with no external dependencies.
- Domain/Application line coverage is 94.68% (gate: 80%).

---

## [0.2.0] - 2026-11-12

.NET 8 LTS and C# 12 modernization of the OpsRelay backend.

### Added
- `Directory.Build.props` centralizing `net8.0`, C# 12, nullable, implicit
  usings, warnings-as-errors and recommended analyzers.
- Layer-specific DI extensions `AddApplicationServices`,
  `AddInfrastructureServices` and `AddApiServices`.
- `BulkUpdateStatusAsync` using EF Core 8 `ExecuteUpdateAsync` to bypass the
  change tracker for bulk status changes.
- `OutboxMessages` table with EF Core 8 JSON-owned metadata mapping.
- `FakeTimeProvider` test helper for deterministic time travel.
- ADR-001 migration decision record, step-by-step migration playbook and an
  enterprise pull request template.

### Changed
- **BREAKING**: target framework upgraded from `net6.0` to `net8.0`; SDK pinned
  to `8.0.428`; language version raised to C# 12.
- **BREAKING**: `Startup.cs` replaced by minimal hosting in `Program.cs`.
- EF Core upgraded 6.0.25 to 8.0.2 with global split-query behavior.
- MediatR upgraded 11.1.0 to 12.2.0, including assembly registration and
  non-generic handler return type changes.
- FluentValidation upgraded to 11.9.0, Serilog.AspNetCore to 8.0.1, xunit to
  2.6.6 and NSubstitute to 5.1.0.
- Handlers, repositories, `UnitOfWork` and `IncidentsController` use C# 12
  primary constructors.
- Collection initialization uses C# 12 collection expressions.
- Business and persistence code uses `TimeProvider.GetUtcNow()` instead of
  ambient `DateTimeOffset.UtcNow`.

### Removed
- `Startup.cs`.
- `MediatR.Extensions.Microsoft.DependencyInjection` package (merged into
  MediatR 12).
- Ambient time calls from business and persistence logic.
- `new List<T>()` and `Array.Empty<T>()` in refactored paths.

### Fixed
- Enum values (`Priority`, `IncidentStatus`) are serialized as strings to match
  the documented API/client contract, while integer enum input remains
  accepted.
- Validation error messages are pinned to `en-US` for deterministic responses
  independent of server culture.
- `GET /api/v1/incidents/{id}` returns RFC 7807 `application/problem+json` when
  the incident is missing.
- Production Serilog configuration keeps information-level request logging
  enabled while suppressing framework noise.

### Performance
- Global `QuerySplittingBehavior.SplitQuery` prevents Cartesian product
  explosion on multi-collection queries.
- Bulk status updates execute as a single direct `UPDATE` with zero tracked
  entities.
- Outbox metadata is stored in a JSON column, eliminating a metadata join.

---

## [0.1.0] - 2026-11-11

Initial .NET 6 Clean Architecture baseline release.

### Added

#### Solution and build foundation
- `OpsRelay.sln` containing six projects split across `src/Backend` and `tests`.
- `.NET 6` target framework, nullable reference types, implicit usings and
  warnings-as-errors across all projects.
- `global.json` pinned to SDK `6.0.428` for the legacy baseline.
- Clean Architecture project references enforcing dependency flow:
  `Domain -> Application -> Infrastructure -> Api`.

#### Domain layer
- `Incident` aggregate root with SLA deadline, optimistic-concurrency
  `RowVersion`, lifecycle status and domain-event collection.
- `SlaPolicy` entity describing response and resolution targets per priority.
- `Priority` and `IncidentStatus` enums.
- `IncidentTitle` and `TeamId` immutable value objects with validation and
  value equality.
- Domain events: `IncidentCreatedDomainEvent`,
  `IncidentStatusChangedDomainEvent` and `SlaBreachedDomainEvent`.
- Repository contracts `IIncidentRepository` and `ISlaPolicyRepository`.

#### Infrastructure layer
- EF Core 6 `OpsRelayDbContext` with SQL Server provider.
- Fluent API configurations for `Incident` and `SlaPolicy`.
- `rowversion` concurrency token and shadow `CreatedAt` / `UpdatedAt` audit
  properties populated automatically in `SaveChangesAsync`.
- Seeded SLA policies for Low, Medium, High and Critical priorities.
- Repository implementations plus `UnitOfWork`.
- Initial migration `20261111000000_InitialCreate`, verified to generate valid
  SQL Server DDL.

#### Application layer
- MediatR CQRS commands and queries for incident creation, status updates,
  lookup by id and paginated listing.
- `ValidationBehavior` and `LoggingBehavior` MediatR pipeline behaviors.
- FluentValidation validators for create and status-update commands.
- DTOs, paginated result model and mapping extensions.
- `ISlaCalculator` and `IUnitOfWork` abstractions with a baseline SLA
  calculator implementation.

#### API layer
- Classic ASP.NET Core 6 two-phase `Startup.cs` initialization with
  `Program.cs` host bootstrap.
- Versioned `IncidentsController` at `/api/v1/incidents`:
  `GET`, `GET {id}`, `POST`, `PATCH {id}/status`.
- RFC 7807 `ProblemDetails` exception middleware for validation, domain,
  not-found and unhandled errors.
- Serilog structured JSON logging to console and rolling files with
  correlation ids, request path, status code and elapsed time.
- SQL Server and `DbContext` health checks exposed at `/health`.
- Swagger UI enabled in development.

### Security
- Non-development connection strings use a placeholder password token that is
  intended to be overridden through environment variables or secrets.

---

[Unreleased]: https://github.com/Kaksi3118/dot-net-incident-platform/compare/v0.3.0...HEAD
[0.3.0]: https://github.com/Kaksi3118/dot-net-incident-platform/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/Kaksi3118/dot-net-incident-platform/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/Kaksi3118/dot-net-incident-platform/releases/tag/v0.1.0
