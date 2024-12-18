namespace Wiseshare.Domain.Common.Models;

// Interface for objects that can trigger domain events
// Provides a consistent structure for handling domain events across all entities.
public interface IHasDomainEvents
{
    // A list of domain events associated with the object
    // Reason: Enables tracking of significant events for processing later.
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }
    // Clears all domain events
    // Allows the system to mark events as processed and prevent duplicates.
    public void ClearDomainEvents();
}