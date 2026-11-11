using OpsRelay.Domain.Common;
using OpsRelay.Domain.Exceptions;

namespace OpsRelay.Domain.ValueObjects;

public sealed class IncidentTitle : ValueObject
{
    public const int MinLength = 5;
    public const int MaxLength = 200;

    private IncidentTitle(string value) => Value = value;

    public string Value { get; }

    public static IncidentTitle Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Incident title is required.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
        {
            throw new DomainException(
                $"Incident title must be between {MinLength} and {MaxLength} characters.");
        }

        return new IncidentTitle(trimmed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
