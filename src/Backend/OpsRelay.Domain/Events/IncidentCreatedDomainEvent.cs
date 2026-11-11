using OpsRelay.Domain.Common;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Domain.Events;

public sealed record IncidentCreatedDomainEvent(
    Guid IncidentId,
    Priority Priority,
    DateTimeOffset ReportedAt,
    DateTimeOffset SlaDeadline) : IDomainEvent
{
    public DateTimeOffset OccurredOn => ReportedAt;
}
