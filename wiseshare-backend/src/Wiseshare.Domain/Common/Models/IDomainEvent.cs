using MediatR;

namespace Wiseshare.Domain.Common.Models;
// Interface for representing domain events
// Reason: Encapsulates changes in the domain and integrates with MediatR for asynchronous handling.
public interface IDomainEvent : INotification
{
}