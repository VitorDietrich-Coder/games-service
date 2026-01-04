namespace Games.Microservice.Infrastructure.Persistence
{
    public interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken ct);
    }
}
