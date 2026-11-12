using Microsoft.EntityFrameworkCore;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Repositories;
using OpsRelay.Infrastructure.Persistence;

namespace OpsRelay.Infrastructure.Repositories;

public class IncidentRepository(
    OpsRelayDbContext dbContext,
    TimeProvider timeProvider) : IIncidentRepository
{
    public Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Incidents.FirstOrDefaultAsync(
            incident => incident.Id == id,
            cancellationToken);

    public async Task<(IReadOnlyList<Incident> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        IncidentStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Incidents.AsNoTracking().AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(incident => incident.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(incident => incident.ReportedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Incident incident, CancellationToken cancellationToken = default) =>
        await dbContext.Incidents.AddAsync(incident, cancellationToken);

    public void Update(Incident incident) => dbContext.Incidents.Update(incident);

    public async Task<int> BulkUpdateStatusAsync(
        IEnumerable<Guid> ids,
        IncidentStatus status,
        CancellationToken cancellationToken = default)
    {
        List<Guid> incidentIds = [.. ids];

        if (incidentIds.Count == 0)
        {
            return 0;
        }

        var updatedAt = timeProvider.GetUtcNow();

        return await dbContext.Incidents
            .Where(incident => incidentIds.Contains(incident.Id))
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(incident => incident.Status, status)
                    .SetProperty(
                        incident => EF.Property<DateTimeOffset>(incident, "UpdatedAt"),
                        updatedAt),
                cancellationToken);
    }
}
