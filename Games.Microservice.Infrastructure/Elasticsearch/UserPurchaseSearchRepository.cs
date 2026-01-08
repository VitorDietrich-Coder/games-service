using Games.Microservice.Domain.Interfaces;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Infrastructure.Elasticsearch
{
    public sealed class UserPurchaseSearchRepository : IUserPurchaseSearchRepository
    {
        private readonly IElasticClient _client;

        public UserPurchaseSearchRepository(IElasticClient client)
        {
            _client = client;
        }

        public async Task IndexAsync(UserPurchaseDocument document)
        {
            await _client.IndexAsync(document, i => i
                .Index("user-purchases"));
        }
    }

}
