# OpsRelay — Agent Build State

> This file is the **single source of truth** for autonomous agents building this project.
> Read this file at the START of every session before doing anything.
> Update the status of your task before ending your session.

---

## Project Overview

**OpsRelay** — Enterprise Incident Lifecycle & SLA Management Platform  
**Plan file**: `C:\Users\maksy\.gemini\antigravity\brain\71ef0f0f-418f-4fc3-bcb7-2dca0ca11ab5\opsrelay-plan.md`  
**Repo root**: `c:\Users\maksy\Projects\net-incident-platform`  
**Conversation**: `71ef0f0f-418f-4fc3-bcb7-2dca0ca11ab5`

---

## Environment Checklist (verify before each session)

| Tool | Required Version | Status | Install Command |
|------|-----------------|--------|-----------------|
| .NET SDK | 8.0.x (also install 6.0.x for legacy phase) | ❌ NOT INSTALLED | `winget install Microsoft.DotNet.SDK.8` |
| .NET SDK 6 | 6.0.x (for PHASE-01 baseline) | ❌ NOT INSTALLED | `winget install Microsoft.DotNet.SDK.6` |
| Node.js | 18+ (v24.20.0 ✅) | ✅ | — |
| npm | 8+ (v11.19.0 ✅) | ✅ | — |
| Angular CLI | 17.x | ❓ Check: `ng version` | `npm install -g @angular/cli@17` |
| Git | 2.x (2.55.0 ✅) | ✅ | — |
| Docker Desktop | 24+ | ❓ Check: `docker version` | https://docker.com/products/docker-desktop |
| SQL Server | via Docker only | N/A | See docker-compose.yml (PHASE-05) |

> **CRITICAL**: Install .NET SDK 8 as the very first action before any `dotnet` commands.
> Run: `winget install Microsoft.DotNet.SDK.8` then restart shell.

---

## Git Strategy

| Branch | Purpose |
|--------|---------|
| `main` | Production-ready releases only. Never commit directly. |
| `develop` | Integration branch. All feature branches merge here. |
| `feature/T{NNN}-{slug}` | One branch per task. e.g. `feature/T001-repo-init` |
| `migration/net6-to-net8` | Dedicated migration branch (PHASE-02) |
| `release/v1.0.0` | Final release branch (PHASE-05) |

**Commit format**: `type(scope): description` (Conventional Commits)  
**Tags**: `v0.1.0` (end PHASE-01), `v0.2.0` (end PHASE-02), `v0.3.0` (end PHASE-03), `v1.0.0` (end PHASE-05)

**Git config for agents**:
```bash
git config user.name "OpsRelay Bot"
git config user.email "opsrelay-bot@build.local"
```

---

## Task Progress Board

> **Legend**: ⬜ Not Started | 🔄 In Progress | ✅ Done | ❌ Blocked | ⏭ Skipped

### PHASE-01: Baseline Setup & .NET 6 Legacy Backend (target tag: v0.1.0)

| Task | Title | Branch | Status | Last Updated |
|------|-------|--------|--------|-------------|
| T001 | Repository Init, Solution & .NET 6 csproj Setup | `feature/T001-repo-init` | ✅ Done | 2026-11-11 |
| T002 | Domain Layer — Entities, Enums, Value Objects, Domain Events | `feature/T002-domain-layer` | ✅ Done | 2026-11-11 |
| T003 | Infrastructure Layer — DbContext, EF Mappings, Repositories, Migrations | `feature/T003-infrastructure` | ✅ Done | 2026-11-11 |
| T004 | Application Layer — CQRS, MediatR, FluentValidation, ISlaCalculator | `feature/T004-application-layer` | ✅ Done | 2026-11-11 |
| T005 | API Layer — .NET 6 Startup.cs, IncidentsController, ProblemDetails | `feature/T005-api-layer` | ✅ Done | 2026-11-11 |
| T006 | Configuration — appsettings, Serilog, Health Checks | `feature/T006-configuration` | ✅ Done | 2026-11-11 |
| T007 | Git Tagging Strategy & CHANGELOG.md | `develop` | ✅ Done | 2026-11-11 |
| T008 | Swagger / OpenAPI with Swashbuckle | `feature/T008-swagger` | ✅ Done | 2026-11-11 |

