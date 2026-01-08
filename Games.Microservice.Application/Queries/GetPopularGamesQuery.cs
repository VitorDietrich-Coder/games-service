using Games.Microservice.Domain.DTOs;
using Games.Microservice.Domain.DTOS;
using MediatR;

namespace Games.Microservice.Application.Queries
{
    public record GetPopularGamesQuery
        : IRequest<IEnumerable<PopularGameDto>>;
}
