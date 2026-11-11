using FluentValidation;

namespace OpsRelay.Application.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(command => command.Priority)
            .IsInEnum();

        RuleFor(command => command.AssignedTeamId)
            .NotEmpty()
            .MaximumLength(50);
    }
}
