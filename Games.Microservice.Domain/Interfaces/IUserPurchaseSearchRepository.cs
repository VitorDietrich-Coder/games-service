using Games.Microservice.Infrastructure.Elasticsearch;
 

namespace Games.Microservice.Domain.Interfaces
{
    public interface IUserPurchaseSearchRepository
    {
        Task IndexAsync(UserPurchaseDocument document);
    }

}
