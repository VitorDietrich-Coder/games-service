 

namespace Games.Microservice.Domain.Events;

public record GamePurchasedEvent(
    Guid GameId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
