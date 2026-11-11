namespace OpsRelay.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
