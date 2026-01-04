using Games.Microservice.Domain.Core;
using Games.Microservice.Domain.Events;
using Games.Microservice.Domain.ValueObjects;

namespace Games.Microservice.Domain.Entities;

public class Game : Entity
{
 
    public Game() { }  

    public Game(
        string name,
        string category,
        CurrencyAmount price)
    {
        Id = Guid.NewGuid();
        Name = ValidateName(name);
        Category = ValidateCategory(category);
        Price = price;
        Purchases = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get;  set; }
    public string Name { get;  set; }
    public string Category { get;  set; }
    public CurrencyAmount Price { get;  set; }
    public int Purchases { get;  set; }
    public DateTime CreatedAt { get;  set; }


    public void RegisterPurchase()
    {
        Purchases++;

        AddDomainEvent(new GamePurchasedDomainEvent(Id));
    }
 
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Game title is required");

        return name;
    }

    private static string ValidateCategory(string Category)
    {
        if (string.IsNullOrWhiteSpace(Category))
            throw new Exception("Game Category is required");

        return Category;
    }
}
