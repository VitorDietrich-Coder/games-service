using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Application.Behaviours
{
    public interface ICorrelationIdAccessor
    {
        string CorrelationId { get; }
    }

}
