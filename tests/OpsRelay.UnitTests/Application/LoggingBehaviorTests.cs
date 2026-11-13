using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using OpsRelay.Application.Common.Behaviors;
using OpsRelay.Application.DTOs;
using OpsRelay.Application.Incidents.Commands.CreateIncident;
using OpsRelay.Domain.Enums;
using Xunit;

namespace OpsRelay.UnitTests.Application;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task HandleReturnsResponseWhenNextSucceeds()
    {
        var response = CreateResponse();
        var behavior = new LoggingBehavior<CreateIncidentCommand, IncidentDto>(
            NullLogger<LoggingBehavior<CreateIncidentCommand, IncidentDto>>.Instance);

        var result = await behavior.Handle(
            CreateCommand(),
            () => Task.FromResult(response),
            CancellationToken.None);

        result.Should().BeSameAs(response);
    }

    [Fact]
    public async Task HandleRethrowsWhenNextFails()
    {
        var behavior = new LoggingBehavior<CreateIncidentCommand, IncidentDto>(
            NullLogger<LoggingBehavior<CreateIncidentCommand, IncidentDto>>.Instance);

        var exception = new InvalidOperationException("boom");

        await FluentActions.Awaiting(() =>
                behavior.Handle(
                    CreateCommand(),
                    () => Task.FromException<IncidentDto>(exception),
                    CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("boom");
    }

    private static CreateIncidentCommand CreateCommand() =>
        new(
            "Database outage",
            "Primary database unavailable",
            Priority.High,
            "SRE-Ops");

    private static IncidentDto CreateResponse() =>
        new(
            Guid.NewGuid(),
            "Database outage",
            "Primary database unavailable",
            Priority.High,
            IncidentStatus.Reported,
            DateTimeOffset.UtcNow,
            null,
            DateTimeOffset.UtcNow.AddHours(8),
            "SRE-Ops");
}

