using Games.Microservice.Domain.Entities;
using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Infrastructure.Interfaces;
using Games.Microservice.Infrastructure.Search.DTOs;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Infrastructure.Elasticsearch
{
    public class GameSearchRepository : IGameSearchRepository
    {
        private readonly IElasticClient _client;

        public GameSearchRepository(IElasticClient client)
        {
            _client = client;
        }

        public async Task IndexAsync(Game game)
        {
            await _client.IndexDocumentAsync(game);
        }

        public async Task<IEnumerable<GameSearchDto>> SearchAsync(string term)
        {
            var response = await _client.SearchAsync<GameSearchDto>(s => s
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(x => x.Title)
                            .Field(x => x.Genre))
                        .Query(term))));

            return response.Documents;
        }

        public async Task<IEnumerable<GameSearchDto>> GetPopularAsync()
        {
            var response = await _client.SearchAsync<GameSearchDto>(s => s
                .Size(5)
                .Sort(srt => srt
                    .Descending("purchases")));

            return response.Documents;
        }

        public async Task<IEnumerable<GameSearchDto>> GetRecommendedAsync(Guid userId)
        {
            // Simples: recomenda por gêneros mais comprados
            var response = await _client.SearchAsync<GameSearchDto>(s => s
                .Size(5)
                .Query(q => q.MatchAll()));

            return response.Documents;
        }
    }

}
