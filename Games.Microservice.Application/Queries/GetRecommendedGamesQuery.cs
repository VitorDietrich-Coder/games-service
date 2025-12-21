using Games.Microservice.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Application.Queries
{
    public record GetRecommendedGamesQuery(Guid id)
        : IRequest<IEnumerable<Game>>;
}
