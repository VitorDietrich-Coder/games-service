using Games.Microservice.Domain.Entities;
using Games.Microservice.Infrastructure.Search.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Infrastructure.Interfaces
{
    public interface IGameSearchRepository
    {
        Task IndexAsync(Game game);
        Task<IEnumerable<GameSearchDto>> SearchAsync(string term);
        Task<IEnumerable<GameSearchDto>> GetRecommendedAsync(Guid userId);
        Task<IEnumerable<GameSearchDto>> GetPopularAsync();
    }
}
