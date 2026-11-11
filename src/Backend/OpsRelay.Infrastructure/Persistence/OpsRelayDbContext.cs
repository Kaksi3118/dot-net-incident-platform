using Microsoft.EntityFrameworkCore;
using OpsRelay.Domain.Common;
using OpsRelay.Domain.Entities;

namespace OpsRelay.Infrastructure.Persistence;

public class OpsRelayDbContext : DbContext
{
    public OpsRelayDbContext(DbContextOptions<OpsRelayDbContext> options)
        : base(options)
    {
    }

    public DbSet<Incident> Incidents => Set<Incident>();

    public DbSet<SlaPolicy> SlaPolicies => Set<SlaPolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OpsRelayDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = DateTimeOffset.UtcNow;

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
