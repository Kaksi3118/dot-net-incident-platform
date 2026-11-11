using Microsoft.EntityFrameworkCore;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Repositories;
using OpsRelay.Infrastructure.Persistence;

namespace OpsRelay.Infrastructure.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly OpsRelayDbContext _dbContext;

    public IncidentRepository(OpsRelayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Incidents.FirstOrDefaultAsync(
            incident => incident.Id == id,
            cancellationToken);

    public async Task<(IReadOnlyList<Incident> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        IncidentStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Incidents.AsNoTracking().AsQueryable();

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
        await _dbContext.Incidents.AddAsync(incident, cancellationToken);

    public void Update(Incident incident) => _dbContext.Incidents.Update(incident);
}
