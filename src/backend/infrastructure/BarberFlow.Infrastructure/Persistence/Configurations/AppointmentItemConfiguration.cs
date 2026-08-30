using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class AppointmentItemConfiguration : EntityConfiguration<AppointmentItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<AppointmentItem> builder)
    {
        builder.ToTable("appointment_items", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_appointment_items_unit_price", "unit_price >= 0");
            tableBuilder.HasCheckConstraint("ck_appointment_items_duration", "duration_minutes > 0");
        });

        builder.Property(item => item.AppointmentId)
            .HasColumnName("appointment_id")
            .IsRequired();

        builder.Property(item => item.ServiceId)
            .HasColumnName("service_id")
            .IsRequired();

        builder.Property(item => item.ServiceName)
            .HasColumnName("service_name")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasColumnName("unit_price")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(item => item.DurationMinutes)
            .HasColumnName("duration_minutes")
            .IsRequired();

        builder.HasIndex(item => item.AppointmentId)
            .HasDatabaseName("ix_appointment_items_appointment_id");

        builder.HasOne<Appointment>()
            .WithMany()
            .HasForeignKey(item => item.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Service>()
            .WithMany()
            .HasForeignKey(item => item.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
