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

public class CreateIncidentCommandHandler
    : IRequestHandler<CreateIncidentCommand, IncidentDto>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly ISlaPolicyRepository _slaPolicyRepository;
    private readonly ISlaCalculator _slaCalculator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateIncidentCommandHandler> _logger;

    public CreateIncidentCommandHandler(
        IIncidentRepository incidentRepository,
        ISlaPolicyRepository slaPolicyRepository,
        ISlaCalculator slaCalculator,
        IUnitOfWork unitOfWork,
        ILogger<CreateIncidentCommandHandler> logger)
    {
        _incidentRepository = incidentRepository;
        _slaPolicyRepository = slaPolicyRepository;
        _slaCalculator = slaCalculator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IncidentDto> Handle(
        CreateIncidentCommand request,
        CancellationToken cancellationToken)
    {
        var reportedAt = DateTimeOffset.UtcNow;

        var policy = await _slaPolicyRepository.GetByPriorityAsync(
            request.Priority,
            cancellationToken);

        if (policy is null)
        {
            throw new DomainException(
                $"No SLA policy is configured for priority '{request.Priority}'.");
        }

        var slaDeadline = _slaCalculator.CalculateDeadline(reportedAt, policy);

        var incident = new Incident(
            Guid.NewGuid(),
            IncidentTitle.Create(request.Title),
            request.Description,
            request.Priority,
            TeamId.Create(request.AssignedTeamId),
            reportedAt,
            slaDeadline);

        await _incidentRepository.AddAsync(incident, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Incident {IncidentId} created with priority {Priority} and SLA deadline {SlaDeadline}",
            incident.Id,
            incident.Priority,
            incident.SlaDeadline);

        return incident.ToDto();
    }
}
