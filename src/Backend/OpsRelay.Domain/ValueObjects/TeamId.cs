using OpsRelay.Domain.Common;
using OpsRelay.Domain.Exceptions;

namespace OpsRelay.Domain.ValueObjects;

public sealed class TeamId : ValueObject
{
    public const int MaxLength = 50;

    private TeamId(string value) => Value = value;

    public string Value { get; }

    public static TeamId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Assigned team is required.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
        {
            throw new DomainException($"Assigned team must not exceed {MaxLength} characters.");
        }

        return new TeamId(trimmed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
