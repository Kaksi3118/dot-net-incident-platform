using FluentAssertions;
using OpsRelay.Domain.Exceptions;
using OpsRelay.Domain.ValueObjects;
using Xunit;

namespace OpsRelay.UnitTests.Domain;

public class ValueObjectTests
{
    [Fact]
    public void IncidentTitleValidatesAndTrimsValue()
    {
        var title = IncidentTitle.Create("  Database outage  ");
        var sameTitle = IncidentTitle.Create("Database outage");

        title.Value.Should().Be("Database outage");
        title.Should().Be(sameTitle);
        (title == sameTitle).Should().BeTrue();
        (title != sameTitle).Should().BeFalse();
        title.GetHashCode().Should().Be(sameTitle.GetHashCode());

        Action shortTitle = () => IncidentTitle.Create("abcd");
        Action longTitle = () => IncidentTitle.Create(new string('x', 201));
        Action emptyTitle = () => IncidentTitle.Create("   ");

        shortTitle.Should().Throw<DomainException>();
        longTitle.Should().Throw<DomainException>();
        emptyTitle.Should().Throw<DomainException>();
    }

    [Fact]
    public void TeamIdValidatesAndUsesValueEquality()
    {
        var team = TeamId.Create("SRE-Ops");
        var sameTeam = TeamId.Create("SRE-Ops");
        var otherTeam = TeamId.Create("Payments");

        team.Should().Be(sameTeam);
        team.Should().NotBe(otherTeam);
        (team == sameTeam).Should().BeTrue();

        Action emptyTeam = () => TeamId.Create("   ");
        Action longTeam = () => TeamId.Create(new string('t', 51));

        emptyTeam.Should().Throw<DomainException>();
        longTeam.Should().Throw<DomainException>();
    }

    [Fact]
    public void DifferentValueObjectTypesAreNotEqual()
    {
        var title = IncidentTitle.Create("Database outage");
        var team = TeamId.Create("Database outage");

        title.Equals(team).Should().BeFalse();
        title.Equals(null).Should().BeFalse();
    }
}
