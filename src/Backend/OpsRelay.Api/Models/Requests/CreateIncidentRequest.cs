using OpsRelay.Domain.Enums;

namespace OpsRelay.Api.Models.Requests;

public sealed record CreateIncidentRequest(
    string Title,
    string Description,
    Priority Priority,
    string AssignedTeamId);
