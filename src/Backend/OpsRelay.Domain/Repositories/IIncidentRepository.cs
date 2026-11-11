using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Domain.Repositories;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Incident> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        IncidentStatus? status,
        CancellationToken cancellationToken = default);

    Task AddAsync(Incident incident, CancellationToken cancellationToken = default);

    void Update(Incident incident);
}
