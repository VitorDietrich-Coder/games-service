using Games.Microservice.Domain.DTOs;
using Games.Microservice.Domain.DTOS;
using Games.Microservice.Domain.Entities;
using Games.Microservice.Domain.Interfaces;
using Nest;


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
                            .Field(x => x.Name)
                            .Field(x => x.Category))
                        .Query(term))));

            return response.Documents;
        }

        public async Task<IEnumerable<GameSearchDto>> GetPopularAsync()
        {
            var response = await _client.SearchAsync<GameSearchDto>(s => s
                .Index("games")
                .Size(5)
                .Sort(sort => sort
                    .Descending(f => f.Purchases)));

            if (!response.IsValid)
                return Enumerable.Empty<GameSearchDto>();

            return response.Documents;
        }


        public async Task<IEnumerable<GameSearchDto>> GetRecommendedAsync(Guid userId)
        {
            var categoriesResponse = await _client.SearchAsync<UserPurchaseDto>(s => s
               .Index("user-purchases")
               .Size(0)
               .Query(q => q
                   .Term(t => t
                       .Field("userId.keyword")
                       .Value(userId.ToString())))
               .Aggregations(a => a
                   .Terms("top_categories", t => t
                       .Field("category.keyword")
                       .Size(3))));


            if (!categoriesResponse.IsValid || !categoriesResponse.Aggregations.Any())
                return Enumerable.Empty<GameSearchDto>();

            var categories = categoriesResponse
                .Aggregations
                .Terms("top_categories")
                .Buckets
                .Select(b => b.Key)
                .ToArray();


            var gamesResponse = await _client.SearchAsync<GameSearchDto>(s => s
               .Index("games")
               .Size(5)
               .Query(q => q
                   .Terms(t => t
                       .Field("category.keyword")
                       .Terms(categories))));

            return gamesResponse.Documents;
        }

    }

}
