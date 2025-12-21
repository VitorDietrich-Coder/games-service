using Games.Microservice.Domain.Entities;

namespace Games.Microservice.Domain.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Game>> GetAllAsync();

    Task AddAsync(Game game);

    Task UpdateAsync(Game game);

    Task<bool> ExistsAsync(Guid id);
}
