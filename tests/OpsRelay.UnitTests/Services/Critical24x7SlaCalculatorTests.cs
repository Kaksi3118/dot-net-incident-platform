using System.Globalization;
using FluentAssertions;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Infrastructure.Services;
using Xunit;

namespace OpsRelay.UnitTests.Services;

public class Critical24x7SlaCalculatorTests
{
    private readonly Critical24x7SlaCalculator _calculator = new();

    [Theory]
    [InlineData(Priority.Low, false)]
    [InlineData(Priority.Medium, false)]
    [InlineData(Priority.High, true)]
    [InlineData(Priority.Critical, true)]
    public void CanHandleMatchesHighAndCriticalPriorities(Priority priority, bool expected) =>
        _calculator.CanHandle(priority).Should().Be(expected);

    [Fact]
    public void CalculateDeadlineCountsWeekendHoursContinuously()
    {
        var reportedAt = Parse("2026-11-13T23:00:00Z");
        var policy = new SlaPolicy(
            Guid.NewGuid(),
            "Critical",
            responseTimeHours: 1,
            resolutionTimeHours: 4,
            Priority.Critical);

        var result = _calculator.CalculateDeadline(
            reportedAt,
            policy,
            TimeProvider.System);

        result.ResponseDeadline.Should().Be(Parse("2026-11-14T00:00:00Z"));
        result.ResolutionDeadline.Should().Be(Parse("2026-11-14T03:00:00Z"));
        result.IncludesNonWorkingHours.Should().BeTrue();
        result.StrategyName.Should().Be(nameof(Critical24x7SlaCalculator));
    }

    [Fact]
    public void CalculateDeadlineDoesNotMutateInput()
    {
        var reportedAt = Parse("2026-11-13T23:00:00Z");
        var original = reportedAt;
        var policy = new SlaPolicy(
            Guid.NewGuid(),
            "Critical",
            responseTimeHours: 1,
            resolutionTimeHours: 4,
            Priority.Critical);

        _ = _calculator.CalculateDeadline(reportedAt, policy, TimeProvider.System);

        reportedAt.Should().Be(original);
    }

    private static DateTimeOffset Parse(string value) =>
        DateTimeOffset.Parse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind);
}
