using OpsRelay.Domain.Enums;

namespace OpsRelay.Domain.Services;

public static class IncidentStateMachine
{
    private static readonly Dictionary<IncidentStatus, HashSet<IncidentStatus>>
        Transitions = new()
        {
            [IncidentStatus.Reported] =
            [
                IncidentStatus.Triaged,
                IncidentStatus.Closed
            ],
            [IncidentStatus.Triaged] =
            [
                IncidentStatus.InProgress,
                IncidentStatus.Escalated,
                IncidentStatus.Closed
            ],
            [IncidentStatus.InProgress] =
            [
                IncidentStatus.Escalated,
                IncidentStatus.Resolved,
                IncidentStatus.Triaged
            ],
            [IncidentStatus.Escalated] =
            [
                IncidentStatus.InProgress,
                IncidentStatus.Resolved
            ],
            [IncidentStatus.Resolved] =
            [
                IncidentStatus.Closed,
                IncidentStatus.InProgress
            ],
            [IncidentStatus.Closed] = []
        };

    public static bool CanTransition(IncidentStatus current, IncidentStatus target) =>
        Transitions.TryGetValue(current, out var targets) && targets.Contains(target);
}
