using OpsRelay.Domain.Enums;

namespace OpsRelay.Api.Models.Requests;

public sealed record UpdateStatusRequest(
    IncidentStatus NewStatus,
    string? Reason);
