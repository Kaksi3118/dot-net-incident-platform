using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Application.Incidents.Commands.UpdateIncidentStatus;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Exceptions;
using OpsRelay.Domain.Repositories;
using OpsRelay.UnitTests.Testing;
using Xunit;

namespace OpsRelay.UnitTests.Application;

public class UpdateIncidentStatusCommandHandlerTests
{
    [Fact]
    public async Task HandleChangesStatusAndCommits()
    {
        var incident = IncidentFactory.Create();
        var repository = Substitute.For<IIncidentRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var timeProvider = new FakeTimeProvider(IncidentFactory.ReportedAt.AddHours(1));

        repository
            .GetByIdAsync(incident.Id, Arg.Any<CancellationToken>())
            .Returns(incident);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new UpdateIncidentStatusCommandHandler(
            repository,
            unitOfWork,
            timeProvider,
            NullLogger<UpdateIncidentStatusCommandHandler>.Instance);

        await handler.Handle(
            new UpdateIncidentStatusCommand(
                incident.Id,
                IncidentStatus.Triaged,
                "Triaged by on-call"),
            CancellationToken.None);

        incident.Status.Should().Be(IncidentStatus.Triaged);
        AssertStatusChangeEventStaged(incident);
        repository.Received(1).Update(incident);
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleThrowsWhenIncidentIsMissing()
    {
        var repository = Substitute.For<IIncidentRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var timeProvider = new FakeTimeProvider(IncidentFactory.ReportedAt);

        repository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Incident?)null);

        var handler = new UpdateIncidentStatusCommandHandler(
            repository,
            unitOfWork,
            timeProvider,
            NullLogger<UpdateIncidentStatusCommandHandler>.Instance);

        await FluentActions.Awaiting(() =>
                handler.Handle(
                    new UpdateIncidentStatusCommand(
                        Guid.NewGuid(),
                        IncidentStatus.Triaged,
                        "missing"),
                    CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleRejectsInvalidTransition()
    {
        var incident = IncidentFactory.Create();
        var repository = Substitute.For<IIncidentRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var timeProvider = new FakeTimeProvider(IncidentFactory.ReportedAt);

        repository
            .GetByIdAsync(incident.Id, Arg.Any<CancellationToken>())
            .Returns(incident);

        var handler = new UpdateIncidentStatusCommandHandler(
            repository,
            unitOfWork,
            timeProvider,
            NullLogger<UpdateIncidentStatusCommandHandler>.Instance);

        await FluentActions.Awaiting(() =>
                handler.Handle(
                    new UpdateIncidentStatusCommand(
                        incident.Id,
                        IncidentStatus.Resolved,
                        "skip"),
                    CancellationToken.None))
            .Should().ThrowAsync<InvalidStatusTransitionException>();

        repository.DidNotReceive().Update(Arg.Any<Incident>());
        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    private static void AssertStatusChangeEventStaged(Incident incident) =>
        incident.DomainEvents.Should().ContainSingle();
}
