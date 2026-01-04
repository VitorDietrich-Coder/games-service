using Games.Microservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;

namespace Games.Microservice.Infrastructure.MapEntities
{
    public class GamesConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasDatabaseName("GamesName");

            builder.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Purchases)
                 .IsRequired()
                 .HasMaxLength(100);

            builder.OwnsOne(x => x.Price, price =>
            {
                price.Property(p => p.Value)
                     .HasColumnName("Price")
                     .IsRequired();

                price.Property(p => p.Currency)
                     .HasColumnName("PriceCurrency")
                     .IsRequired()
                     .HasMaxLength(3);
            });

            builder.Property(u => u.CreatedAt)
                    .IsRequired();
        }
    }
}
