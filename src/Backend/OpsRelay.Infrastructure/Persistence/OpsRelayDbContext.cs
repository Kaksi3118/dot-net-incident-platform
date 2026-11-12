using Microsoft.EntityFrameworkCore;
using OpsRelay.Domain.Common;
using OpsRelay.Domain.Entities;
using OpsRelay.Infrastructure.Persistence.Entities;

namespace OpsRelay.Infrastructure.Persistence;

public class OpsRelayDbContext(
    DbContextOptions<OpsRelayDbContext> options,
    TimeProvider timeProvider) : DbContext(options)
{
    public DbSet<Incident> Incidents => Set<Incident>();

    public DbSet<SlaPolicy> SlaPolicies => Set<SlaPolicy>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OpsRelayDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = timeProvider.GetUtcNow();

        foreach (var entry in ChangeTracker.Entries<Entity<Guid>>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAt").CurrentValue = timestamp;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAt").CurrentValue = timestamp;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
