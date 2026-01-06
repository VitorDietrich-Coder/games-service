namespace Games.Microservice.Contracts
{
    public record PaymentCompletedIntegrationEvent(
        Guid PaymentId,
        Guid GameId,
        Guid UserId,
        decimal Price,
        string CorrelationId
    );
}
