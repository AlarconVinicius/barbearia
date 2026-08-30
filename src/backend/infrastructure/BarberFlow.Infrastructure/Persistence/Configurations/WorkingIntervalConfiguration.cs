using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class WorkingIntervalConfiguration : EntityConfiguration<WorkingInterval>
{
    protected override void ConfigureEntity(EntityTypeBuilder<WorkingInterval> builder)
    {
        builder.ToTable("working_intervals", tableBuilder =>
            tableBuilder.HasCheckConstraint(
                "ck_working_intervals_time_range",
                "starts_at < ends_at"));

        builder.Property(interval => interval.EmployeeId)
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(interval => interval.DayOfWeek)
            .HasColumnName("day_of_week")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(interval => interval.StartsAt)
            .HasColumnName("starts_at")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(interval => interval.EndsAt)
            .HasColumnName("ends_at")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.HasIndex(interval => new { interval.EmployeeId, interval.DayOfWeek })
            .HasDatabaseName("ix_working_intervals_employee_id_day_of_week");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(interval => interval.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
