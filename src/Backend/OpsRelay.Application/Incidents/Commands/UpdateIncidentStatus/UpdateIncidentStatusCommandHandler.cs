using MediatR;
using Microsoft.Extensions.Logging;
using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Domain.Repositories;

namespace OpsRelay.Application.Incidents.Commands.UpdateIncidentStatus;

public class UpdateIncidentStatusCommandHandler(
    IIncidentRepository incidentRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateIncidentStatusCommandHandler> logger)
    : IRequestHandler<UpdateIncidentStatusCommand>
{
    public async Task Handle(
        UpdateIncidentStatusCommand request,
        CancellationToken cancellationToken)
    {
        var incident = await incidentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (incident is null)
        {
            throw new KeyNotFoundException($"Incident '{request.Id}' was not found.");
        }

        incident.ChangeStatus(request.NewStatus, request.Reason, DateTimeOffset.UtcNow);

        incidentRepository.Update(incident);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Incident {IncidentId} status changed to {Status}",
            incident.Id,
            incident.Status);
    }
}
