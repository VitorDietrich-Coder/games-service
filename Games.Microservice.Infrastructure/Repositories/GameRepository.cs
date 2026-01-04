using Games.Microservice.Domain.Entities;
using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace Games.Microservice.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GamesDbContext _context;

        public GameRepository(GamesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
            => await _context.Games.ToListAsync();

        public async Task<IEnumerable<Game>> GetRecommendedAsync()
            => await _context.Games
                .OrderByDescending(g => g.Price)
                .Take(5)
                .ToListAsync();

        public async Task<Game?> GetByIdAsync(Guid id)
            => await _context.Games.FindAsync(id);

        Task<IReadOnlyList<Game>> IGameRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(Game user)
        {
            await _context.Games.AddAsync(user);
            await Task.CompletedTask;
        }


        public async Task<Game?> UpdateAsync(Game game)
        {
            var existingGame = await _context.Games
                .FirstOrDefaultAsync(u => u.Id == game.Id);

            if (existingGame is null)
                return null;

            _context.Entry(existingGame).CurrentValues.SetValues(game);

            return existingGame;
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Game?> GetByNameAsync(string name)
            => await _context.Games.FindAsync(name);

    }

}
