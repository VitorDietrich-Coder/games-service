namespace Games.Microservice.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken ct);
    }
}
