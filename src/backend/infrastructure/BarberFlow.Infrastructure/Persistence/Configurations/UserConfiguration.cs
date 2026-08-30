using BarberFlow.Domain.Entities;
using BarberFlow.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : EntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(user => user.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasColumnName("email")
            .HasConversion(email => email.Value, value => new Email(value))
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(user => user.PhoneNumber)
            .HasColumnName("phone_number")
            .HasConversion(phoneNumber => phoneNumber.Value, value => new PhoneNumber(value))
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(user => user.Cpf)
            .HasColumnName("cpf")
            .HasConversion(cpf => cpf.Value, value => new Cpf(value))
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique()
            .HasDatabaseName("ux_users_email");

        builder.HasIndex(user => user.Cpf)
            .IsUnique()
            .HasDatabaseName("ux_users_cpf");
    }
}
