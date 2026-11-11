using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Infrastructure.Persistence;

namespace OpsRelay.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly OpsRelayDbContext _dbContext;

    public UnitOfWork(OpsRelayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
