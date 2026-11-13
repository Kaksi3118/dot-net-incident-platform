using FluentAssertions;
using NSubstitute;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Services;
using OpsRelay.Infrastructure.Services;
using Xunit;

namespace OpsRelay.UnitTests.Services;

public class SlaCalculatorFactoryTests
{
    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    public void GetCalculatorResolvesBusinessHoursStrategy(Priority priority)
    {
        var factory = CreateFactory();

        factory.GetCalculator(priority)
            .Should().BeOfType<StandardBusinessHoursSlaCalculator>();
    }

    [Theory]
    [InlineData(Priority.High)]
    [InlineData(Priority.Critical)]
    public void GetCalculatorResolvesCriticalStrategy(Priority priority)
    {
        var factory = CreateFactory();

        factory.GetCalculator(priority)
            .Should().BeOfType<Critical24x7SlaCalculator>();
    }

    [Fact]
    public void GetCalculatorSelectsFirstMatchingStrategy()
    {
        var businessHours = Substitute.For<ISlaCalculator>();
        businessHours.CanHandle(Priority.Low).Returns(true);

        var critical = Substitute.For<ISlaCalculator>();
        critical.CanHandle(Priority.Critical).Returns(true);

        var factory = new SlaCalculatorFactory([businessHours, critical]);

        factory.GetCalculator(Priority.Low).Should().BeSameAs(businessHours);
        factory.GetCalculator(Priority.Critical).Should().BeSameAs(critical);
    }

    [Fact]
    public void GetCalculatorThrowsForUnknownPriority()
    {
        var factory = CreateFactory();

        Action resolve = () => factory.GetCalculator((Priority)99);

        resolve.Should().Throw<InvalidOperationException>()
            .WithMessage("*99*");
    }

    [Fact]
    public void GetCalculatorThrowsWhenNoStrategyIsRegistered()
    {
        var factory = new SlaCalculatorFactory([]);

        Action resolve = () => factory.GetCalculator(Priority.Low);

        resolve.Should().Throw<InvalidOperationException>();
    }

    private static SlaCalculatorFactory CreateFactory() =>
        new(
        [
            new StandardBusinessHoursSlaCalculator(),
            new Critical24x7SlaCalculator()
        ]);
}
