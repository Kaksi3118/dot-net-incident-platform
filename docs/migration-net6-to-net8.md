# .NET 6 to .NET 8 Migration Playbook

This playbook documents the exact sequence used to migrate OpsRelay from the
.NET 6 baseline to .NET 8. It is intended for enterprise teams that need a
reviewable, low-risk upgrade path.

## Prerequisites

- .NET 8 SDK installed (`dotnet --version` returns `8.0.x`).
- A green baseline build and test run on .NET 6.
- A dedicated branch: `migration/net6-to-net8`.
- A database backup or disposable database for migration verification.

## Step 1: Create the migration branch from the baseline

```bash
git checkout develop
git checkout -b migration/net6-to-net8
dotnet build OpsRelay.sln
dotnet test
```

Do not continue until the baseline is green.

## Step 2: Pin the SDK and centralize build properties

Update `global.json` to the .NET 8 SDK, then add `Directory.Build.props`:

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <AnalysisLevel>latest-recommended</AnalysisLevel>
  </PropertyGroup>
</Project>
```

Remove duplicated `TargetFramework`, `LangVersion`, nullable and implicit-usings
properties from individual `.csproj` files.

## Step 3: Upgrade packages in dependency order

| Package | From | To |
|---------|------|----|
| Microsoft.EntityFrameworkCore.SqlServer | 6.0.25 | 8.0.2 |
| Microsoft.EntityFrameworkCore.Design | 6.0.25 | 8.0.2 |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore | 6.0.25 | 8.0.2 |
| MediatR | 11.1.0 | 12.2.0 |
| FluentValidation | 11.3.0 | 11.9.0 |
| Serilog.AspNetCore | 6.1.0 | 8.0.1 |
| xunit | 2.4.2 | 2.6.6 |
| NSubstitute | 5.0.0 | 5.1.0 |
| Microsoft.AspNetCore.Mvc.Testing | 6.0.25 | 8.0.2 |

Remove `MediatR.Extensions.Microsoft.DependencyInjection`; MediatR 12 includes
the registration extensions in the core package.

### MediatR 12 breaking changes

Replace the old assembly registration:

```csharp
services.AddMediatR(typeof(CreateIncidentCommand).Assembly);
```

with configuration-based registration:

```csharp
services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
    configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});
```

Non-generic `IRequestHandler<TRequest>.Handle` now returns `Task` instead of
`Task<Unit>`.

## Step 4: Replace Startup.cs with minimal hosting

1. Create one extension per layer:
   - `AddApplicationServices()`
   - `AddInfrastructureServices(IConfiguration)`
   - `AddApiServices(IConfiguration)`
2. Move DI registrations from `Startup.ConfigureServices` into those extensions.
3. Convert `Program.cs` to top-level statements:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services));
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();
app.UseSerilogRequestLogging();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
```

4. Add `public partial class Program { }` so `WebApplicationFactory<Program>`
   can be used by integration tests.
5. Delete `Startup.cs` only after the minimal host boots and endpoints respond.

## Step 5: Adopt C# 12 features

### Primary constructors

```csharp
public class GetIncidentByIdQueryHandler(IIncidentRepository repository)
    : IRequestHandler<GetIncidentByIdQuery, IncidentDto?>
{
    public async Task<IncidentDto?> Handle(
        GetIncidentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var incident = await repository.GetByIdAsync(request.Id, cancellationToken);

        return incident?.ToDto();
    }
}
```

### Collection expressions

```csharp
private readonly List<IDomainEvent> _domainEvents = [];
public byte[] RowVersion { get; private set; } = [];
List<ValidationFailure> failures = [.. validationResults.SelectMany(result => result.Errors)];
```

Prefer `[]` over `new List<T>()` and `Array.Empty<T>()`. Use `[.. source]` when a
materialized collection is required.

## Step 6: Replace ambient time with TimeProvider

Register the system provider:

```csharp
services.AddSingleton(TimeProvider.System);
```

Inject `TimeProvider` into handlers and the `DbContext`, then replace
`DateTimeOffset.UtcNow` with `timeProvider.GetUtcNow()`. In tests, use
`Microsoft.Extensions.Time.Testing.FakeTimeProvider`:

```csharp
var timeProvider = new FakeTimeProvider
    (new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero));
timeProvider.Advance(TimeSpan.FromHours(4));
```

A `DbContext` constructor that accepts both
`DbContextOptions<OpsRelayDbContext>` and `TimeProvider` is resolved by
`AddDbContext`. The design-time factory must pass `TimeProvider.System`
explicitly.

## Step 7: Adopt EF Core 8 features

- Enable split queries globally:

```csharp
options.UseSqlServer(connectionString, sql =>
    sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
```

- Use `ExecuteUpdateAsync` for bulk operations that must bypass the change
  tracker. Shadow properties can be set through `EF.Property`:

```csharp
await dbContext.Incidents
    .Where(incident => ids.Contains(incident.Id))
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(incident => incident.Status, status)
        .SetProperty(incident => EF.Property<DateTimeOffset>(incident, "UpdatedAt"), now));
```

- Map owned metadata to JSON:

```csharp
builder.OwnsOne(message => message.Metadata, metadata => metadata.ToJson());
```

- Add a migration and verify the generated SQL before deploying:

```bash
dotnet ef migrations add AddOutboxMessages \
  --project src/Backend/OpsRelay.Infrastructure
dotnet ef migrations script \
  --project src/Backend/OpsRelay.Infrastructure
```

## Step 8: Verify behavior

Run all of the following against a real SQL Server instance:

- `dotnet build OpsRelay.sln` with zero warnings.
- Both migrations from a clean database.
- `GET /health` returns 200 with healthy checks.
- `POST /api/v1/incidents` returns 201 and a Location header.
- `GET /api/v1/incidents/{id}` returns 200.
- `PATCH /api/v1/incidents/{id}/status` returns 204 and persists the status.
- Validation and not-found responses use RFC 7807 `application/problem+json`.
- Bulk update does not leave the updated entity tracked.
- Outbox JSON metadata round-trips.

## Step 9: Document and release

1. Write the ADR and update `CHANGELOG.md`.
2. Open a pull request using `.github/pull_request_template.md`.
3. Merge with `--no-ff` into `develop`.
4. Tag the merge commit (`v0.2.0` for this migration).
5. Delete the migration branch.

## Rollback

The migration is isolated on `migration/net6-to-net8`. If verification fails,
do not merge it. The `v0.1.0` tag remains the supported .NET 6 baseline, and the
last migration in the database can be rolled back with:

```bash
dotnet ef database update 20261111000000_InitialCreate \
  --project src/Backend/OpsRelay.Infrastructure
```
