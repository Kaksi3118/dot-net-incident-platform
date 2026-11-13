using FluentAssertions;
using OpsRelay.Application.Incidents.Commands.UpdateIncidentStatus;
using OpsRelay.Domain.Enums;
using Xunit;

namespace OpsRelay.UnitTests.Application;

public class UpdateIncidentStatusCommandValidatorTests
{
    [Fact]
    public void ValidatorAcceptsValidCommand()
    {
        var validator = new UpdateIncidentStatusCommandValidator();

        var result = validator.Validate(
            new UpdateIncidentStatusCommand(
                Guid.NewGuid(),
                IncidentStatus.Triaged,
                "Triaged by on-call"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidatorRejectsMissingIdInvalidStatusAndLongReason()
    {
        var validator = new UpdateIncidentStatusCommandValidator();

        var result = validator.Validate(
            new UpdateIncidentStatusCommand(
                Guid.Empty,
                (IncidentStatus)0,
                new string('r', 501)));

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName)
            .Should().Contain(["Id", "NewStatus", "Reason"]);
    }
}
