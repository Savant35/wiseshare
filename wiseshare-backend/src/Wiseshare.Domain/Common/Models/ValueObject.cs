namespace Wiseshare.Domain.Common.Models;
// Base class for immutable objects compared by their values
// Ensures consistency when working with objects that represent concepts like "Money" or "Address."
public abstract class ValueObject : IEquatable<ValueObject>
{
    // Abstract method that specifies which values are used for equality checks
    // Forces derived classes to define what makes them equal.
    public abstract IEnumerable<object?> GetEqualityComponents();
    // Determines if two ValueObjects are equal based on their components
    // Provides a consistent way to compare ValueObjects by their contents.
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var valueObject = (ValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(valueObject.GetEqualityComponents());
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }
}