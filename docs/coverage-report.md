# Code Coverage Report

- **Date:** 2026-11-12
- **Tool:** `coverlet.msbuild` 6.0.4 with `dotnet test`
- **Scope:** `OpsRelay.Domain` and `OpsRelay.Application`
- **Gate:** 80% line coverage (total)
- **Result:** PASS — 94.68% total line coverage

## Command

```bash
dotnet test tests/OpsRelay.UnitTests/OpsRelay.UnitTests.csproj \
  -c Release \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:CoverletOutput=TestResults/coverage/ \
  "/p:Include=[OpsRelay.Domain]*%2c[OpsRelay.Application]*" \
  /p:Threshold=80 \
  /p:ThresholdType=line \
  /p:ThresholdStat=total
```

## Summary

| Module | Line | Branch | Method |
|--------|------|--------|--------|
| OpsRelay.Application | 96.09% | 100.00% | 98.33% |
| OpsRelay.Domain | 93.15% | 89.58% | 94.28% |
| **Total** | **94.68%** | **92.18%** | **96.15%** |

- Total tests: **99**
- Test duration: **under 1 second**
- External dependencies: none (no database or network)

## Application Breakdown

| Class | Line |
|-------|------|
| `LoggingBehavior<TRequest, TResponse>` | 100% |
| `ValidationBehavior<TRequest, TResponse>` | 100% |
| `DomainEventNotification<TDomainEvent>` | 100% |
| `PagedResultDto<T>` | 100% |
| `IncidentDto` | 100% |
| `SlaPolicyDto` | 100% |
| `CreateIncidentCommand` | 100% |
| `CreateIncidentCommandHandler` | 100% |
| `CreateIncidentCommandValidator` | 100% |
| `UpdateIncidentStatusCommand` | 100% |
| `UpdateIncidentStatusCommandHandler` | 100% |
| `UpdateIncidentStatusCommandValidator` | 100% |
| `GetIncidentByIdQuery` / handler | 100% |
| `GetIncidentsPagedQuery` / handler | 100% |
| `IncidentMappingExtensions` | 100% |
| `Application.DependencyInjection` | 0% |

`Application.DependencyInjection` is not exercised directly by unit tests.
Service composition is covered by the API smoke tests and will be covered by the
`WebApplicationFactory` integration suite in T033.

## Domain Breakdown

| Class | Line |
|-------|------|
| `AggregateRoot<TId>` | 100% |
| `Entity<TId>` | 80.00% |
| `ValueObject` | 100% |
| `Incident` | 87.27% |
| `SlaPolicy` | 89.28% |
| `IncidentCreatedDomainEvent` | 100% |
| `IncidentStatusChangedDomainEvent` | 100% |
| `SlaBreachedDomainEvent` | 100% |
| `DomainException` | 100% |
| `InvalidStatusTransitionException` | 100% |
| `IncidentStateMachine` | 100% |
| `SlaCalculationResult` | 100% |
| `IncidentTitle` | 90.00% |
| `TeamId` | 88.88% |

The remaining uncovered lines are defensive guard branches and type-mismatch
paths that cannot be reached through the public API without violating the
domain model.

## Coverage Focus

- All 36 state-machine transition permutations are asserted.
- Both SLA strategies are tested across nights, weekends and 24x7 boundaries.
- Handlers, validators, pipeline behaviors and mappings are covered directly.
- `FakeTimeProvider` is used for deterministic audit and transition timestamps.
