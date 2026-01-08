using Games.Microservice.Contracts;
using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Infrastructure.Elasticsearch;


namespace Games.Microservice.Application.EventConsumers
{
    public class PaymentConfirmedConsumer
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameSearchRepository _gameSearch;
        private readonly IUserPurchaseSearchRepository _userPurchaseSearchRepository;

        public PaymentConfirmedConsumer(
            IGameRepository gameRepo,
            IGameSearchRepository gameSearch,
            IUserPurchaseSearchRepository purchaseSearchRepository,
            IUnitOfWork unitOfWork
            )
        {
            _gameRepository = gameRepo;
            _gameSearch = gameSearch;
            _unitOfWork = unitOfWork;
            _userPurchaseSearchRepository = purchaseSearchRepository;
        }

        public async Task Handle(PaymentCompletedIntegrationEvent @event)
        {
            var game = await _gameRepository.GetByIdAsync(@event.GameId);
            if (game is null)
                return;

 
            game.RegisterPurchase();
 
            await _gameRepository.UpdateAsync(game);

            await _unitOfWork.CommitAsync(default);
 
            await _gameSearch.IndexAsync(game);

            var purchase = new UserPurchaseDocument
            {
                UserId = @event.UserId,
                GameId = game.Id,
                Category = game.Category, 
                PurchasedAt = DateTime.UtcNow
            };

            await _userPurchaseSearchRepository.IndexAsync(purchase);
        }
    }

}
