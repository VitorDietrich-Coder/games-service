using Games.Microservice.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Application.Queries
{
    public record SearchGamesQuery(string Term)
        : IRequest<IEnumerable<GameSearchDto>>;
}
