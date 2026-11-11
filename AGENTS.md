# OpsRelay — Agent Context & Briefing

> **FOR AGENTS**: Read this file completely at the start of every session.
> Do not skip any section. This is your mission briefing.

---

## What You Are Building

**OpsRelay** is a production-grade, portfolio-quality **Enterprise Incident Lifecycle & SLA Management Platform**.

It demonstrates:
- Clean Architecture in ASP.NET Core
- CQRS with MediatR
- A deliberate **.NET 6 → .NET 8 migration** (C# 12, primary constructors, TimeProvider, minimal hosting)
- SLA calculation engines via Strategy Pattern
- Angular 17 standalone components with Signals
- Integration testing with Testcontainers
- Full Docker containerization

**Target audience**: Recruiters and engineering leads evaluating .NET / Full-Stack expertise.

---

## Your Mission

Implement the **OpsRelay** project by following the execution plan **task by task**, in order.

### The Plan
Full specification: `C:\Users\maksy\.gemini\antigravity\brain\71ef0f0f-418f-4fc3-bcb7-2dca0ca11ab5\opsrelay-plan.md`

### Progress Tracker
Current task statuses: `c:\Users\maksy\Projects\net-incident-platform\build-state.md`

---

## Rules You Must Follow

### 1. Always Read State First
Before writing a single line of code, read `build-state.md` to know what's done and what's next.

### 2. One Task at a Time
Pick the **lowest-numbered ⬜ task** with no unresolved dependencies.
Mark it `🔄 In Progress` in `build-state.md` immediately.

### 3. Git Discipline
```
# Always start from develop
git checkout develop
git pull origin develop   # if remote exists
git checkout -b feature/T{NNN}-{slug}

# Commit frequently (at meaningful checkpoints)
git add -A
git commit -m "type(scope): description"

# After task is complete, merge back
git checkout develop
git merge --no-ff feature/T{NNN}-{slug}
git branch -d feature/T{NNN}-{slug}
```

### 4. Conventional Commits
Format: `type(scope): description`

Types: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`, `build`, `ci`

Scopes match the layer: `domain`, `application`, `infrastructure`, `api`, `frontend`, `tests`, `docker`, `ci`

Examples:
- `feat(domain): add Incident aggregate with IncidentTitle value object`
- `refactor(api): dissolve Startup.cs into Program.cs top-level statements`
- `test(application): add SLA calculator unit tests with FakeTimeProvider`

### 5. Set Git Identity Before First Commit
```bash
git config user.name "OpsRelay Bot"
git config user.email "opsrelay-bot@build.local"
git config core.autocrlf true
```

### 6. Create develop Branch for First Task
```bash
git checkout -b develop
# After T001 is complete and committed on develop:
git tag v0.1.0-alpha   # mark that develop exists
```

### 7. Mark Tasks Done
After completing a task, update `build-state.md`:
- Change `🔄 In Progress` → `✅ Done`
- Add timestamp to Completed Work Log
- Commit the state file: `chore(state): mark T{NNN} as done`

### 8. Never Break the Build
Before marking a task done, verify:
- `dotnet build OpsRelay.sln` → 0 errors
- `dotnet test` → all tests pass (once tests exist)
- If you can't verify (SDK missing), note it in `build-state.md`

### 9. Environment First
If .NET SDK is missing, install it before anything else:
```powershell
winget install Microsoft.DotNet.SDK.8
winget install Microsoft.DotNet.SDK.6
# Restart PowerShell after install
```

### 10. File Paths Are Relative to Repo Root
All file paths in the plan are relative to: `c:\Users\maksy\Projects\net-incident-platform`

---

## Phase Gates

Do not start the next phase until the phase gate is met:

| Gate | Condition |
|------|-----------|
| PHASE-01 complete | `dotnet build` passes, `git tag v0.1.0` exists |
| PHASE-02 complete | `dotnet build` passes on net8.0, `git tag v0.2.0` exists on `develop` |
| PHASE-03 complete | `dotnet test` passes with ≥80% coverage, `git tag v0.3.0` exists |
| PHASE-04 complete | `ng build --configuration production` passes |
| PHASE-05 complete | `docker compose up` starts cleanly, integration tests green, `git tag v1.0.0` on `main` |

---

## Stack Quick Reference

### Backend
| Component | Technology |
|-----------|-----------|
| Runtime (Phase 1) | .NET 6 / ASP.NET Core 6 |
| Runtime (Phase 2+) | .NET 8 / ASP.NET Core 8 |
| Language | C# 12 (after migration) |
| ORM | Entity Framework Core 6 → 8 |
| Database | Microsoft SQL Server 2022 (via Docker) |
| CQRS | MediatR 11 → 12 |
| Validation | FluentValidation 11 |
| Logging | Serilog |
| API Docs | Swashbuckle / Scalar |
| Testing | xUnit, FluentAssertions, NSubstitute, Testcontainers |

### Frontend
| Component | Technology |
|-----------|-----------|
| Framework | Angular 17 (Standalone) |
| Language | TypeScript 5.x (strict) |
| UI Library | Angular Material 17 + Tailwind CSS |
| State | Angular Signals + RxJS |
| Forms | Typed Reactive Forms |
| HTTP | HttpClient with functional interceptors |

### Infrastructure
| Component | Technology |
|-----------|-----------|
| Containerization | Docker (multi-stage, Alpine) |
| Orchestration | docker-compose v3.8 |
| CI/CD | GitHub Actions |
| DB in CI | Testcontainers (MsSql) |

---

## Connection String (Development)

Default `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OpsRelayDb;User Id=sa;Password=OpsRelay_Dev!2024;TrustServerCertificate=True;"
  }
}
```

For Docker Compose:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver,1433;Database=OpsRelayDb;User Id=sa;Password=OpsRelay_Dev!2024;TrustServerCertificate=True;"
  }
}
```

---

## Ports

| Service | Port |
|---------|------|
| API (HTTP) | 5000 |
| API (HTTPS) | 5001 |
| Angular Dev | 4200 |
| SQL Server | 1433 |

---

## Key Namespaces

```
OpsRelay.Domain.Entities
OpsRelay.Domain.Events
OpsRelay.Domain.Enums
OpsRelay.Domain.ValueObjects
OpsRelay.Domain.Interfaces
OpsRelay.Application.Incidents.Commands.CreateIncident
OpsRelay.Application.Incidents.Commands.UpdateStatus
OpsRelay.Application.Incidents.Queries.GetIncidentById
OpsRelay.Application.Incidents.Queries.GetIncidentsPaged
OpsRelay.Application.Common.Behaviours
OpsRelay.Application.Common.Interfaces
OpsRelay.Application.Common.DTOs
OpsRelay.Infrastructure.Persistence
OpsRelay.Infrastructure.Repositories
OpsRelay.Infrastructure.Services
OpsRelay.Api.Controllers
OpsRelay.Api.Middleware
OpsRelay.Api.Extensions
```

---

## If You Get Stuck

1. Re-read the relevant task in the plan file (it has exhaustive details)
2. Check `build-state.md` Completed Work Log for prior context
3. For .NET SDK errors — verify `dotnet --version` returns 8.x
4. For EF migration errors — ensure SQL Server container is running: `docker compose up sqlserver -d`
5. For Angular errors — ensure Node 18+ and `ng version` shows 17.x
