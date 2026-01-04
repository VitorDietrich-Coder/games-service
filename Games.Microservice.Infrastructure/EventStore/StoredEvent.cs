namespace Games.Microservice.Infrastructure.EventStore;

public class StoredEvent
{
    public Guid Id { get; set; }

    public Guid AggregateId { get; set; }

    public string CorrelationId { get; set; } = default!;

    public string Type { get; set; } = default!;

    public string Data { get; set; } = default!;

    public DateTime OccurredAt { get; set; }
}
