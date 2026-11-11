# Changelog

All notable changes to OpsRelay will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Project folder scaffolding (directory structure, .gitignore, .editorconfig)
- AGENTS.md autonomous agent context file
- build-state.md task progress tracker

---

## [0.1.0] — Planned

### Added — PHASE-01: Baseline Setup & .NET 6 Legacy Backend
- Solution file `OpsRelay.sln` with 6 Clean Architecture projects targeting `net6.0`
- Domain layer: `Incident` aggregate, `SlaPolicy` entity, enums, value objects, domain events
- Infrastructure layer: EF Core 6 `OpsRelayDbContext`, Fluent API mappings, repositories, initial migration
- Application layer: MediatR CQRS, commands/queries, `FluentValidation` pipeline behavior, `ISlaCalculator` interface
- API layer: .NET 6 `Startup.cs`, `IncidentsController`, RFC 7807 ProblemDetails middleware
- Serilog logging, `/health` endpoint, Swashbuckle OpenAPI

---

## [0.2.0] — Planned

### Changed — PHASE-02: .NET 6 → .NET 8 Modernization
- **BREAKING**: Target framework upgraded from `net6.0` to `net8.0` across all projects
- **BREAKING**: `Startup.cs` dissolved into `Program.cs` with top-level statements
- Refactored all services to C# 12 primary constructors
- Replaced legacy collection syntax with C# 12 collection expressions `[]`
- Replaced `DateTime.UtcNow` with `System.TimeProvider.GetUtcNow()` throughout
- Upgraded EF Core 6 → 8: split queries, `ExecuteUpdateAsync`, JSON column support
- Upgraded MediatR 11 → 12, FluentValidation 11, Swashbuckle → Scalar

### Added
- `AddApplicationServices`, `AddInfrastructureServices`, `AddApiServices` extension methods
- `FakeTimeProvider` for deterministic unit test clock control
- ADR-001: .NET 8 Migration Case Study

---

## [0.3.0] — Planned

### Added — PHASE-03: SLA Calculation Engine & Domain Logic
- `ISlaCalculator` strategy interface with `SlaCalculationResult`
- `StandardBusinessHoursSlaCalculator` (Mon–Fri, 09:00–17:00 UTC, skips weekends)
- `Critical24x7SlaCalculator` (calendar-time for High/Critical priority)
- `SlaCalculatorFactory` resolving strategies from DI
- `IncidentStateMachine` with guard clauses and transition validation
- Domain event dispatcher with Transactional Outbox pattern
- xUnit unit tests: SLA calculators, state machine, boundary conditions
- Code coverage gate ≥80%

---

## [1.0.0] — Planned

### Added — PHASE-04 & PHASE-05
- Angular 17 standalone frontend with Signals, Angular Material, Tailwind CSS
- `IncidentListComponent` (server-side pagination, MatTable, signals)
- `SlaBadgeComponent` (computed SLA countdown with color transitions)
- `IncidentDetailComponent` + `StatusTransitionDialogComponent` (Typed Reactive Forms)
- WebApplicationFactory integration tests with Testcontainers.MsSql
- Multi-stage `Dockerfile` (Alpine, non-root user)
- `docker-compose.yml` with SQL Server health checks
- `README.md` with Mermaid architecture diagrams and Migration Case Study
- ADR-001 through ADR-004
- GitHub Actions CI workflow (build/test/coverage/docker)
- Release branch `release/v1.0.0` merged to `main`
