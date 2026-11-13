using FluentAssertions;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Services;
using Xunit;

namespace OpsRelay.UnitTests.Domain;

public class IncidentStateMachineTests
{
    public static IEnumerable<object[]> TransitionMatrix()
    {
        foreach (var current in Enum.GetValues<IncidentStatus>())
        {
            foreach (var target in Enum.GetValues<IncidentStatus>())
            {
                yield return new object[] { current, target, IsAllowed(current, target) };
            }
        }
    }

    [Theory]
    [MemberData(nameof(TransitionMatrix))]
    public void CanTransitionMatchesTheLifecycleMatrix(
        IncidentStatus current,
        IncidentStatus target,
        bool expected) =>
        IncidentStateMachine.CanTransition(current, target).Should().Be(expected);

    private static bool IsAllowed(IncidentStatus current, IncidentStatus target) =>
        current switch
        {
            IncidentStatus.Reported =>
                target is IncidentStatus.Triaged or IncidentStatus.Closed,
            IncidentStatus.Triaged =>
                target is IncidentStatus.InProgress
                    or IncidentStatus.Escalated
                    or IncidentStatus.Closed,
            IncidentStatus.InProgress =>
                target is IncidentStatus.Escalated
                    or IncidentStatus.Resolved
                    or IncidentStatus.Triaged,
            IncidentStatus.Escalated =>
                target is IncidentStatus.InProgress or IncidentStatus.Resolved,
            IncidentStatus.Resolved =>
                target is IncidentStatus.Closed or IncidentStatus.InProgress,
            IncidentStatus.Closed => false,
            _ => false
        };
}
