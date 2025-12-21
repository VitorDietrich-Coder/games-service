using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Domain.Events
{
    public interface IDomainEvent
    {
        public DateTime OccurredOn { get; }
    }
}
