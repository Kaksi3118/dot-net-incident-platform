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

/// <summary>
/// Manages the incident lifecycle, including creation, retrieval and status changes.
/// </summary>
[ApiController]
[Route("api/v1/incidents")]
public class IncidentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<IncidentsController> _logger;

    public IncidentsController(IMediator mediator, ILogger<IncidentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Returns a paginated list of incidents, optionally filtered by status.
    /// </summary>
    /// <param name="pageNumber">One-based page number.</param>
    /// <param name="pageSize">Number of records per page (1-100).</param>
    /// <param name="status">Optional incident status filter.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
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

    /// <summary>
    /// Returns a single incident by its identifier.
    /// </summary>
    /// <param name="id">Incident identifier.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IncidentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IncidentDto>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var incident = await _mediator.Send(
            new GetIncidentByIdQuery(id),
            cancellationToken);

        if (incident is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "The requested resource was not found.",
                detail: $"Incident '{id}' was not found.",
                instance: Request.Path.Value);
        }

        return Ok(incident);
    }

    /// <summary>
    /// Creates a new incident and calculates its SLA deadline from the matching policy.
    /// </summary>
    /// <param name="request">Incident details.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    [HttpPost]
    [ProducesResponseType(typeof(IncidentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Changes the lifecycle status of an incident.
    /// </summary>
    /// <param name="id">Incident identifier.</param>
    /// <param name="request">Requested status and optional reason.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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
