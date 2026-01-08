using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Domain.DTOS
{
    public sealed class PopularGameDto
    {
        public Guid GameId { get; init; }
        public string Title { get; init; } = default!;
        public string Category { get; init; } = default!;
        public int PurchaseCount { get; init; }
    }

}
