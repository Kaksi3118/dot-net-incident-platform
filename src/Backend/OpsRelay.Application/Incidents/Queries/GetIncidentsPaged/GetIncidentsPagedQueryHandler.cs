using MediatR;
using OpsRelay.Application.Common.Models;
using OpsRelay.Application.DTOs;
using OpsRelay.Application.Mappings;
using OpsRelay.Domain.Repositories;

namespace OpsRelay.Application.Incidents.Queries.GetIncidentsPaged;

public class GetIncidentsPagedQueryHandler(IIncidentRepository incidentRepository)
    : IRequestHandler<GetIncidentsPagedQuery, PagedResultDto<IncidentDto>>
{
    public async Task<PagedResultDto<IncidentDto>> Handle(
        GetIncidentsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await incidentRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Status,
            cancellationToken);

        var dtos = items.Select(incident => incident.ToDto()).ToList();

        return new PagedResultDto<IncidentDto>(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
