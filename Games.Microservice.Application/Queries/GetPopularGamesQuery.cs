using Games.Microservice.Infrastructure.Search.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Application.Queries
{
    public record GetPopularGamesQuery
        : IRequest<IEnumerable<GameSearchDto>>;
}
