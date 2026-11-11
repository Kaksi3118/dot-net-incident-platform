namespace OpsRelay.Domain.Common;

public abstract class Entity<TId>
    where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        return ((Entity<TId>)obj).Id.Equals(Id);
    }

    public override int GetHashCode() => Id.GetHashCode();
}
