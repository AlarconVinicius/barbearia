using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class ServiceConfiguration : EntityConfiguration<Service>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_services_price", "price >= 0");
            tableBuilder.HasCheckConstraint("ck_services_duration", "duration_minutes > 0");
        });

        builder.Property(service => service.Name)
            .HasColumnName("name")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(service => service.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(service => service.Price)
            .HasColumnName("price")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(service => service.DurationMinutes)
            .HasColumnName("duration_minutes")
            .IsRequired();

        builder.Property(service => service.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasIndex(service => service.Name)
            .HasDatabaseName("ix_services_name");
    }
}
