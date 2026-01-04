

using Users.Microservice.Domain.Core.Events;

namespace Games.Microservice.Domain.Events;

public sealed class GamePurchasedDomainEvent : DomainEvent
{
    public Guid AggregateId { get; }

    public GamePurchasedDomainEvent(Guid aggregateId) : base(aggregateId)
    {
        AggregateId = aggregateId;
    }
}
