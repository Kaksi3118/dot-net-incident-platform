# Changelog

All notable changes to OpsRelay will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed
- Enum values (`Priority`, `IncidentStatus`) are now serialized as strings to
  match the documented API/client contract, while integer enum input remains
  accepted.
- Validation error messages are pinned to `en-US` for deterministic API
  responses independent of server culture.
- `GET /api/v1/incidents/{id}` now returns RFC 7807
  `application/problem+json` when the incident is missing.
- Production Serilog configuration keeps information-level request logging
  enabled while suppressing framework noise.

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

[Unreleased]: https://github.com/opsrelay/opsrelay/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/opsrelay/opsrelay/releases/tag/v0.1.0
