using MediatR;
using OpsRelay.Application.DTOs;
using OpsRelay.Application.Mappings;
using OpsRelay.Domain.Repositories;

namespace OpsRelay.Application.Incidents.Queries.GetIncidentById;

public class GetIncidentByIdQueryHandler : IRequestHandler<GetIncidentByIdQuery, IncidentDto?>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentByIdQueryHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<IncidentDto?> Handle(
        GetIncidentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.Id, cancellationToken);

        return incident?.ToDto();
    }
}
