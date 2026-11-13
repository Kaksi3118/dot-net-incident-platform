using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Services;

namespace OpsRelay.Infrastructure.Services;

public class DefaultSlaCalculator : ISlaCalculator
{
    public bool CanHandle(Priority priority) => true;

    public SlaCalculationResult CalculateDeadline(
        DateTimeOffset reportedAt,
        SlaPolicy policy,
        TimeProvider timeProvider) =>
        new(
            reportedAt.AddHours(policy.ResponseTimeHours),
            reportedAt.AddHours(policy.ResolutionTimeHours),
            IncludesNonWorkingHours: true,
            StrategyName: nameof(DefaultSlaCalculator));
}
