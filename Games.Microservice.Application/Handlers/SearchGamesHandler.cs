using Games.Microservice.Application.Queries;
using Games.Microservice.Infrastructure.Interfaces;
using Games.Microservice.Infrastructure.Search.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Application.Handlers
{
    public class SearchGamesHandler
      : IRequestHandler<SearchGamesQuery, IEnumerable<GameSearchDto>>
    {
        private readonly IGameSearchRepository _search;

        public SearchGamesHandler(IGameSearchRepository search)
        {
            _search = search;
        }

        public async Task<IEnumerable<GameSearchDto>> Handle(
            SearchGamesQuery request,
            CancellationToken cancellationToken)
        {
            return await _search.SearchAsync(request.Term);
        }
    }

}