### PHASE-02: .NET 6 → .NET 8 Modernization (target tag: v0.2.0)

| Task | Title | Branch | Status | Last Updated |
|------|-------|--------|--------|-------------|
| T009 | Create migration branch, upgrade .csproj & NuGet packages | `migration/net6-to-net8` | ⬜ | — |
| T010 | Dissolve Startup.cs → Program.cs with extension methods | `migration/net6-to-net8` | ⬜ | — |
| T011 | C# 12 Primary Constructors refactor across all services | `migration/net6-to-net8` | ⬜ | — |
| T012 | Collection expressions — replace legacy collection syntax | `migration/net6-to-net8` | ⬜ | — |
| T013 | TimeProvider modernization — replace DateTime.UtcNow | `migration/net6-to-net8` | ⬜ | — |
| T014 | EF Core 8 upgrades — split queries, bulk ops, JSON columns | `migration/net6-to-net8` | ⬜ | — |
| T015 | PR documentation & ADR-001 | `migration/net6-to-net8` | ⬜ | — |
| T016 | Tag v0.2.0, merge migration branch, update CHANGELOG | `develop` | ⬜ | — |

### PHASE-03: SLA Calculation Engine & Domain Logic (target tag: v0.3.0)

| Task | Title | Branch | Status | Last Updated |
|------|-------|--------|--------|-------------|
| T017 | ISlaCalculator strategy interface & SlaCalculationResult | `feature/T017-sla-interface` | ⬜ | — |
| T018 | StandardBusinessHoursSlaCalculator | `feature/T018-business-hours-sla` | ⬜ | — |
| T019 | Critical24x7SlaCalculator | `feature/T019-247-sla` | ⬜ | — |
| T020 | SlaCalculatorFactory / strategy selector | `feature/T020-sla-factory` | ⬜ | — |
| T021 | IncidentStateMachine — state transitions & guard clauses | `feature/T021-state-machine` | ⬜ | — |
| T022 | Domain Event Dispatcher & Outbox pattern | `feature/T022-domain-events` | ⬜ | — |
| T023 | Unit Tests — xUnit, FluentAssertions, NSubstitute, FakeTimeProvider | `feature/T023-unit-tests` | ⬜ | — |
| T024 | Tag v0.3.0, coverage gate ≥80% documented | `develop` | ⬜ | — |

### PHASE-04: Angular 17+ Client Development

| Task | Title | Branch | Status | Last Updated |
|------|-------|--------|--------|-------------|
| T025 | Angular CLI init — standalone, strict, Material, Tailwind | `feature/T025-angular-init` | ⬜ | — |
| T026 | Core module — environment, app.config.ts, routing | `feature/T026-core-module` | ⬜ | — |
| T027 | TypeScript models mirroring backend DTOs | `feature/T027-ts-models` | ⬜ | — |
| T028 | IncidentService with strongly-typed HTTP methods | `feature/T028-incident-service` | ⬜ | — |
| T029 | Error interceptor — RFC 7807 → MatSnackBar, 503 retry | `feature/T029-error-interceptor` | ⬜ | — |
| T030 | IncidentListComponent — signals, MatTable, server-side pagination | `feature/T030-incident-list` | ⬜ | — |
| T031 | SlaBadgeComponent — computed signals, color transitions | `feature/T031-sla-badge` | ⬜ | — |
| T032 | IncidentDetailComponent + StatusTransitionDialog | `feature/T032-incident-detail` | ⬜ | — |

### PHASE-05: Integration Testing, Docker & Portfolio Polish

