using MediatR;
using OpsRelay.Application.DTOs;

namespace OpsRelay.Application.Incidents.Queries.GetIncidentById;

public sealed record GetIncidentByIdQuery(Guid Id) : IRequest<IncidentDto?>;
