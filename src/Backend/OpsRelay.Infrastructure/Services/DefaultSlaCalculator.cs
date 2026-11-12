using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Infrastructure.Services;

public class DefaultSlaCalculator : ISlaCalculator
{
    public bool CanHandle(Priority priority) => true;

    public DateTimeOffset CalculateDeadline(
        DateTimeOffset reportedAt,
        SlaPolicy policy,
        TimeProvider timeProvider) =>
        reportedAt.AddHours(policy.ResolutionTimeHours);
}
