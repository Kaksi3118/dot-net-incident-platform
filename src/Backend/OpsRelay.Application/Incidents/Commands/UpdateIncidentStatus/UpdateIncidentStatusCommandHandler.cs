using MediatR;
using Microsoft.Extensions.Logging;
using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Domain.Repositories;

namespace OpsRelay.Application.Incidents.Commands.UpdateIncidentStatus;

public class UpdateIncidentStatusCommandHandler : IRequestHandler<UpdateIncidentStatusCommand>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateIncidentStatusCommandHandler> _logger;

    public UpdateIncidentStatusCommandHandler(
        IIncidentRepository incidentRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateIncidentStatusCommandHandler> logger)
    {
        _incidentRepository = incidentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(
        UpdateIncidentStatusCommand request,
        CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (incident is null)
        {
            throw new KeyNotFoundException($"Incident '{request.Id}' was not found.");
        }

        incident.ChangeStatus(request.NewStatus, request.Reason, DateTimeOffset.UtcNow);

        _incidentRepository.Update(incident);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Incident {IncidentId} status changed to {Status}",
            incident.Id,
            incident.Status);
    }
}
