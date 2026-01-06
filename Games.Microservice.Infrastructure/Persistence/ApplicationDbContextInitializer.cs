using Games.Microservice.Domain.Entities;
using Games.Microservice.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
 

namespace Games.Microservice.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly GamesDbContext _context;

    public ApplicationDbContextInitialiser(GamesDbContext context)
    {
        _context = context;
    }

    public void Initialise()
    {
        // Early development strategy
        //_context.Database.EnsureDeleted();
        //_context.Database.EnsureCreated();
        //_context.Database.Migrate();

        // Late development strategy
        if (_context.Database.IsSqlServer())
        {
            _context.Database.Migrate();
        }
        else
        {
            _context.Database.EnsureCreated();
        }
    }

    public void Seed()
    {
        SeedGames();
    }

   
    
    private void SeedGames()
    {
        if (_context.Games.Any())
            return;

        if (_context.Database.IsSqlServer())
        {
            using var transaction = _context.Database.BeginTransaction();

     
            var games = GetGames();
            _context.Games.AddRange(games);
            _context.SaveChanges();

        
            transaction.Commit();
        }
        else
        {
            var games = GetGames();
            _context.Games.AddRange(games);
            _context.SaveChanges();
        }
    }

     
    public static List<Game> GetGames()
    {
        return new List<Game>
        {
            new Game { Name = "Cyberpunk 2077", Category = "Action RPG", Price = new CurrencyAmount(125.99M, "BRL") },
            new Game { Name = "Horizon Zero Dawn", Category = "Action RPG", Price = new CurrencyAmount(130.99M, "BRL") },
            new Game { Name = "Assassin's Creed Valhalla", Category = "Action-adventure", Price = new CurrencyAmount(150.40M, "BRL") },
            new Game { Name = "Sekiro: Shadows Die Twice", Category = "Action RPG" , Price = new CurrencyAmount(35.99M, "BRL")},
        };
    }
}
