using OpsRelay.Domain.Enums;

namespace OpsRelay.Api.Models.Requests;

/// <summary>
/// Payload for creating a new incident.
/// </summary>
/// <param name="Title">Short incident title (5-200 characters).</param>
/// <param name="Description">Detailed incident description (maximum 2000 characters).</param>
/// <param name="Priority">Incident priority (Low, Medium, High or Critical).</param>
/// <param name="AssignedTeamId">Identifier of the team that owns the incident.</param>
public sealed record CreateIncidentRequest(
    string Title,
    string Description,
    Priority Priority,
    string AssignedTeamId);
