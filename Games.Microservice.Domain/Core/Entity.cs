using Games.Microservice.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Microservice.Domain.Core.Events;

namespace Games.Microservice.Domain.Core
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        private readonly List<DomainEvent> _domainEvents;
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;

        protected Entity()
        {
            _domainEvents = new List<DomainEvent>();
        }
        protected void AddDomainEvent(DomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
