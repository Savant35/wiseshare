namespace Wiseshare.Domain.Common.Models;

public abstract class Entity<TId> : IEquatable<Entity<TId>>, IHasDomainEvents
    where TId : ValueObject
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public TId Id { get; protected set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity(TId id)//constructure to ensure every entity ahs a unique ID
    {
        Id = id;
    }

    public override bool Equals(object? obj)//determines if two entites are queal based on thier IDs
    {
        return obj is Entity<TId> entity && Id.Equals(entity.Id);
    }

    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }
        //equality operateor for comparing entites
        //enfores that entites must have a valid ID when created
    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !Equals(left, right);
    }
    // Adds a domain event to the entity's event list
    // Enables the entity to notify other parts of the system about significant changes.
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    // Clears all domain events from the entity
    // Resets the event list after events have been processed, preventing duplicate handling.
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

#pragma warning disable CS8618
    protected Entity()
    {
    }
#pragma warning restore CS8618
}