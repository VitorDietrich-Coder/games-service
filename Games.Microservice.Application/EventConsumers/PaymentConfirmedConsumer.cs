using Games.Microservice.Contracts;
using Games.Microservice.Domain.Interfaces;
 

namespace Games.Microservice.Application.EventConsumers
{
    public class PaymentConfirmedConsumer
    {
        private readonly IGameRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameSearchRepository _search;

        public PaymentConfirmedConsumer(
            IGameRepository repo,
            IGameSearchRepository search,
            IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _search = search;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PaymentCompletedIntegrationEvent @event)
        {
            var game = await _repo.GetByIdAsync(@event.GameId);

            game.RegisterPurchase();

            await _repo.UpdateAsync(game);
            await _unitOfWork.CommitAsync(default);

            await _search.IndexAsync(game); 
        }
    }

}
