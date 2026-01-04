using Games.Microservice.Domain.Entities;
using Games.Microservice.Infrastructure.EventStore;
using Games.Microservice.Infrastructure.MapEntities;
using Microsoft.EntityFrameworkCore;
using Users.Microservice.Domain.Core.Events;

namespace Games.Microservice.Infrastructure.Persistence
{
    public class GamesDbContext : DbContext
    {
        public GamesDbContext(DbContextOptions<GamesDbContext> options)
            : base(options) { }

        public DbSet<Game> Games => Set<Game>();
        public DbSet<StoredEvent> StoredEvents => Set<StoredEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<DomainEvent>();
            modelBuilder.Ignore<List<DomainEvent>>();
            modelBuilder.Ignore<IReadOnlyCollection<DomainEvent>>();

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(GamesConfiguration).Assembly
            );

        }


    }
}
