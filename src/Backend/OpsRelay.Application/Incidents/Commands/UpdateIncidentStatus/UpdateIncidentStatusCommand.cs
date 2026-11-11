using MediatR;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Application.Incidents.Commands.UpdateIncidentStatus;

public sealed record UpdateIncidentStatusCommand(
    Guid Id,
    IncidentStatus NewStatus,
    string? Reason) : IRequest;
