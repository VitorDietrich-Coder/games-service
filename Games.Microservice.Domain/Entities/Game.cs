using Games.Microservice.Domain.Core;
using Games.Microservice.Domain.Events;
using Games.Microservice.Domain.ValueObjects;

namespace Games.Microservice.Domain.Entities;

public class Game : AggregateRoot
{
    private readonly List<IDomainEvent> _events = new();

    protected Game() { } // EF

    public Game(
        string title,
        string genre,
        Money price)
    {
        Id = Guid.NewGuid();
        Title = ValidateTitle(title);
        Genre = ValidateGenre(genre);
        Price = price;
        Purchases = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Genre { get; private set; }
    public Money Price { get; private set; }
    public int Purchases { get; private set; }
    public DateTime CreatedAt { get; private set; }


    public void RegisterPurchase()
    {
        Purchases++;

        AddDomainEvent(new GamePurchasedEvent(Id));
    }


    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent @event)
        => _events.Add(@event);

    public void ClearDomainEvents()
        => _events.Clear();

    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new Exception("Game title is required");

        return title;
    }

    private static string ValidateGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            throw new Exception("Game genre is required");

        return genre;
    }
}
