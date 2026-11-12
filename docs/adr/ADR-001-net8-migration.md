# ADR-001: Migration from .NET 6 to .NET 8 LTS

- **Status:** Accepted
- **Date:** 2026-11-12
- **Deciders:** OpsRelay engineering
- **Related:** `docs/migration-net6-to-net8.md`, `CHANGELOG.md`

## Context

OpsRelay was built on .NET 6 to demonstrate a realistic legacy baseline. .NET 6
reached end of support in November 2024, so the platform must move to a
supported LTS runtime. The migration is also an opportunity to demonstrate the
C# 12 language features and .NET 8 platform capabilities expected from a modern
ASP.NET Core codebase.

The baseline has four constraints that shape the migration:

- The REST surface (`/api/v1/incidents`) must remain backward compatible.
- All six projects must build with zero warnings under
  `TreatWarningsAsErrors=true`.
- Domain and application logic must remain deterministic and testable.
- The migration must be reviewable commit by commit on a dedicated branch.

## Decision Drivers

- Supported LTS runtime with security servicing.
- Lower DI and persistence boilerplate.
- Deterministic time handling for SLA tests.
- Better EF Core query and bulk-write performance.
- A credible portfolio artifact showing a real migration, not a rewrite.

## Considered Options

### Option 1: Stay on .NET 6

Rejected. The runtime is out of support, and the repository would not reflect
current enterprise standards.

### Option 2: Upgrade to .NET 7

Rejected. .NET 7 was a Standard Term Support release and is also out of support.
It would add migration work without reaching an LTS baseline.

### Option 3: Upgrade to .NET 8 LTS

Selected. .NET 8 is an LTS release, supports C# 12, and aligns with EF Core 8
and ASP.NET Core 8. It also provides `TimeProvider` and production-grade EF Core
features without requiring a framework rewrite.

### Option 4: Rewrite the platform

Rejected. A rewrite would discard the legacy comparison that makes the
migration case study valuable.

## Decision

Upgrade all projects to `net8.0` and C# 12, and modernize the affected idioms
incrementally on `migration/net6-to-net8`:

1. Centralize build properties in `Directory.Build.props`.
2. Upgrade EF Core 6 to 8, MediatR 11 to 12, FluentValidation to 11.9, and the
   test/serialization stack to .NET 8-compatible versions.
3. Replace `Startup.cs` with minimal hosting and per-layer
   `IServiceCollection` extensions.
4. Adopt C# 12 primary constructors and collection expressions.
5. Replace ambient `DateTimeOffset.UtcNow` calls with `TimeProvider`.
6. Enable global split queries, `ExecuteUpdateAsync` bulk updates and EF Core 8
   JSON-owned types.

## Consequences

### Positive

- The solution targets a supported LTS runtime.
- Constructor and collection boilerplate is reduced.
- SLA calculations and persistence audit timestamps are testable through
  `FakeTimeProvider`.
- Bulk updates bypass the change tracker and split queries avoid Cartesian
  product explosions.
- The repository now contains an auditable before/after migration narrative.

### Negative

- MediatR 12 and EF Core 8 introduce breaking changes that must be handled at
  once; the migration branch is not compatible with the .NET 6 baseline.
- Minimal hosting removes the explicit two-phase `Startup` seam that some
  enterprise teams use for convention-based registration.
- `TimeProvider` requires discipline: ambient time calls must not be reintroduced
  into handlers or persistence code.

### Neutral

- The API contract is unchanged apart from enum JSON representation, which was
  aligned with the documented client contract during Phase 1 debugging.
- The two persistence migrations now run in sequence from a clean database.

## Quantitative Impact

Measured on this repository during the migration:

| Metric | Result |
|--------|--------|
| C# 12 primary-constructor refactor | 9 files changed, 45 insertions / 109 deletions (net -64 lines) |
| `Startup.cs` removed | 95 lines deleted; registration split into 3 layer extensions |
| Ambient time calls remaining in business/persistence code | 0 |
| `new List<T>()` / `Array.Empty<T>()` in refactored paths | 0 |
| Bulk update tracked entities | 0 (verified `ExecuteUpdateAsync` leaves the tracked entity unchanged while the database row updates) |
| Outbox metadata joins after JSON mapping | 0 (metadata is stored in the `OutboxMessages.Metadata` JSON column) |

Expected platform-level improvements, to be benchmarked with BenchmarkDotNet in
a later phase, include lower allocation pressure from source-generated JSON and
dynamic PGO in .NET 8, fewer materialized entities for bulk status changes, and
fewer rows returned by split queries when loading related collections. This ADR
records the structural changes; it does not claim production benchmark numbers
that were not measured.

## Verification

- `dotnet build OpsRelay.sln` passes with 0 errors and 0 warnings.
- Both EF Core migrations apply cleanly to SQL Server 2022.
- Health checks return 200 with both `sqlserver` and `dbcontext` healthy.
- Full incident lifecycle verified: POST 201 with Location, GET 200, PATCH 204,
  paging and filtering, RFC 7807 400/404 responses.
- JSON outbox metadata round-trips through the database.

## Links

- Migration playbook: [`docs/migration-net6-to-net8.md`](../migration-net6-to-net8.md)
- EF Core migration history: `src/Backend/OpsRelay.Infrastructure/Persistence/Migrations`
- Microsoft .NET 8 release notes: <https://learn.microsoft.com/dotnet/core/whats-new/dotnet-8>
- EF Core 8 release notes: <https://learn.microsoft.com/ef/core/what-is-new/ef-core-8.0>
