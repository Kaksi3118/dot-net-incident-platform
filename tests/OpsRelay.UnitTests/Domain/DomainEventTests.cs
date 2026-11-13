using FluentAssertions;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Events;
using OpsRelay.Domain.Exceptions;
using Xunit;

namespace OpsRelay.UnitTests.Domain;

public class DomainEventTests
{
    [Fact]
    public void IncidentCreatedEventExposesPayloadAndOccurrence()
    {
        var occurredOn = DateTimeOffset.UtcNow;
        var deadline = occurredOn.AddHours(4);
        var incidentId = Guid.NewGuid();

        var domainEvent = new IncidentCreatedDomainEvent(
            incidentId, Priority.Critical, occurredOn, deadline);

        domainEvent.IncidentId.Should().Be(incidentId);
        domainEvent.Priority.Should().Be(Priority.Critical);
        domainEvent.ReportedAt.Should().Be(occurredOn);
        domainEvent.SlaDeadline.Should().Be(deadline);
        domainEvent.OccurredOn.Should().Be(occurredOn);
    }

    [Fact]
    public void IncidentStatusChangedEventExposesPayloadAndOccurrence()
    {
        var occurredOn = DateTimeOffset.UtcNow;
        var incidentId = Guid.NewGuid();

        var domainEvent = new IncidentStatusChangedDomainEvent(
            incidentId,
            IncidentStatus.Reported,
            IncidentStatus.Triaged,
            "Triaged",
            occurredOn);

        domainEvent.IncidentId.Should().Be(incidentId);
        domainEvent.PreviousStatus.Should().Be(IncidentStatus.Reported);
        domainEvent.NewStatus.Should().Be(IncidentStatus.Triaged);
        domainEvent.Reason.Should().Be("Triaged");
        domainEvent.OccurredOn.Should().Be(occurredOn);
    }

    [Fact]
    public void SlaBreachedEventExposesPayloadAndOccurrence()
    {
        var occurredOn = DateTimeOffset.UtcNow;
        var deadline = occurredOn.AddMinutes(-10);
        var incidentId = Guid.NewGuid();

        var domainEvent = new SlaBreachedDomainEvent(
            incidentId, Priority.High, deadline, occurredOn);

        domainEvent.IncidentId.Should().Be(incidentId);
        domainEvent.Priority.Should().Be(Priority.High);
        domainEvent.SlaDeadline.Should().Be(deadline);
        domainEvent.BreachedAt.Should().Be(occurredOn);
        domainEvent.OccurredOn.Should().Be(occurredOn);
    }

    [Fact]
    public void DomainExceptionPreservesInnerException()
    {
        var inner = new InvalidOperationException("inner");

        var exception = new DomainException("outer", inner);

        exception.Message.Should().Be("outer");
        exception.InnerException.Should().BeSameAs(inner);
    }
}
