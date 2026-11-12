using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsRelay.Infrastructure.Persistence.Entities;

namespace OpsRelay.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.EventType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(message => message.Payload)
            .IsRequired();

        builder.Property(message => message.OccurredOn)
            .IsRequired();

        builder.Property(message => message.ProcessedOn);

        builder.OwnsOne(message => message.Metadata, metadata => metadata.ToJson());

        builder.HasIndex(message => message.ProcessedOn);
    }
}
