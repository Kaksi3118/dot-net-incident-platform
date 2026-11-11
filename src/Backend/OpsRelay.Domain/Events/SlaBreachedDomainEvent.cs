using OpsRelay.Domain.Common;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Domain.Events;

public sealed record SlaBreachedDomainEvent(
    Guid IncidentId,
    Priority Priority,
    DateTimeOffset SlaDeadline,
    DateTimeOffset BreachedAt) : IDomainEvent
{
    public DateTimeOffset OccurredOn => BreachedAt;
}
