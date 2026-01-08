using Games.Microservice.Application.Queries;
using Games.Microservice.Domain.DTOs;
using Games.Microservice.Domain.DTOS;
using Games.Microservice.Domain.Interfaces;
using MediatR;

public sealed class GetPopularGamesQueryHandler
    : IRequestHandler<GetPopularGamesQuery, IEnumerable<PopularGameDto>>
{
    private readonly IGameSearchRepository _searchRepository;

    public GetPopularGamesQueryHandler(IGameSearchRepository gameSearchRepository)
    {
        _searchRepository = gameSearchRepository;
    }

    public async Task<IEnumerable<PopularGameDto>> Handle(
        GetPopularGamesQuery request,
        CancellationToken cancellationToken)
    {
        var response = await _searchRepository.GetPopularAsync();

        return response.Select(g => new PopularGameDto
        {
            GameId = g.Id,
            Title = g.Name,
            Category = g.Category,
            PurchaseCount = g.Purchases 
        });
    }
}
