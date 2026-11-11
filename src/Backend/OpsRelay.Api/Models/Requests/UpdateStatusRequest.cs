using OpsRelay.Domain.Enums;

namespace OpsRelay.Api.Models.Requests;

/// <summary>
/// Payload for changing an incident status.
/// </summary>
/// <param name="NewStatus">Target lifecycle status.</param>
/// <param name="Reason">Optional explanation for the transition.</param>
public sealed record UpdateStatusRequest(
    IncidentStatus NewStatus,
    string? Reason);
