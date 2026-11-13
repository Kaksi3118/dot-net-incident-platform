using FluentAssertions;
using NSubstitute;
using OpsRelay.Application.Incidents.Queries.GetIncidentById;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Repositories;
using OpsRelay.UnitTests.Testing;
using Xunit;

namespace OpsRelay.UnitTests.Application;

public class GetIncidentByIdQueryHandlerTests
{
    [Fact]
    public async Task HandleReturnsMappedIncidentWhenFound()
    {
        var incident = IncidentFactory.Create();
        var repository = Substitute.For<IIncidentRepository>();

        repository
            .GetByIdAsync(incident.Id, Arg.Any<CancellationToken>())
            .Returns(incident);

        var handler = new GetIncidentByIdQueryHandler(repository);

        var result = await handler.Handle(
            new GetIncidentByIdQuery(incident.Id),
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(incident.Id);
        result.Title.Should().Be(incident.Title.Value);
        result.Priority.Should().Be(Priority.Critical);
        result.Status.Should().Be(IncidentStatus.Reported);
        result.AssignedTeamId.Should().Be(incident.AssignedTeamId.Value);
    }

    [Fact]
    public async Task HandleReturnsNullWhenMissing()
    {
        var repository = Substitute.For<IIncidentRepository>();

        repository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Incident?)null);

        var handler = new GetIncidentByIdQueryHandler(repository);

        var result = await handler.Handle(
            new GetIncidentByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        result.Should().BeNull();
    }
}
