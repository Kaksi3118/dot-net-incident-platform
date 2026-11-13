using MediatR;
using OpsRelay.Application.Common.Events;
using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Domain.Common;

namespace OpsRelay.Infrastructure.Events;

public class MediatRDomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchEventsAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in events)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var notification = Activator.CreateInstance(notificationType, domainEvent)
                ?? throw new InvalidOperationException(
                    $"Could not create a domain event notification for '{domainEvent.GetType().Name}'.");

            await mediator.Publish((INotification)notification, cancellationToken);
        }
    }
}
