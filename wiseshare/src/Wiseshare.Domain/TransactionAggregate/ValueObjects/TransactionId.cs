using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Domain.TransactionAggregate;

// Represents a unique identifier for a Transaction, tied to a specific User and Property
public sealed class TransactionId : AggregateRootId<string>
{
    // Private constructor accepting a string value
    private TransactionId(string value) : base(value)
    {
    }

    // Composite constructor combining UserId and PropertyId to form TransactionId
    private TransactionId(UserId userId, PropertyId propertyId)
        : base($"Transaction_{userId.Value}_{propertyId.Value}_{Guid.NewGuid()}")
    {
    }

    // Static method to create a new TransactionId using UserId and PropertyId
    public static TransactionId Create(UserId userId, PropertyId propertyId)
    {
        return new TransactionId(userId, propertyId);
    }

    // Static method to create a TransactionId from an existing string value
    public static TransactionId Create(string value)
    {
        return new TransactionId(value);
    }
}