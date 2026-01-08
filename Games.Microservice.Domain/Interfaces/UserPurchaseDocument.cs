using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Infrastructure.Elasticsearch
{
    public sealed class UserPurchaseDocument
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public string Category { get; set; } = default!;
        public DateTime PurchasedAt { get; set; }
    }

}
