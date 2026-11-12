# Pull Request

## Summary

<!-- One paragraph describing the change and why it is needed. -->

## Task Reference

- **Task ID:** T000
- **Branch:** `feature/T000-...`
- **Phase:** PHASE-00
- **Related ADR:** <!-- e.g. docs/adr/ADR-001-net8-migration.md -->

## Changes

### Added

-

### Changed

-

### Removed

-

## Acceptance Criteria

- [ ] Task acceptance criteria are satisfied.
- [ ] `dotnet build OpsRelay.sln --configuration Release` passes with 0 warnings.
- [ ] `dotnet test` passes.
- [ ] Public API behavior remains backward compatible (or breaking changes are
      documented below).
- [ ] EF Core migrations are included and verified against SQL Server 2022.
- [ ] No secrets, credentials or local connection strings are committed.

## Risk Assessment

| Area | Risk | Mitigation |
|------|------|------------|
| API contract | Low / Medium / High | |
| Database schema | Low / Medium / High | |
| Authentication/authorization | Low / Medium / High | |
| Performance | Low / Medium / High | |
| Deployment | Low / Medium / High | |

### Breaking Changes

<!-- List breaking changes and migration steps, or write "None". -->

## Verification Steps

1.
2.
3.

### Evidence

<!-- Test output, Swagger screenshot, SQL diff, benchmark results. -->

## Reviewer Checklist

- [ ] Architecture boundaries respected.
- [ ] Error handling follows RFC 7807 ProblemDetails.
- [ ] Logging uses structured Serilog properties and correlation ids.
- [ ] Time-dependent logic uses `TimeProvider` (no ambient `UtcNow`).
- [ ] EF Core bulk operations are intentional and tested.
- [ ] `CHANGELOG.md` and `build-state.md` are updated.

## Migration Notes (PHASE-02 only)

### Before (.NET 6 / C# 10)

```csharp
// Legacy code
```

### After (.NET 8 / C# 12)

```csharp
// Modern equivalent
```

### Performance Impact

<!-- Quantify allocations, tracked entities, query counts or latency where possible. -->
