using OpsRelay.Domain.Common;
using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Events;
using OpsRelay.Domain.Exceptions;
using OpsRelay.Domain.Services;
using OpsRelay.Domain.ValueObjects;

namespace OpsRelay.Domain.Entities;

public class Incident : AggregateRoot<Guid>
{
    public const int MaxDescriptionLength = 2000;

    private Incident()
    {
        Title = null!;
        Description = null!;
        AssignedTeamId = null!;
    }

    public Incident(
        Guid id,
        IncidentTitle title,
        string description,
        Priority priority,
        TeamId assignedTeamId,
        DateTimeOffset reportedAt,
        DateTimeOffset slaDeadline)
    {
        if (title is null)
        {
            throw new DomainException("Incident title is required.");
        }

        if (description is null)
        {
            throw new DomainException("Incident description is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Incident description is required.");
        }

        if (description.Length > MaxDescriptionLength)
        {
            throw new DomainException(
                $"Incident description must not exceed {MaxDescriptionLength} characters.");
        }

        if (assignedTeamId is null)
        {
            throw new DomainException("Assigned team is required.");
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Title = title;
        Description = description.Trim();
        Priority = priority;
        Status = IncidentStatus.Reported;
        ReportedAt = reportedAt;
        SlaDeadline = slaDeadline;
        AssignedTeamId = assignedTeamId;

        AddDomainEvent(new IncidentCreatedDomainEvent(Id, Priority, ReportedAt, SlaDeadline));
    }

    public IncidentTitle Title { get; private set; }

    public string Description { get; private set; }

    public Priority Priority { get; private set; }

    public IncidentStatus Status { get; private set; }

    public DateTimeOffset ReportedAt { get; private set; }

    public DateTimeOffset? ResolvedAt { get; private set; }

    public DateTimeOffset SlaDeadline { get; private set; }

    public TeamId AssignedTeamId { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    public void ChangeStatus(
        IncidentStatus newStatus,
        string? reason,
        TimeProvider timeProvider)
    {
        if (!IncidentStateMachine.CanTransition(Status, newStatus))
        {
            throw new InvalidStatusTransitionException(Status, newStatus);
        }

        var previousStatus = Status;
        var changedAt = timeProvider.GetUtcNow();

        Status = newStatus;

        if (newStatus == IncidentStatus.Resolved)
        {
            ResolvedAt = changedAt;
        }
        else if (previousStatus == IncidentStatus.Resolved)
        {
            ResolvedAt = null;
        }

        AddDomainEvent(
            new IncidentStatusChangedDomainEvent(Id, previousStatus, newStatus, reason, changedAt));
    }
}
