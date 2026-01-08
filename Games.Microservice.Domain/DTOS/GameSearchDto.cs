using Games.Microservice.Domain.ValueObjects;

namespace Games.Microservice.Domain.DTOs;

public class GameSearchDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Category { get; set; } = default!;

    public CurrencyAmount Price { get; set; }

    public string Currency { get; set; } = default!;

    public int Purchases { get; set; }

    public DateTime CreatedAt { get; set; }
}
