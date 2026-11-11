using OpsRelay.Domain.Enums;

namespace OpsRelay.Application.DTOs;

public sealed record IncidentDto(
    Guid Id,
    string Title,
    string Description,
    Priority Priority,
    IncidentStatus Status,
    DateTimeOffset ReportedAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset SlaDeadline,
    string AssignedTeamId);
