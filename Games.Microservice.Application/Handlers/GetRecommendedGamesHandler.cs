using Games.Microservice.Application.Queries;
using Games.Microservice.Domain.DTOs;
using Games.Microservice.Domain.Entities;
using Games.Microservice.Domain.Interfaces;
 
using MediatR;

namespace Games.Microservice.Application.Handlers;

public sealed class GetRecommendedGamesHandler
    : IRequestHandler<GetRecommendedGamesQuery, IEnumerable<GameSearchDto>>
{
    private readonly IGameSearchRepository _searchRepository;

    public GetRecommendedGamesHandler(IGameSearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<IEnumerable<GameSearchDto>> Handle(
        GetRecommendedGamesQuery request,
        CancellationToken cancellationToken)
    {
        return await _searchRepository.GetRecommendedAsync(request.id);
    }
}
