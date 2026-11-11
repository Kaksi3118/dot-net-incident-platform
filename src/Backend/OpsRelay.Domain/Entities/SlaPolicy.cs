using OpsRelay.Domain.Common;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Exceptions;

namespace OpsRelay.Domain.Entities;

public class SlaPolicy : Entity<Guid>
{
    public const int MaxNameLength = 100;

    private SlaPolicy()
    {
        Name = null!;
    }

    public SlaPolicy(
        Guid id,
        string name,
        int responseTimeHours,
        int resolutionTimeHours,
        Priority priority)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("SLA policy name is required.");
        }

        if (name.Trim().Length > MaxNameLength)
        {
            throw new DomainException(
                $"SLA policy name must not exceed {MaxNameLength} characters.");
        }

        if (responseTimeHours <= 0)
        {
            throw new DomainException("SLA response time must be greater than zero.");
        }

        if (resolutionTimeHours <= 0)
        {
            throw new DomainException("SLA resolution time must be greater than zero.");
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = name.Trim();
        ResponseTimeHours = responseTimeHours;
        ResolutionTimeHours = resolutionTimeHours;
        Priority = priority;
    }

    public string Name { get; private set; }

    public int ResponseTimeHours { get; private set; }

    public int ResolutionTimeHours { get; private set; }

    public Priority Priority { get; private set; }
}
