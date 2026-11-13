using FluentAssertions;
using NSubstitute;
using OpsRelay.Application.Incidents.Queries.GetIncidentsPaged;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Repositories;
using OpsRelay.UnitTests.Testing;
using Xunit;

namespace OpsRelay.UnitTests.Application;

public class GetIncidentsPagedQueryHandlerTests
{
    [Fact]
    public async Task HandleReturnsMappedPageWithCalculatedMetadata()
    {
        var incidents = new List<Incident>
        {
            IncidentFactory.Create(priority: Priority.Low),
            IncidentFactory.Create(priority: Priority.Critical)
        };

        var repository = Substitute.For<IIncidentRepository>();
        repository
            .GetPagedAsync(
                1,
                2,
                IncidentStatus.Reported,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<(IReadOnlyList<Incident> Items, int TotalCount)>(
                (incidents, 5)));

        var handler = new GetIncidentsPagedQueryHandler(repository);

        var result = await handler.Handle(
            new GetIncidentsPagedQuery(1, 2, IncidentStatus.Reported),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
        result.Items[0].AssignedTeamId.Should().Be("SRE-Ops");
    }
}
