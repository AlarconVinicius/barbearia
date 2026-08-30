using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class AppointmentRequestConfiguration : EntityConfiguration<AppointmentRequest>
{
    protected override void ConfigureEntity(EntityTypeBuilder<AppointmentRequest> builder)
    {
        builder.ToTable("appointment_requests", tableBuilder =>
            tableBuilder.HasCheckConstraint(
                "ck_appointment_requests_different_users",
                "customer_id <> employee_id"));

        builder.Property(request => request.IdempotencyKey)
            .HasColumnName("idempotency_key")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(request => request.RequestedByUserId)
            .HasColumnName("requested_by_user_id")
            .IsRequired();

        builder.Property(request => request.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(request => request.EmployeeId)
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(request => request.RequestedStartsAtUtc)
            .HasColumnName("requested_starts_at_utc")
            .IsRequired();

        builder.Property(request => request.Type)
            .HasColumnName("type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(request => request.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(request => request.RejectionReason)
            .HasColumnName("rejection_reason")
            .HasConversion<int?>();

        builder.Property(request => request.RejectionDetails)
            .HasColumnName("rejection_details")
            .HasMaxLength(1000);

        builder.Property(request => request.AppointmentId)
            .HasColumnName("appointment_id");

        builder.Property(request => request.ProcessedAtUtc)
            .HasColumnName("processed_at_utc");

        builder.HasIndex(request => new
        {
            request.RequestedByUserId,
            request.IdempotencyKey
        })
            .IsUnique()
            .HasDatabaseName("ux_appointment_requests_requested_by_idempotency_key");

        builder.HasIndex(request => new
        {
            request.EmployeeId,
            request.Status,
            request.RequestedStartsAtUtc
        })
            .HasDatabaseName("ix_appointment_requests_employee_status_start");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(request => request.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(request => request.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(request => request.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Appointment>()
            .WithMany()
            .HasForeignKey(request => request.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
