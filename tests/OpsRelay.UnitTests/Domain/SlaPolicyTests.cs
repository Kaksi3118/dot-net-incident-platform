using FluentAssertions;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Exceptions;
using Xunit;

namespace OpsRelay.UnitTests.Domain;

public class SlaPolicyTests
{
    [Fact]
    public void ConstructorRejectsInvalidConfiguration()
    {
        Action blankName = () => _ = new SlaPolicy(
            Guid.NewGuid(), "   ", 1, 4, Priority.Critical);
        Action longName = () => _ = new SlaPolicy(
            Guid.NewGuid(), new string('n', 101), 1, 4, Priority.Critical);
        Action zeroResponse = () => _ = new SlaPolicy(
            Guid.NewGuid(), "Critical", 0, 4, Priority.Critical);
        Action zeroResolution = () => _ = new SlaPolicy(
            Guid.NewGuid(), "Critical", 1, 0, Priority.Critical);

        blankName.Should().Throw<DomainException>();
        longName.Should().Throw<DomainException>();
        zeroResponse.Should().Throw<DomainException>();
        zeroResolution.Should().Throw<DomainException>();
    }

    [Fact]
    public void ConstructorSetsValues()
    {
        var id = Guid.NewGuid();

        var policy = new SlaPolicy(
            id,
            "  Critical  ",
            responseTimeHours: 1,
            resolutionTimeHours: 4,
            Priority.Critical);

        policy.Id.Should().Be(id);
        policy.Name.Should().Be("Critical");
        policy.ResponseTimeHours.Should().Be(1);
        policy.ResolutionTimeHours.Should().Be(4);
        policy.Priority.Should().Be(Priority.Critical);
    }
}
