using MediatR;
using OpsRelay.Application.DTOs;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Application.Incidents.Commands.CreateIncident;

public sealed record CreateIncidentCommand(
    string Title,
    string Description,
    Priority Priority,
    string AssignedTeamId) : IRequest<IncidentDto>;
