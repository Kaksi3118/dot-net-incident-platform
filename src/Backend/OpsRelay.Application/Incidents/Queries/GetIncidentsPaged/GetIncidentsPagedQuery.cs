using MediatR;
using OpsRelay.Application.Common.Models;
using OpsRelay.Application.DTOs;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Application.Incidents.Queries.GetIncidentsPaged;

public sealed record GetIncidentsPagedQuery(
    int PageNumber,
    int PageSize,
    IncidentStatus? Status) : IRequest<PagedResultDto<IncidentDto>>;
