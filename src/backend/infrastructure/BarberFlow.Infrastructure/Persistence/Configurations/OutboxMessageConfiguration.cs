using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration : EntityConfiguration<OutboxMessage>
{
    protected override void ConfigureEntity(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", tableBuilder =>
            tableBuilder.HasCheckConstraint(
                "ck_outbox_messages_attempt_count",
                "attempt_count >= 0"));

        builder.Property(message => message.OccurredAtUtc)
            .HasColumnName("occurred_at_utc")
            .IsRequired();

        builder.Property(message => message.Type)
            .HasColumnName("type")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(message => message.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(message => message.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(100);

        builder.Property(message => message.ProcessedAtUtc)
            .HasColumnName("processed_at_utc");

        builder.Property(message => message.AttemptCount)
            .HasColumnName("attempt_count")
            .IsRequired();

        builder.Property(message => message.LastAttemptAtUtc)
            .HasColumnName("last_attempt_at_utc");

        builder.Property(message => message.LastError)
            .HasColumnName("last_error")
            .HasMaxLength(2000);

        builder.HasIndex(message => new { message.ProcessedAtUtc, message.OccurredAtUtc })
            .HasDatabaseName("ix_outbox_messages_processed_at_occurred_at");
    }
}
