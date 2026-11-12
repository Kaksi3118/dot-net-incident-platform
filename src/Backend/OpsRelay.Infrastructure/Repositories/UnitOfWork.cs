using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Infrastructure.Persistence;

namespace OpsRelay.Infrastructure.Repositories;

public class UnitOfWork(OpsRelayDbContext dbContext) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
