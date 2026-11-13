using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Events;
using OpsRelay.Domain.Exceptions;
using OpsRelay.Domain.ValueObjects;
using OpsRelay.UnitTests.Testing;
using Xunit;

namespace OpsRelay.UnitTests.Domain;

public class IncidentAggregateTests
{
    [Fact]
    public void ConstructorRejectsMissingValues()
    {
        var title = IncidentTitle.Create("Database outage");
        var team = TeamId.Create("SRE-Ops");

        Action missingTitle = () => _ = new Incident(
            Guid.NewGuid(), null!, "description", Priority.Low, team,
            IncidentFactory.ReportedAt, IncidentFactory.ReportedAt.AddHours(4));

        Action missingDescription = () => _ = new Incident(
            Guid.NewGuid(), title, null!, Priority.Low, team,
            IncidentFactory.ReportedAt, IncidentFactory.ReportedAt.AddHours(4));

        Action blankDescription = () => _ = new Incident(
            Guid.NewGuid(), title, "   ", Priority.Low, team,
            IncidentFactory.ReportedAt, IncidentFactory.ReportedAt.AddHours(4));

        Action missingTeam = () => _ = new Incident(
            Guid.NewGuid(), title, "description", Priority.Low, null!,
            IncidentFactory.ReportedAt, IncidentFactory.ReportedAt.AddHours(4));

        missingTitle.Should().Throw<DomainException>();
        missingDescription.Should().Throw<DomainException>();
        blankDescription.Should().Throw<DomainException>();
        missingTeam.Should().Throw<DomainException>();
    }

    [Fact]
    public void ConstructorStagesCreationEventAndInitialisesState()
    {
        var incident = new Incident(
            Guid.Empty,
            IncidentTitle.Create("Database outage"),
            "Primary database unavailable",
            Priority.Critical,
            TeamId.Create("SRE-Ops"),
            IncidentFactory.ReportedAt,
            IncidentFactory.ReportedAt.AddHours(4));

        incident.Id.Should().NotBe(Guid.Empty);
        incident.Status.Should().Be(IncidentStatus.Reported);
        incident.ResolvedAt.Should().BeNull();
        incident.RowVersion.Should().NotBeNull();
        incident.RowVersion.Should().BeEmpty();

        var created = incident.DomainEvents.Single().Should()
            .BeOfType<IncidentCreatedDomainEvent>().Subject;

        created.IncidentId.Should().Be(incident.Id);
        created.Priority.Should().Be(Priority.Critical);
        created.ReportedAt.Should().Be(IncidentFactory.ReportedAt);
        created.SlaDeadline.Should().Be(IncidentFactory.ReportedAt.AddHours(4));

        incident.ClearDomainEvents();
        incident.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ChangeStatusAppliesTransitionAndStagesEvent()
    {
        var incident = IncidentFactory.Create();
        var timeProvider = new FakeTimeProvider(IncidentFactory.ReportedAt.AddHours(1));

        incident.ChangeStatus(IncidentStatus.Triaged, "Triage complete", timeProvider);

        incident.Status.Should().Be(IncidentStatus.Triaged);
        incident.ResolvedAt.Should().BeNull();

        var changed = incident.DomainEvents.Single().Should()
            .BeOfType<IncidentStatusChangedDomainEvent>().Subject;

        changed.IncidentId.Should().Be(incident.Id);
        changed.PreviousStatus.Should().Be(IncidentStatus.Reported);
        changed.NewStatus.Should().Be(IncidentStatus.Triaged);
        changed.Reason.Should().Be("Triage complete");
        changed.ChangedAt.Should().Be(timeProvider.GetUtcNow());
    }

    [Fact]
    public void ChangeStatusRejectsInvalidTransition()
    {
        var incident = IncidentFactory.Create();
        var timeProvider = new FakeTimeProvider(IncidentFactory.ReportedAt.AddHours(1));

        Action transition = () =>
            incident.ChangeStatus(IncidentStatus.Resolved, "Skip triage", timeProvider);

        var exception = transition.Should()
            .Throw<InvalidStatusTransitionException>().Which;

        exception.CurrentStatus.Should().Be(IncidentStatus.Reported);
        exception.TargetStatus.Should().Be(IncidentStatus.Resolved);
        incident.Status.Should().Be(IncidentStatus.Reported);
        incident.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ResolvedTimestampFollowsTimeProviderAndClearsOnReopen()
    {
        var incident = IncidentFactory.Create();
        var timeProvider = new FakeTimeProvider(IncidentFactory.ReportedAt.AddHours(1));

        incident.ChangeStatus(IncidentStatus.Triaged, "Triaged", timeProvider);
        incident.ChangeStatus(IncidentStatus.InProgress, "Working", timeProvider);

        timeProvider.Advance(TimeSpan.FromHours(2));
        incident.ChangeStatus(IncidentStatus.Resolved, "Fixed", timeProvider);

        incident.ResolvedAt.Should().Be(timeProvider.GetUtcNow());

        incident.ChangeStatus(IncidentStatus.InProgress, "Regression", timeProvider);

        incident.ResolvedAt.Should().BeNull();
    }

    [Fact]
    public void ClosedIncidentCannotTransition()
    {
        var incident = IncidentFactory.Create();
        var timeProvider = new FakeTimeProvider(IncidentFactory.ReportedAt.AddHours(1));

        incident.ChangeStatus(IncidentStatus.Closed, "Duplicate", timeProvider);

        Action transition = () =>
            incident.ChangeStatus(IncidentStatus.InProgress, "Reopen", timeProvider);

        transition.Should().Throw<InvalidStatusTransitionException>();
        incident.Status.Should().Be(IncidentStatus.Closed);
    }

    [Fact]
    public void EntitiesWithSameIdentifierAreEqual()
    {
        var id = Guid.NewGuid();
        var first = IncidentFactory.Create(id: id);
        var second = IncidentFactory.Create(id: id);
        var other = IncidentFactory.Create();

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
        first.Should().NotBe(other);
    }
}

