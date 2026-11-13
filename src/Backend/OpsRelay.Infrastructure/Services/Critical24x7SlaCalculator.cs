using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Services;

namespace OpsRelay.Infrastructure.Services;

public class Critical24x7SlaCalculator : ISlaCalculator
{
    public bool CanHandle(Priority priority) =>
        priority is Priority.High or Priority.Critical;

    public SlaCalculationResult CalculateDeadline(
        DateTimeOffset reportedAt,
        SlaPolicy policy,
        TimeProvider timeProvider) =>
        new(
            reportedAt.AddHours(policy.ResponseTimeHours),
            reportedAt.AddHours(policy.ResolutionTimeHours),
            IncludesNonWorkingHours: true,
            StrategyName: nameof(Critical24x7SlaCalculator));
}
