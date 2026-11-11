using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpsRelay.Api.Models.Requests;
using OpsRelay.Application.Common.Models;
using OpsRelay.Application.DTOs;
using OpsRelay.Application.Incidents.Commands.CreateIncident;
using OpsRelay.Application.Incidents.Commands.UpdateIncidentStatus;
using OpsRelay.Application.Incidents.Queries.GetIncidentById;
using OpsRelay.Application.Incidents.Queries.GetIncidentsPaged;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class IncidentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<IncidentsController> _logger;

    public IncidentsController(IMediator mediator, ILogger<IncidentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<IncidentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<IncidentDto>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] IncidentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await _mediator.Send(
            new GetIncidentsPagedQuery(pageNumber, pageSize, status),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IncidentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IncidentDto>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var incident = await _mediator.Send(
            new GetIncidentByIdQuery(id),
            cancellationToken);

        if (incident is null)
        {
            return NotFound();
        }

        return Ok(incident);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IncidentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IncidentDto>> Create(
        [FromBody] CreateIncidentRequest request,
        CancellationToken cancellationToken = default)
    {
        var incident = await _mediator.Send(
            new CreateIncidentCommand(
                request.Title,
                request.Description,
                request.Priority,
                request.AssignedTeamId),
            cancellationToken);

        _logger.LogInformation("Incident {IncidentId} created through the API", incident.Id);

        return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateIncidentStatusCommand(id, request.NewStatus, request.Reason),
            cancellationToken);

        return NoContent();
    }
}
