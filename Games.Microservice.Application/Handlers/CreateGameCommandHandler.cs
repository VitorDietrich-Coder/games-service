 
using Games.Microservice.Application.Commands.CreateGame;
using Games.Microservice.Application.Games.Models.Response;
using Games.Microservice.Domain.Entities;
using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Domain.ValueObjects;
using MediatR;

namespace Games.Microservice.Application.Handlers;


public sealed class CreateGameCommandHandler
    : IRequestHandler<CreateGameCommand, GameResponse>
{
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGameCommandHandler(IGameRepository gameRepository, IUnitOfWork unitOfWork)
    {
        _gameRepository = gameRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GameResponse> Handle(
        CreateGameCommand request,
        CancellationToken cancellationToken)
    {
        var game = new Game(
            name: request.Name,
            category: request.Category,
            price: new CurrencyAmount(request.Price)
        );

        await _gameRepository.AddAsync(game);

        await _unitOfWork.CommitAsync(cancellationToken);

        return (GameResponse)game;
    }
}