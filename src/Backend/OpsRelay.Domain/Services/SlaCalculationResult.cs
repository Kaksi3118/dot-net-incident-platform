namespace OpsRelay.Domain.Services;

public sealed record SlaCalculationResult(
    DateTimeOffset ResponseDeadline,
    DateTimeOffset ResolutionDeadline,
    bool IncludesNonWorkingHours,
    string StrategyName);
