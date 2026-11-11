using Microsoft.EntityFrameworkCore;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Repositories;
using OpsRelay.Infrastructure.Persistence;

namespace OpsRelay.Infrastructure.Repositories;

public class SlaPolicyRepository : ISlaPolicyRepository
{
    private readonly OpsRelayDbContext _dbContext;

    public SlaPolicyRepository(OpsRelayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<SlaPolicy?> GetByPriorityAsync(
        Priority priority,
        CancellationToken cancellationToken = default) =>
        _dbContext.SlaPolicies
            .AsNoTracking()
            .FirstOrDefaultAsync(policy => policy.Priority == priority, cancellationToken);

    public async Task<IReadOnlyList<SlaPolicy>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await _dbContext.SlaPolicies
            .AsNoTracking()
            .OrderBy(policy => policy.Priority)
            .ToListAsync(cancellationToken);
}
