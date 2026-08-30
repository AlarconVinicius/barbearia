using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class AppointmentConfiguration : EntityConfiguration<Appointment>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_appointments_time_range",
                "starts_at_utc < ends_at_utc");
            tableBuilder.HasCheckConstraint(
                "ck_appointments_different_users",
                "customer_id <> employee_id");
        });

        builder.Property(appointment => appointment.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(appointment => appointment.EmployeeId)
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(appointment => appointment.StartsAtUtc)
            .HasColumnName("starts_at_utc")
            .IsRequired();

        builder.Property(appointment => appointment.EndsAtUtc)
            .HasColumnName("ends_at_utc")
            .IsRequired();

        builder.Property(appointment => appointment.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(appointment => appointment.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

        builder.Property(appointment => appointment.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");

        builder.Property(appointment => appointment.CancelledByUserId)
            .HasColumnName("cancelled_by_user_id");

        builder.Property(appointment => appointment.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(500);

        builder.HasIndex(appointment => new
        {
            appointment.EmployeeId,
            appointment.Status,
            appointment.StartsAtUtc
        })
            .HasDatabaseName("ix_appointments_employee_status_start");

        builder.HasIndex(appointment => new
        {
            appointment.CustomerId,
            appointment.StartsAtUtc
        })
            .HasDatabaseName("ix_appointments_customer_start");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(appointment => appointment.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(appointment => appointment.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(appointment => appointment.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(appointment => appointment.CancelledByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
