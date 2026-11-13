using System.Globalization;
using FluentAssertions;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Infrastructure.Services;
using Xunit;

namespace OpsRelay.UnitTests.Services;

public class StandardBusinessHoursSlaCalculatorTests
{
    private readonly StandardBusinessHoursSlaCalculator _calculator = new();

    [Theory]
    [InlineData(Priority.Low, true)]
    [InlineData(Priority.Medium, true)]
    [InlineData(Priority.High, false)]
    [InlineData(Priority.Critical, false)]
    public void CanHandleMatchesLowAndMediumPriorities(Priority priority, bool expected) =>
        _calculator.CanHandle(priority).Should().Be(expected);

    [Theory]
    [InlineData("2026-11-13T16:00:00Z", 4, "2026-11-16T12:00:00Z")]
    [InlineData("2026-11-14T11:00:00Z", 2, "2026-11-16T11:00:00Z")]
    [InlineData("2026-11-17T08:30:00Z", 4, "2026-11-17T13:00:00Z")]
    [InlineData("2026-11-13T16:30:00Z", 2, "2026-11-16T10:30:00Z")]
    [InlineData("2026-11-13T16:30:00Z", 10, "2026-11-17T10:30:00Z")]
    [InlineData("2026-11-14T14:00:00Z", 4, "2026-11-16T13:00:00Z")]
    [InlineData("2026-11-16T08:00:00Z", 4, "2026-11-16T13:00:00Z")]
    public void CalculateDeadlineSkipsNightsAndWeekends(
        string reportedAtText,
        int resolutionHours,
        string expectedResolutionText)
    {
        var reportedAt = Parse(reportedAtText);
        var expectedResolution = Parse(expectedResolutionText);
        var policy = CreatePolicy(resolutionHours);

        var result = _calculator.CalculateDeadline(
            reportedAt,
            policy,
            TimeProvider.System);

        result.ResolutionDeadline.Should().Be(expectedResolution);
        result.IncludesNonWorkingHours.Should().BeFalse();
        result.StrategyName.Should().Be(nameof(StandardBusinessHoursSlaCalculator));
    }

    [Fact]
    public void CalculateDeadlineComputesResponseAndResolutionDeadlines()
    {
        var reportedAt = Parse("2026-11-13T16:00:00Z");
        var policy = new SlaPolicy(
            Guid.NewGuid(),
            "Standard",
            responseTimeHours: 2,
            resolutionTimeHours: 4,
            Priority.Medium);

        var result = _calculator.CalculateDeadline(
            reportedAt,
            policy,
            TimeProvider.System);

        result.ResponseDeadline.Should().Be(Parse("2026-11-16T10:00:00Z"));
        result.ResolutionDeadline.Should().Be(Parse("2026-11-16T12:00:00Z"));
    }

    [Fact]
    public void CalculateDeadlineDoesNotMutateInput()
    {
        var reportedAt = Parse("2026-11-13T16:30:00Z");
        var original = reportedAt;
        var policy = CreatePolicy(2);

        _ = _calculator.CalculateDeadline(reportedAt, policy, TimeProvider.System);

        reportedAt.Should().Be(original);
    }

    private static SlaPolicy CreatePolicy(int resolutionHours) =>
        new(
            Guid.NewGuid(),
            "Standard",
            responseTimeHours: 1,
            resolutionTimeHours: resolutionHours,
            Priority.Medium);

    private static DateTimeOffset Parse(string value) =>
        DateTimeOffset.Parse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind);
}
