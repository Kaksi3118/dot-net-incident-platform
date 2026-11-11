using OpsRelay.Domain.Common;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Domain.Events;

public sealed record IncidentStatusChangedDomainEvent(
    Guid IncidentId,
    IncidentStatus PreviousStatus,
    IncidentStatus NewStatus,
    string? Reason,
    DateTimeOffset ChangedAt) : IDomainEvent
{
    public DateTimeOffset OccurredOn => ChangedAt;
}
