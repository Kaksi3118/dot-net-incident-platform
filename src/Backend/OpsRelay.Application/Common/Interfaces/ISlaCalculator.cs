using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Application.Common.Interfaces;

public interface ISlaCalculator
{
    bool CanHandle(Priority priority);

    DateTimeOffset CalculateDeadline(
        DateTimeOffset reportedAt,
        SlaPolicy policy,
        TimeProvider timeProvider);
}
