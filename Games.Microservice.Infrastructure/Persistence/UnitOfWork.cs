using Games.Microservice.Domain.Interfaces;

namespace Games.Microservice.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GamesDbContext _context;

        public UnitOfWork(GamesDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
