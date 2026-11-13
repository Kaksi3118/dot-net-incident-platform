using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Domain.Services;

public interface ISlaCalculator
{
    bool CanHandle(Priority priority);

    SlaCalculationResult CalculateDeadline(
        DateTimeOffset reportedAt,
        SlaPolicy policy,
        TimeProvider timeProvider);
}
