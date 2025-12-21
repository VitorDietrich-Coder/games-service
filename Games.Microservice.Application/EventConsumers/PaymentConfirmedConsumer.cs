using Games.Microservice.Domain.Events;
using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Infrastructure.Interfaces;
using Games.Microservice.Infrastructure.Repositories;

namespace Games.Microservice.Application.EventConsumers
{
    public class PaymentConfirmedConsumer
    {
        private readonly IGameRepository _repo;
        private readonly IGameSearchRepository _search;

        public PaymentConfirmedConsumer(
            IGameRepository repo,
            IGameSearchRepository search)
        {
            _repo = repo;
            _search = search;
        }

        public async Task Handle(PaymentConfirmedEvent @event)
        {
            var game = await _repo.GetByIdAsync(@event.GameId);

            game.RegisterPurchase();

            await _repo.UpdateAsync(game);
            await _search.IndexAsync(game); 
        }
    }

}
