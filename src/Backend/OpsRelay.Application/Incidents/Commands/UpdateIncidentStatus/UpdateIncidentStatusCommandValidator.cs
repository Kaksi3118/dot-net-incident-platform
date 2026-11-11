using FluentValidation;

namespace OpsRelay.Application.Incidents.Commands.UpdateIncidentStatus;

public class UpdateIncidentStatusCommandValidator : AbstractValidator<UpdateIncidentStatusCommand>
{
    public UpdateIncidentStatusCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.NewStatus)
            .IsInEnum();

        RuleFor(command => command.Reason)
            .MaximumLength(500);
    }
}
