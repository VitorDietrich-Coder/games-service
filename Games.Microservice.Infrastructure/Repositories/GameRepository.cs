using Games.Microservice.Domain.Entities;
using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task AddAsync(Game game)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Game game)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }

}
