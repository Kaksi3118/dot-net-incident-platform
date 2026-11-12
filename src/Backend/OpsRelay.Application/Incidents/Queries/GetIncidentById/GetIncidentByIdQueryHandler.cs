using MediatR;
using OpsRelay.Application.DTOs;
using OpsRelay.Application.Mappings;
using OpsRelay.Domain.Repositories;

namespace OpsRelay.Application.Incidents.Queries.GetIncidentById;

public class GetIncidentByIdQueryHandler(IIncidentRepository incidentRepository)
    : IRequestHandler<GetIncidentByIdQuery, IncidentDto?>
{
    public async Task<IncidentDto?> Handle(
        GetIncidentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var incident = await incidentRepository.GetByIdAsync(request.Id, cancellationToken);

        return incident?.ToDto();
    }
}
