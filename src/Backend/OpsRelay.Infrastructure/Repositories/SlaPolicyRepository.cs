using Microsoft.EntityFrameworkCore;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Repositories;
using OpsRelay.Infrastructure.Persistence;

namespace OpsRelay.Infrastructure.Repositories;

public class SlaPolicyRepository(OpsRelayDbContext dbContext) : ISlaPolicyRepository
{
    public Task<SlaPolicy?> GetByPriorityAsync(
        Priority priority,
        CancellationToken cancellationToken = default) =>
        dbContext.SlaPolicies
            .AsNoTracking()
            .FirstOrDefaultAsync(policy => policy.Priority == priority, cancellationToken);

    public async Task<IReadOnlyList<SlaPolicy>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.SlaPolicies
            .AsNoTracking()
            .OrderBy(policy => policy.Priority)
            .ToListAsync(cancellationToken);
}
