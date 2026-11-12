namespace OpsRelay.Infrastructure.Persistence.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public string EventType { get; set; } = string.Empty;

    public OutboxMetadata Metadata { get; set; } = new();

    public string Payload { get; set; } = string.Empty;

    public DateTimeOffset OccurredOn { get; set; }

    public DateTimeOffset? ProcessedOn { get; set; }
}
