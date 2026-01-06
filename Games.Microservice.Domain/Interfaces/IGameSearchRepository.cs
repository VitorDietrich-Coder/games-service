using Games.Microservice.Domain.Entities;
using Games.Microservice.Infrastructure.Search.DTOs;

namespace Games.Microservice.Domain.Interfaces
{
    public interface IGameSearchRepository
    {
        Task IndexAsync(Game game);
        Task<IEnumerable<GameSearchDto>> SearchAsync(string term);
        Task<IEnumerable<GameSearchDto>> GetRecommendedAsync(Guid userId);
        Task<IEnumerable<GameSearchDto>> GetPopularAsync();
    }
}
