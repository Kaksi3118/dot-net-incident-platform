using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Domain.Repositories;

public interface ISlaPolicyRepository
{
    Task<SlaPolicy?> GetByPriorityAsync(
        Priority priority,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SlaPolicy>> GetAllAsync(CancellationToken cancellationToken = default);
}
