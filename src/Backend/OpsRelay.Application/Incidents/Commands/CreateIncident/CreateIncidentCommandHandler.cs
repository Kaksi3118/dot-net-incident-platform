using MediatR;
using Microsoft.Extensions.Logging;
using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Application.DTOs;
using OpsRelay.Application.Mappings;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Exceptions;
using OpsRelay.Domain.Repositories;
using OpsRelay.Domain.ValueObjects;

namespace OpsRelay.Application.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandHandler(
    IIncidentRepository incidentRepository,
    ISlaPolicyRepository slaPolicyRepository,
    ISlaCalculator slaCalculator,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ILogger<CreateIncidentCommandHandler> logger)
    : IRequestHandler<CreateIncidentCommand, IncidentDto>
{
    public async Task<IncidentDto> Handle(
        CreateIncidentCommand request,
        CancellationToken cancellationToken)
    {
        var reportedAt = timeProvider.GetUtcNow();

        var policy = await slaPolicyRepository.GetByPriorityAsync(
            request.Priority,
            cancellationToken);

        if (policy is null)
        {
            throw new DomainException(
                $"No SLA policy is configured for priority '{request.Priority}'.");
        }

        var slaDeadline = slaCalculator.CalculateDeadline(reportedAt, policy, timeProvider);

        var incident = new Incident(
            Guid.NewGuid(),
            IncidentTitle.Create(request.Title),
            request.Description,
            request.Priority,
            TeamId.Create(request.AssignedTeamId),
            reportedAt,
            slaDeadline);

        await incidentRepository.AddAsync(incident, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Incident {IncidentId} created with priority {Priority} and SLA deadline {SlaDeadline}",
            incident.Id,
            incident.Priority,
            incident.SlaDeadline);

        return incident.ToDto();
    }
}
