using Games.Microservice.Application.Commands;
using Games.Microservice.Domain.Events;
using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Infrastructure.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Application.Handlers
{
    public class PurchaseGameHandler
     : IRequestHandler<PurchaseGameCommand, bool>
    {
        private IGameRepository _repo;
        private IEventBus _eventBus;

        public PurchaseGameHandler(
            IGameRepository repo,
            IEventBus eventBus)
        {
            _repo = repo;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(
            PurchaseGameCommand request,
            CancellationToken cancellationToken)
        {
            var game = await _repo.GetByIdAsync(request.GameId);

            if (game is null)
                throw new Exception("Jogo não encontrado");

      

            await _eventBus.PublishAsync(new GamePurchaseRequestedEvent(
                game.Id,
                request.UserId,
                game.Price
            ), "Purchase Game");

            return true;
        }
    }


}
