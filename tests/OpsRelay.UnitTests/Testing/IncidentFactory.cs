using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.ValueObjects;

namespace OpsRelay.UnitTests.Testing;

public static class IncidentFactory
{
    public static readonly DateTimeOffset ReportedAt =
        new(2026, 11, 16, 9, 0, 0, TimeSpan.Zero);

    public static Incident Create(
        Priority priority = Priority.Critical,
        Guid? id = null)
    {
        var incident = new Incident(
            id ?? Guid.NewGuid(),
            IncidentTitle.Create("Database outage"),
            "Primary database unavailable",
            priority,
            TeamId.Create("SRE-Ops"),
            ReportedAt,
            ReportedAt.AddHours(4));

        incident.ClearDomainEvents();

        return incident;
    }
}