| Task | Title | Branch | Status | Last Updated |
|------|-------|--------|--------|-------------|
| T033 | WebApplicationFactory integration tests (full lifecycle) | `feature/T033-integration-tests` | ⬜ | — |
| T034 | Testcontainers.MsSql setup & shared fixture | `feature/T034-testcontainers` | ⬜ | — |
| T035 | Multi-stage Dockerfile (Alpine, non-root user) | `feature/T035-dockerfile` | ⬜ | — |
| T036 | docker-compose.yml — API + SQL Server + healthchecks | `feature/T036-docker-compose` | ⬜ | — |
| T037 | README.md — Mermaid diagrams, case study, quick start | `feature/T037-readme` | ⬜ | — |
| T038 | Architecture Decision Records (ADR-001 to ADR-004) | `feature/T038-adrs` | ⬜ | — |
| T039 | GitHub Actions CI workflow | `feature/T039-ci-workflow` | ⬜ | — |
| T040 | Final tag v1.0.0, release branch, CHANGELOG complete | `release/v1.0.0` | ⬜ | — |

---

## Completed Work Log

| Timestamp | Agent | Action |
|-----------|-------|--------|
| 2026-11-11 17:17 UTC | Coordinator | Project folder scaffolding created (all directories, .gitignore, .editorconfig, AGENTS.md, build-state.md, git config) |
| 2026-11-11 | OpsRelay Bot | T001 complete — OpsRelay.sln with 6 net6.0 projects, clean-architecture references, NuGet baseline, global.json pinned to SDK 6.0.428; build 0 errors / 0 warnings |
| 2026-11-11 | OpsRelay Bot | T002 complete — Incident/SlaPolicy aggregates, enums, IncidentTitle/TeamId value objects, domain events, guard-clause validation; build 0 warnings |
| 2026-11-11 | OpsRelay Bot | T003 complete — OpsRelayDbContext, fluent mappings, rowversion, shadow audits, seeded SLA policies, repositories, UnitOfWork, InitialCreate migration (SQL script verified) |
| 2026-11-11 | OpsRelay Bot | T004 complete — CQRS commands/queries, MediatR validation+logging behaviors, FluentValidation validators, DTO mapping, baseline ISlaCalculator; build 0 warnings |
| 2026-11-11 | OpsRelay Bot | T005 complete — classic Startup.cs + Program.cs, routed IncidentsController, RFC 7807 middleware; Swagger HTTP 200 smoke test passed. DB-backed endpoints pending Docker/SQL Server availability |
| 2026-11-11 | OpsRelay Bot | T006 complete — Serilog JSON console/file logging with CorrelationId, environment configs, /health SQL Server + DbContext checks. Health returns structured 503 JSON while SQL Server is unavailable |
| 2026-11-11 | OpsRelay Bot | T007 complete — Keep a Changelog 0.1.0 release notes and docs/git-workflow.md; annotated tag v0.1.0 created on develop |
| 2026-11-11 | OpsRelay Bot | T008 complete — Swagger with XML docs, enum name/value schema filter and request/ProblemDetails examples; /swagger UI and swagger.json verified HTTP 200 |
| 2026-11-11 | OpsRelay Bot | PHASE-01 complete — all T001-T008 done, build 0 errors / 0 warnings, tag v0.1.0 present. DB-backed integration verification deferred until Docker/SQL Server is available |
| 2026-11-12 | OpsRelay Bot | PHASE-01 debug — SQL Server 2022 container, migration applied, full API lifecycle verified (health 200, POST 201 + Location + 4h Critical SLA, GET 200, PATCH 204, paging/filter, 400/404 ProblemDetails, rowversion + audit columns). Fixed enum serialization to strings, en-US validation messages, problem+json on GET 404, production log level |

---

## Agent Session Protocol

Every autonomous agent session MUST follow this protocol:

### On Session START:
1. `cd c:\Users\maksy\Projects\net-incident-platform`
2. Read `AGENTS.md` and `build-state.md`
3. Read the full plan: `C:\Users\maksy\.gemini\antigravity\brain\71ef0f0f-418f-4fc3-bcb7-2dca0ca11ab5\opsrelay-plan.md`
4. Identify the **lowest-numbered ⬜ task** that has no `🔄 In Progress` blocker dependency
5. Claim it: set status → `🔄 In Progress` in `build-state.md`, commit the state file update
6. `git checkout -b feature/T{NNN}-{slug}` from `develop`
7. Implement the task fully per plan specifications
8. Update `build-state.md` status → `✅ Done`
9. Merge to `develop` via `git merge --no-ff feature/T{NNN}-{slug}`

