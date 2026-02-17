using Games.Microservice.Contracts;
using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Infrastructure.Elasticsearch;
using Microsoft.Extensions.Logging;


namespace Games.Microservice.Application.EventConsumers
{
    public class PaymentConfirmedConsumer
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameSearchRepository _gameSearch;
        private readonly IUserPurchaseSearchRepository _userPurchaseSearchRepository;
        private readonly ILogger<PaymentCompletedIntegrationEvent> _logger;

        public PaymentConfirmedConsumer(
            IGameRepository gameRepo,
            IGameSearchRepository gameSearch,
            IUserPurchaseSearchRepository purchaseSearchRepository,
            IUnitOfWork unitOfWork,
            ILogger<PaymentCompletedIntegrationEvent> logger)
        {
            _gameRepository = gameRepo;
            _gameSearch = gameSearch;
            _unitOfWork = unitOfWork;
            _userPurchaseSearchRepository = purchaseSearchRepository;
        }

        public async Task Handle(PaymentCompletedIntegrationEvent @event)
        {
            _logger.LogInformation("Handling PaymentCompletedIntegrationEvent for GameId: {GameId}, UserId: {UserId}", @event.GameId, @event.UserId);

            var game = await _gameRepository.GetByIdAsync(@event.GameId);
            if (game is null)
            {
                _logger.LogWarning("Game not found for GameId: {GameId}", @event.GameId);
                return;
            }

            _logger.LogInformation("Registering purchase for GameId: {GameId}", game.Id);
            game.RegisterPurchase();

            await _gameRepository.UpdateAsync(game);
            await _unitOfWork.CommitAsync(default);
            _logger.LogInformation("Updated game and committed transaction for GameId: {GameId}", game.Id);

            await _gameSearch.IndexAsync(game);
            _logger.LogInformation("Indexed game in Elasticsearch for GameId: {GameId}", game.Id);

            var purchase = new UserPurchaseDocument
            {
                UserId = @event.UserId,
                GameId = game.Id,
                Category = game.Category,
                PurchasedAt = DateTime.UtcNow
            };

            await _userPurchaseSearchRepository.IndexAsync(purchase);
            _logger.LogInformation("Indexed user purchase in Elasticsearch for UserId: {UserId}, GameId: {GameId}", purchase.UserId, purchase.GameId);
        }  
    }

}
