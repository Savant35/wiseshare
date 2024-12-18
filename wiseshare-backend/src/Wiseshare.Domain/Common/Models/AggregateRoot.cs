namespace Wiseshare.Domain.Common.Models;

public abstract class AggregateRoot<TId, TIdType> : Entity<TId>
    where TId : AggregateRootId<TIdType>
{   
    // Strongly-typed ID for the Aggregate Root
    // Ensures clarity and type safety for IDs specific to Aggregate Roots.
    public new AggregateRootId<TIdType> Id { get; protected set; }
    // Constructor to ensure every Aggregate Root has a unique ID
    // Guarantees that the Aggregate Root is always created with a valid identifier.
    protected AggregateRoot(TId id)
    {
        Id = id;
    }

#pragma warning disable CS8618
    protected AggregateRoot()
    {
    }
#pragma warning restore CS8618
}