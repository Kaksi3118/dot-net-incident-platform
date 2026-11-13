using FluentAssertions;
using OpsRelay.Application.Mappings;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.UnitTests.Testing;
using Xunit;

namespace OpsRelay.UnitTests.Application;

public class IncidentMappingExtensionsTests
{
    [Fact]
    public void IncidentMapsToDto()
    {
        var incident = IncidentFactory.Create();

        var dto = incident.ToDto();

        dto.Id.Should().Be(incident.Id);
        dto.Title.Should().Be(incident.Title.Value);
        dto.Description.Should().Be(incident.Description);
        dto.Priority.Should().Be(incident.Priority);
        dto.Status.Should().Be(incident.Status);
        dto.ReportedAt.Should().Be(incident.ReportedAt);
        dto.ResolvedAt.Should().Be(incident.ResolvedAt);
        dto.SlaDeadline.Should().Be(incident.SlaDeadline);
        dto.AssignedTeamId.Should().Be(incident.AssignedTeamId.Value);
    }

    [Fact]
    public void SlaPolicyMapsToDto()
    {
        var policy = new SlaPolicy(
            Guid.NewGuid(),
            "Critical",
            1,
            4,
            Priority.Critical);

        var dto = policy.ToDto();

        dto.Id.Should().Be(policy.Id);
        dto.Name.Should().Be(policy.Name);
        dto.ResponseTimeHours.Should().Be(policy.ResponseTimeHours);
        dto.ResolutionTimeHours.Should().Be(policy.ResolutionTimeHours);
        dto.Priority.Should().Be(policy.Priority);
    }
}
