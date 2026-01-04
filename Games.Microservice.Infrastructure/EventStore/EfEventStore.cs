using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Infrastructure.Persistence;
using System.Text.Json;
using Users.Microservice.Domain.Core.Events;
 

namespace Games.Microservice.Infrastructure.EventStore
{
    public class StoredEvents : IEventStore
    {
        private readonly GamesDbContext _context;

        public StoredEvents(GamesDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(DomainEvent @event)
        {
            var storedEvent = new StoredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = @event.AggregateId,
                CorrelationId = @event.CorrelationId,
                Type = @event.GetType().Name,
                Data = JsonSerializer.Serialize(@event),
                OccurredAt = @event.OccurredAt
            };

            _context.StoredEvents.Add(storedEvent);
    
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId)
        {
            var events = _context.StoredEvents
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.OccurredAt)
                .ToList();

            return events.Select(e =>
                JsonSerializer.Deserialize<DomainEvent>(e.Data)!);
        }
    }
}