### On Session END (even if incomplete):
1. Commit all work-in-progress: `git commit -m "wip(T{NNN}): checkpoint — {what was done}"`  
2. Update `build-state.md` with current status and notes
3. Push branch to remote if configured

### Error Handling:
- If a `dotnet` command fails because SDK is missing → install it first (see Environment Checklist)
- If a migration fails → check connection string in `appsettings.Development.json`
- If a port conflict → API defaults to port 5000/5001; Angular dev server to 4200

---

## Key Architecture Decisions (quick ref)

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Architecture | Clean Architecture | Testability, separation of concerns |
| Messaging | CQRS + MediatR | Decouple read/write, pipeline behaviors |
| SLA Logic | Strategy Pattern | Swappable calculators per priority |
| State Machine | IncidentStateMachine class | Explicit guard clauses, raise domain events |
| Time Abstraction | System.TimeProvider | Deterministic tests via FakeTimeProvider |
| Frontend State | Angular Signals | Simpler than NgRx for this scale |
| DB Concurrency | RowVersion byte[] | Optimistic concurrency on Incident |
| Error Format | RFC 7807 ProblemDetails | Interoperable, standard API error format |

---

## Directory Structure (target)

```
net-incident-platform/
├── src/
│   ├── Backend/
│   │   ├── OpsRelay.Domain/
│   │   │   ├── Entities/          # Incident.cs, SlaPolicy.cs, AggregateRoot.cs
│   │   │   ├── Enums/             # Priority.cs, IncidentStatus.cs
│   │   │   ├── Events/            # IDomainEvent.cs, Incident*DomainEvent.cs
│   │   │   ├── ValueObjects/      # TeamId.cs, IncidentTitle.cs
│   │   │   └── Interfaces/        # IIncidentRepository.cs, ISlaPolicyRepository.cs
│   │   ├── OpsRelay.Application/
│   │   │   ├── Incidents/
│   │   │   │   ├── Commands/      # CreateIncident/, UpdateStatus/
│   │   │   │   └── Queries/       # GetIncidentById/, GetIncidentsPaged/
│   │   │   └── Common/
│   │   │       ├── Behaviours/    # ValidationBehavior.cs
│   │   │       ├── Interfaces/    # IUnitOfWork.cs, ISlaCalculator.cs
│   │   │       └── DTOs/          # IncidentDto.cs, PagedResultDto.cs
│   │   ├── OpsRelay.Infrastructure/
│   │   │   ├── Persistence/
│   │   │   │   ├── OpsRelayDbContext.cs
│   │   │   │   ├── Configurations/ # IncidentConfiguration.cs, SlaPolicyConfiguration.cs
│   │   │   │   └── Migrations/
│   │   │   ├── Repositories/      # IncidentRepository.cs, SlaPolicyRepository.cs
│   │   │   └── Services/          # UnitOfWork.cs, SlaCalculatorFactory.cs
│   │   └── OpsRelay.Api/
│   │       ├── Controllers/       # IncidentsController.cs
│   │       ├── Middleware/        # ExceptionHandlingMiddleware.cs
│   │       └── Extensions/        # ServiceCollectionExtensions.cs
│   └── Frontend/
│       └── ops-relay-web/         # Angular 17 standalone app
├── tests/
│   ├── OpsRelay.UnitTests/
│   │   ├── Domain/                # Incident entity tests, state machine tests
│   │   └── Application/           # SLA calculator tests, handler tests
│   └── OpsRelay.IntegrationTests/
│       ├── Controllers/           # IncidentsController integration tests
│       └── Fixtures/              # WebAppFactory, Testcontainers setup
├── docs/
│   ├── adr/                       # ADR-001 through ADR-004
│   └── diagrams/
├── .github/
│   ├── workflows/                 # ci.yml
│   └── PULL_REQUEST_TEMPLATE/     # pr-template.md
├── .gitignore
├── .editorconfig
├── AGENTS.md                      # THIS FILE — agent context
├── build-state.md                 # Task progress board
├── CHANGELOG.md
├── OpsRelay.sln
├── Dockerfile
└── docker-compose.yml
```
