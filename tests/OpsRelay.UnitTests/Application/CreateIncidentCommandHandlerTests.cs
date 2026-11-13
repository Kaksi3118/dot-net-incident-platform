using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Application.Incidents.Commands.CreateIncident;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Exceptions;
using OpsRelay.Domain.Repositories;
using OpsRelay.Domain.Services;
using OpsRelay.UnitTests.Testing;
using Xunit;

namespace OpsRelay.UnitTests.Application;

public class CreateIncidentCommandHandlerTests
{
    private static readonly DateTimeOffset Now = IncidentFactory.ReportedAt;

    [Fact]
    public async Task HandlePersistsIncidentWithCalculatedDeadline()
    {
        var repository = Substitute.For<IIncidentRepository>();
        var policyRepository = Substitute.For<ISlaPolicyRepository>();
        var calculatorFactory = Substitute.For<ISlaCalculatorFactory>();
        var calculator = Substitute.For<ISlaCalculator>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var timeProvider = new FakeTimeProvider(Now);

        var policy = new SlaPolicy(
            Guid.NewGuid(), "Critical", 1, 4, Priority.Critical);
        var calculation = new SlaCalculationResult(
            Now.AddHours(1), Now.AddHours(4), false, "TestStrategy");

        policyRepository
            .GetByPriorityAsync(Priority.Critical, Arg.Any<CancellationToken>())
            .Returns(policy);
        calculatorFactory.GetCalculator(Priority.Critical).Returns(calculator);
        calculator
            .CalculateDeadline(
                Arg.Any<DateTimeOffset>(),
                Arg.Any<SlaPolicy>(),
                Arg.Any<TimeProvider>())
            .Returns(calculation);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateIncidentCommandHandler(
            repository,
            policyRepository,
            calculatorFactory,
            unitOfWork,
            timeProvider,
            NullLogger<CreateIncidentCommandHandler>.Instance);

        var command = new CreateIncidentCommand(
            "Database outage",
            "Primary database unavailable",
            Priority.Critical,
            "SRE-Ops");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Title.Should().Be("Database outage");
        result.Description.Should().Be("Primary database unavailable");
        result.Priority.Should().Be(Priority.Critical);
        result.Status.Should().Be(IncidentStatus.Reported);
        result.ReportedAt.Should().Be(Now);
        result.SlaDeadline.Should().Be(Now.AddHours(4));
        result.AssignedTeamId.Should().Be("SRE-Ops");

        await repository.Received(1).AddAsync(
            Arg.Is<Incident>(incident =>
                incident.Title.Value == command.Title &&
                incident.Description == command.Description &&
                incident.Priority == Priority.Critical &&
                incident.Status == IncidentStatus.Reported &&
                incident.ReportedAt == Now &&
                incident.SlaDeadline == Now.AddHours(4) &&
                incident.AssignedTeamId.Value == command.AssignedTeamId),
            Arg.Any<CancellationToken>());

        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleThrowsWhenPolicyIsMissing()
    {
        var repository = Substitute.For<IIncidentRepository>();
        var policyRepository = Substitute.For<ISlaPolicyRepository>();
        var calculatorFactory = Substitute.For<ISlaCalculatorFactory>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var timeProvider = new FakeTimeProvider(Now);

        policyRepository
            .GetByPriorityAsync(Priority.Low, Arg.Any<CancellationToken>())
            .Returns((SlaPolicy?)null);

        var handler = new CreateIncidentCommandHandler(
            repository,
            policyRepository,
            calculatorFactory,
            unitOfWork,
            timeProvider,
            NullLogger<CreateIncidentCommandHandler>.Instance);

        var command = new CreateIncidentCommand(
            "Database outage",
            "Primary database unavailable",
            Priority.Low,
            "SRE-Ops");

        await FluentActions.Awaiting(() => handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>();

        calculatorFactory.DidNotReceive().GetCalculator(Arg.Any<Priority>());
        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
