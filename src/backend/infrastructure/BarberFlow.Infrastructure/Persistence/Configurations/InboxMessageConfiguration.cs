using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class InboxMessageConfiguration : EntityConfiguration<InboxMessage>
{
    protected override void ConfigureEntity(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_messages");

        builder.Property(message => message.MessageId)
            .HasColumnName("message_id")
            .IsRequired();

        builder.Property(message => message.Consumer)
            .HasColumnName("consumer")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(message => message.ProcessedAtUtc)
            .HasColumnName("processed_at_utc")
            .IsRequired();

        builder.HasIndex(message => new { message.MessageId, message.Consumer })
            .IsUnique()
            .HasDatabaseName("ux_inbox_messages_message_id_consumer");
    }
}
