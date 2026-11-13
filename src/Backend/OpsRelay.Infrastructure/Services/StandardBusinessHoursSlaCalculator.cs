using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Services;

namespace OpsRelay.Infrastructure.Services;

public class StandardBusinessHoursSlaCalculator : ISlaCalculator
{
    private static readonly TimeSpan BusinessStart = new(9, 0, 0);
    private static readonly TimeSpan BusinessEnd = new(17, 0, 0);

    public bool CanHandle(Priority priority) =>
        priority is Priority.Low or Priority.Medium;

    public SlaCalculationResult CalculateDeadline(
        DateTimeOffset reportedAt,
        SlaPolicy policy,
        TimeProvider timeProvider) =>
        new(
            AddBusinessHours(reportedAt, policy.ResponseTimeHours),
            AddBusinessHours(reportedAt, policy.ResolutionTimeHours),
            IncludesNonWorkingHours: false,
            StrategyName: nameof(StandardBusinessHoursSlaCalculator));

    private static DateTimeOffset AddBusinessHours(DateTimeOffset start, int hours)
    {
        var remaining = TimeSpan.FromHours(hours);
        var current = MoveToBusinessHours(start);

        while (remaining > TimeSpan.Zero)
        {
            var endOfBusinessDay = new DateTimeOffset(
                current.Year,
                current.Month,
                current.Day,
                BusinessEnd.Hours,
                BusinessEnd.Minutes,
                BusinessEnd.Seconds,
                TimeSpan.Zero);

            var availableToday = endOfBusinessDay - current;

            if (remaining <= availableToday)
            {
                return current + remaining;
            }

            remaining -= availableToday;
            current = NextBusinessDayStart(endOfBusinessDay);
        }

        return current;
    }

    private static DateTimeOffset MoveToBusinessHours(DateTimeOffset value)
    {
        var current = value.ToUniversalTime();

        if (current.TimeOfDay < BusinessStart)
        {
            current = AtBusinessStart(current);
        }
        else if (current.TimeOfDay >= BusinessEnd)
        {
            current = NextBusinessDayStart(current);
        }

        while (current.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            current = AtBusinessStart(current.AddDays(1));
        }

        return current;
    }

    private static DateTimeOffset NextBusinessDayStart(DateTimeOffset current)
    {
        var next = AtBusinessStart(current.AddDays(1));

        while (next.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            next = AtBusinessStart(next.AddDays(1));
        }

        return next;
    }

    private static DateTimeOffset AtBusinessStart(DateTimeOffset value) =>
        new(
            value.Year,
            value.Month,
            value.Day,
            BusinessStart.Hours,
            BusinessStart.Minutes,
            BusinessStart.Seconds,
            TimeSpan.Zero);
}
