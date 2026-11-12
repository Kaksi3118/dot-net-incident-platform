namespace OpsRelay.Infrastructure.Persistence.Entities;

public class OutboxMetadata
{
    public string? CorrelationId { get; set; }

    public string? AggregateType { get; set; }

    public Guid AggregateId { get; set; }
}
