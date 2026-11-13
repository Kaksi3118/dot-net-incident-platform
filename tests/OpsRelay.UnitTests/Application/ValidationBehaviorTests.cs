using FluentAssertions;
using FluentValidation;
using OpsRelay.Application.Common.Behaviors;
using OpsRelay.Application.DTOs;
using OpsRelay.Application.Incidents.Commands.CreateIncident;
using OpsRelay.Domain.Enums;
using Xunit;

namespace OpsRelay.UnitTests.Application;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task HandleInvokesNextWhenRequestIsValid()
    {
        var response = CreateResponse();
        var nextCalled = false;

        var behavior = new ValidationBehavior<CreateIncidentCommand, IncidentDto>(
            [new CreateIncidentCommandValidator()]);

        var result = await behavior.Handle(
            new CreateIncidentCommand(
                "Database outage",
                "Primary database unavailable",
                Priority.High,
                "SRE-Ops"),
            () =>
            {
                nextCalled = true;
                return Task.FromResult(response);
            },
            CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.Should().BeSameAs(response);
    }

    [Fact]
    public async Task HandleThrowsValidationExceptionWhenRequestIsInvalid()
    {
        var behavior = new ValidationBehavior<CreateIncidentCommand, IncidentDto>(
            [new CreateIncidentCommandValidator()]);

        var invalidCommand = new CreateIncidentCommand(
            "bad",
            string.Empty,
            (Priority)0,
            string.Empty);

        var exception = await FluentActions.Awaiting(() =>
                behavior.Handle(
                    invalidCommand,
                    () => Task.FromResult(CreateResponse()),
                    CancellationToken.None))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Select(error => error.PropertyName)
            .Should().Contain(["Title", "Description", "Priority", "AssignedTeamId"]);
    }

    [Fact]
    public async Task HandleInvokesNextWhenNoValidatorsAreRegistered()
    {
        var response = CreateResponse();
        var nextCalled = false;

        var behavior = new ValidationBehavior<CreateIncidentCommand, IncidentDto>([]);

        var result = await behavior.Handle(
            new CreateIncidentCommand(
                "Database outage",
                "Primary database unavailable",
                Priority.High,
                "SRE-Ops"),
            () =>
            {
                nextCalled = true;
                return Task.FromResult(response);
            },
            CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.Should().BeSameAs(response);
    }

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

