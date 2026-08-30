using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class AuthenticationCodeConfiguration : EntityConfiguration<AuthenticationCode>
{
    protected override void ConfigureEntity(EntityTypeBuilder<AuthenticationCode> builder)
    {
        builder.ToTable("authentication_codes", tableBuilder =>
            tableBuilder.HasCheckConstraint(
                "ck_authentication_codes_attempt_count",
                "attempt_count >= 0 AND attempt_count <= 3"));

        builder.Property(code => code.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(code => code.Purpose)
            .HasColumnName("purpose")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(code => code.CodeHash)
            .HasColumnName("code_hash")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(code => code.ExpiresAtUtc)
            .HasColumnName("expires_at_utc")
            .IsRequired();

        builder.Property(code => code.AttemptCount)
            .HasColumnName("attempt_count")
            .IsRequired();

        builder.Property(code => code.UsedAtUtc)
            .HasColumnName("used_at_utc");

        builder.Property(code => code.InvalidatedAtUtc)
            .HasColumnName("invalidated_at_utc");

        builder.Property(code => code.LockedUntilUtc)
            .HasColumnName("locked_until_utc");

        builder.HasIndex(code => new { code.UserId, code.Purpose, code.CreatedAtUtc })
            .HasDatabaseName("ix_authentication_codes_user_purpose_created_at");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(code => code.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
