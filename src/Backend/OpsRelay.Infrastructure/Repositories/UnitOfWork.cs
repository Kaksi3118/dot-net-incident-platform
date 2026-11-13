using System.Text.Json;
using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Domain.Common;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Events;
using OpsRelay.Infrastructure.Persistence;
using OpsRelay.Infrastructure.Persistence.Entities;

namespace OpsRelay.Infrastructure.Repositories;

public class UnitOfWork(
    OpsRelayDbContext dbContext,
    IDomainEventDispatcher domainEventDispatcher) : IUnitOfWork
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = dbContext.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        List<IDomainEvent> domainEvents = [.. aggregates.SelectMany(a => a.DomainEvents)];

        foreach (var domainEvent in domainEvents)
        {
            dbContext.OutboxMessages.Add(CreateOutboxMessage(domainEvent));
        }

        var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        if (domainEvents.Count > 0)
        {
            await domainEventDispatcher.DispatchEventsAsync(domainEvents, cancellationToken);
        }

        return affectedRows;
    }

    private static OutboxMessage CreateOutboxMessage(IDomainEvent domainEvent)
    {
        var eventType = domainEvent.GetType();

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = eventType.FullName ?? eventType.Name,
            Payload = JsonSerializer.Serialize(domainEvent, eventType, SerializerOptions),
            OccurredOn = domainEvent.OccurredOn,
            Metadata = CreateMetadata(domainEvent)
        };
    }

    private static OutboxMetadata CreateMetadata(IDomainEvent domainEvent) =>
        domainEvent switch
        {
            IncidentCreatedDomainEvent created => new OutboxMetadata
            {
                AggregateType = nameof(Incident),
                AggregateId = created.IncidentId
            },
            IncidentStatusChangedDomainEvent changed => new OutboxMetadata
            {
                AggregateType = nameof(Incident),
                AggregateId = changed.IncidentId
            },
            SlaBreachedDomainEvent breached => new OutboxMetadata
            {
                AggregateType = nameof(Incident),
                AggregateId = breached.IncidentId
            },
            _ => new OutboxMetadata
            {
                AggregateType = domainEvent.GetType().Name,
                AggregateId = Guid.Empty
            }
        };
}
