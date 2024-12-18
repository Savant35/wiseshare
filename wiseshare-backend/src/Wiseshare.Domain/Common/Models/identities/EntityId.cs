namespace Wiseshare.Domain.Common.Models;

public abstract class EntityId<TId> : ValueObject
{
    public TId Value { get; }
    // Constructor to initialize the ID value
    // Ensures the ID is always set and cannot change after creation.
    protected EntityId(TId value)
    {
        Value = value;
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
    // Provides a string representation of the ID
    // Simplifies debugging and logging by displaying the ID value.
    public override string? ToString() => Value?.ToString() ?? base.ToString();

#pragma warning disable CS8618
    protected EntityId()
    {
    }
#pragma warning restore CS8618
}