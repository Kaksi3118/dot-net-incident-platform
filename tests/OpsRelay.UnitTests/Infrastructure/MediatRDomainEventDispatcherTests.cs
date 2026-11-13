using FluentAssertions;
using MediatR;
using NSubstitute;
using OpsRelay.Domain.Common;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Events;
using OpsRelay.Infrastructure.Events;
using Xunit;

namespace OpsRelay.UnitTests.Infrastructure;

public class MediatRDomainEventDispatcherTests
{
    [Fact]
    public async Task DispatchPublishesOneNotificationPerEvent()
    {
        var mediator = Substitute.For<IMediator>();
        mediator
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var dispatcher = new MediatRDomainEventDispatcher(mediator);
        var incidentId = Guid.NewGuid();
        var occurredOn = DateTimeOffset.UtcNow;

        IDomainEvent[] events =
        {
            new IncidentCreatedDomainEvent(
                incidentId, Priority.Critical, occurredOn, occurredOn.AddHours(4)),
            new IncidentStatusChangedDomainEvent(
                incidentId,
                IncidentStatus.Reported,
                IncidentStatus.Triaged,
                "Triaged",
                occurredOn)
        };

        await dispatcher.DispatchEventsAsync(events, CancellationToken.None);

        await mediator.Received(2)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }
}
