using OpsRelay.Domain.Enums;

namespace OpsRelay.Domain.Exceptions;

public class InvalidStatusTransitionException : DomainException
{
    public InvalidStatusTransitionException(
        IncidentStatus currentStatus,
        IncidentStatus targetStatus)
        : base($"Cannot transition an incident from '{currentStatus}' to '{targetStatus}'.")
    {
        CurrentStatus = currentStatus;
        TargetStatus = targetStatus;
    }

    public IncidentStatus CurrentStatus { get; }

    public IncidentStatus TargetStatus { get; }
}
