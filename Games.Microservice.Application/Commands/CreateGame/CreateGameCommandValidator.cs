using FluentValidation;
using Games.Microservice.Application.Commands.CreateGame;
using Games.Microservice.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace FGC.Application.Games.Commands.CreateGame
{
    public class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
    {
        private readonly IGameRepository _gameRepository;

        public CreateGameCommandValidator(IGameRepository gameRepository)
        {

            _gameRepository = gameRepository;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Game name is required.")
                .Matches(@"^[A-Za-z0-9\s]+$")
                .WithMessage("Game name can only contain letters, numbers, and spaces.")
                .MaximumLength(100)
                .WithMessage("Game name must be at most 100 characters long.")
                .MustAsync(async (name, cancellation) =>
                {
                    var exists = await _gameRepository.GetByNameAsync(name) is null;
                    return !exists;
                })
                .WithMessage("A game with this name already exists.");


            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Genre is required.")
                .Matches(@"^[A-Za-z\s]+$")
                .WithMessage("Category can only contain letters and spaces.")
                .MaximumLength(100)
                .WithMessage("Category must be at most 100 characters long.");

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage("Price is required.")
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.")
                .LessThanOrEqualTo(1000)
                .WithMessage("Price must be less than or equal to 1000.");

        }
    }
}