namespace Games.Microservice.Infrastructure.Search.DTOs;

public class GameSearchDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string Genre { get; set; } = default!;

    public decimal Price { get; set; }

    public string Currency { get; set; } = default!;

    public int Purchases { get; set; }

    public DateTime CreatedAt { get; set; }
}
