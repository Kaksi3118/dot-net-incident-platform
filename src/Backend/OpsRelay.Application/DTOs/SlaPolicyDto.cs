using OpsRelay.Domain.Enums;

namespace OpsRelay.Application.DTOs;

public sealed record SlaPolicyDto(
    Guid Id,
    string Name,
    int ResponseTimeHours,
    int ResolutionTimeHours,
    Priority Priority);
