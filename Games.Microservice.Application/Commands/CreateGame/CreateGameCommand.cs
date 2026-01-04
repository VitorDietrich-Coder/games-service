using Games.Microservice.Application.Games.Models.Response;
using MediatR;

namespace Games.Microservice.Application.Commands.CreateGame;

public sealed record CreateGameCommand(
    string Name,
    string Category,
    decimal Price
) : IRequest<GameResponse>;