using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Microservice.Infrastructure.EventStore;

public class StoredEventMap : IEntityTypeConfiguration<StoredEvent>
{
    public void Configure(EntityTypeBuilder<StoredEvent> builder)
    {
        builder.ToTable("StoredEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Data)
            .IsRequired();

        builder.Property(e => e.OccurredAt)
            .IsRequired();

        builder.HasIndex(e => e.AggregateId);
    }
}
