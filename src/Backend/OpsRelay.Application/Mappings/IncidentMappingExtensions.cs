using OpsRelay.Application.DTOs;
using OpsRelay.Domain.Entities;

namespace OpsRelay.Application.Mappings;

public static class IncidentMappingExtensions
{
    public static IncidentDto ToDto(this Incident incident) =>
        new(
            incident.Id,
            incident.Title.Value,
            incident.Description,
            incident.Priority,
            incident.Status,
            incident.ReportedAt,
            incident.ResolvedAt,
            incident.SlaDeadline,
            incident.AssignedTeamId.Value);

    public static SlaPolicyDto ToDto(this SlaPolicy policy) =>
        new(
            policy.Id,
            policy.Name,
            policy.ResponseTimeHours,
            policy.ResolutionTimeHours,
            policy.Priority);
}
